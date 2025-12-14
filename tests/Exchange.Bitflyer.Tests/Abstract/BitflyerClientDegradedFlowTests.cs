using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Transport.Logging;
using Common.Transport.Policy;
using Common.Transport.Transport;
using Xunit;

namespace Exchange.Bitflyer.Tests;

public class BitflyerClientDegradedFlowTests
{
    [Fact]
    public async Task RepresentativeFlow_Succeeds_Under429AndDelay()
    {
        var transport = new DegradedBitflyerTransport();
        var policyOptions = new HttpPolicyOptions
        {
            MaxRetryAttemptsForGet = 3,
            MaxRetryAttemptsForOther = 2,
            RetryBaseDelay = TimeSpan.FromMilliseconds(5),
            RetryMaxDelay = TimeSpan.FromMilliseconds(20),
            RequestsPerSecond = 5,
            RateLimitBurst = 2,
            Timeout = TimeSpan.FromSeconds(2),
            CircuitBreakerFailureThreshold = 3,
            CircuitBreakerOpenDuration = TimeSpan.FromSeconds(1)
        };

        var client = BitflyerTestClientFactory.CreateWithTransport(
            transport,
            policy: HttpPolicyFactory.CreateDefault(policyOptions),
            logger: NoOpRestClientLogger.Instance,
            observer: NoOpRestCallObserver.Instance,
            errorClassifier: null);

        // 1. balances（初回429後に成功）
        var balances = await client.GetBalancesAsync();
        Assert.NotEmpty(balances);

        // 2. send order（劣化環境だが成功）
        var orderResult = await client.PlaceMarketOrderAsync(Symbol.BtcJpy, Side.Buy, 0.001m);
        Assert.False(string.IsNullOrWhiteSpace(orderResult.OrderId));

        // 3. poll status（初回429後にCOMPLETED）
        var status = await client.PollOrderStatusAsync(
            productCode: "BTC_JPY",
            childOrderAcceptanceId: orderResult.OrderId,
            pollInterval: TimeSpan.FromMilliseconds(10),
            maxAttempts: 3);

        Assert.Equal(OrderState.Completed, status.Status);

        // 4. executions（約定履歴）
        var executions = await client.GetMarketExecutionsAsync("BTC/JPY");
        Assert.NotEmpty(executions);

        // 5. child orders 履歴（完了済みの履歴が返る）
        var childOrders = await client.GetOrdersAsync("BTC_JPY");
        Assert.NotEmpty(childOrders);

        // 6. positions（建玉ありの確認）
        var positionsBeforeClose = await client.GetOpenPositionsAsync("BTC_JPY");
        Assert.NotEmpty(positionsBeforeClose);

        // 7. close order（反対売買で決済）→ poll status
        var closeOrderResult = await client.PlaceMarketOrderAsync(Symbol.BtcJpy, Side.Sell, 0.001m);
        Assert.False(string.IsNullOrWhiteSpace(closeOrderResult.OrderId));

        var closeStatus = await client.PollOrderStatusAsync(
            productCode: "BTC_JPY",
            childOrderAcceptanceId: closeOrderResult.OrderId,
            pollInterval: TimeSpan.FromMilliseconds(10),
            maxAttempts: 3);

        Assert.Equal(OrderState.Completed, closeStatus.Status);

        // 8. positions（決済後に空）
        var positionsAfterClose = await client.GetOpenPositionsAsync("BTC_JPY");
        Assert.Empty(positionsAfterClose);

        // 9. collateral（口座状態確認）
        var collateral = await client.GetCollateralAsync();
        Assert.True(collateral.Amount > 0);

        // 呼び出し回数が劣化環境を通ったことを確認
        Assert.True(transport.BalanceCalls >= 2);
        Assert.True(transport.ChildOrderCalls >= 2);
        Assert.True(transport.ExecutionCalls >= 1);
        Assert.True(transport.CollateralCalls >= 1);
        Assert.True(transport.PositionCalls >= 1);
    }

    private sealed class DegradedBitflyerTransport : IHttpTransport
    {
        public int BalanceCalls { get; private set; }
        public int SendOrderCalls { get; private set; }
        public int ChildOrderCalls { get; private set; }
        public int ExecutionCalls { get; private set; }
        public int CancelCalls { get; private set; }
        public int CollateralCalls { get; private set; }
        public int PositionCalls { get; private set; }

        private decimal _positionSize = 0;

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            if (path.Contains("/v1/me/getbalance", StringComparison.OrdinalIgnoreCase))
            {
                BalanceCalls++;
                if (BalanceCalls == 1)
                {
                    return new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("{\"error_message\":\"too many\"}", Encoding.UTF8, "application/json")
                    };
                }

                var json = "[{\"currency_code\":\"JPY\",\"amount\":1000000,\"available\":1000000},{\"currency_code\":\"BTC\",\"amount\":1,\"available\":1}]";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/sendchildorder", StringComparison.OrdinalIgnoreCase))
            {
                SendOrderCalls++;
                var body = request.Content != null
                    ? await request.Content.ReadAsStringAsync(cancellationToken)
                    : string.Empty;

                // ざっくり JSON を読む（Side と Size のみ）
                var side = body.Contains("\"side\":\"SELL\"", StringComparison.OrdinalIgnoreCase)
                    ? Side.Sell
                    : Side.Buy;

                var size = 0.0m;
                const string sizeKey = "\"size\":";
                var sizeIndex = body.IndexOf(sizeKey, StringComparison.OrdinalIgnoreCase);
                if (sizeIndex >= 0)
                {
                    var after = body[(sizeIndex + sizeKey.Length)..];
                    var end = after.IndexOfAny(new[] { ',', '}', ' ' });
                    var sizeStr = end >= 0 ? after[..end] : after;
                    decimal.TryParse(sizeStr, out size);
                }

                if (side == Side.Buy)
                {
                    _positionSize += size;
                }
                else
                {
                    _positionSize = Math.Max(0, _positionSize - size);
                }

                var json = "{\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\"}";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/cancelallchildorders", StringComparison.OrdinalIgnoreCase)
                || path.Contains("/v1/me/cancelchildorder", StringComparison.OrdinalIgnoreCase))
            {
                CancelCalls++;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/getchildorders", StringComparison.OrdinalIgnoreCase))
            {
                ChildOrderCalls++;
                if (ChildOrderCalls == 1)
                {
                    return new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("{\"error_message\":\"too many\"}", Encoding.UTF8, "application/json")
                    };
                }

                var now = DateTime.UtcNow;
                var json = $"[{{\"id\":1,\"child_order_id\":\"JFX123\",\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"child_order_type\":\"MARKET\",\"price\":0,\"average_price\":4000000,\"size\":0.001,\"child_order_state\":\"COMPLETED\",\"expire_date\":\"{now:O}\",\"child_order_date\":\"{now:O}\",\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\",\"outstanding_size\":0,\"cancel_size\":0,\"executed_size\":0.001,\"total_commission\":0}}]";

                // 少し遅延を挟んで劣化環境を模擬
                await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/getexecutions", StringComparison.OrdinalIgnoreCase))
            {
                ExecutionCalls++;
                var now = DateTime.UtcNow;
                var json = $"[{{\"id\":1,\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"price\":4000000,\"size\":0.001,\"exec_date\":\"{now:O}\"}}]";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/getexecutions", StringComparison.OrdinalIgnoreCase))
            {
                ExecutionCalls++;
                var now = DateTime.UtcNow;
                var json = $"[{{\"id\":1,\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\",\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"price\":4000000,\"size\":0.001,\"exec_date\":\"{now:O}\"}}]";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/getcollateral", StringComparison.OrdinalIgnoreCase))
            {
                CollateralCalls++;
                var json = "{\"collateral\":1000000,\"open_position_pnl\":0,\"require_collateral\":0,\"keep_rate\":10}";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
            }

            if (path.Contains("/v1/me/getpositions", StringComparison.OrdinalIgnoreCase))
            {
                PositionCalls++;
                var now = DateTime.UtcNow;
                if (_positionSize > 0)
                {
                    var json = $"[{{\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"price\":4000000,\"size\":{_positionSize},\"commission\":0,\"swap_point_accumulate\":0,\"require_collateral\":1000,\"open_date\":\"{now:O}\",\"leverage\":4}}]";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                }

                var emptyJson = "[]";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(emptyJson, Encoding.UTF8, "application/json")
                };
            }

            // デフォルト: 200 ok
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }
    }
}
