using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Factory;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Transport.Logging;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Transport;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

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
        var orderRequest = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.001m,
            Price: null,
            TriggerPrice: null,
            TimeInForce: null,
            MinuteToExpire: null,
            ClientOrderId: null);

        var orderResult = await client.SendOrderAsync(orderRequest);
        Assert.False(string.IsNullOrWhiteSpace(orderResult.OrderId));

        // 3. poll status（初回429後にCOMPLETED）
        var status = await client.PollOrderStatusAsync(
            productCode: "BTC_JPY",
            childOrderAcceptanceId: orderResult.OrderId,
            pollInterval: TimeSpan.FromMilliseconds(10),
            maxAttempts: 3);

        Assert.Equal(OrderStatusType.Completed, status.Status);

        // 4. executions（履歴取得）
        var executions = await client.GetExecutionsAsync(Symbols.BtcJpy);
        Assert.NotEmpty(executions);

        // 呼び出し回数が劣化環境を通ったことを確認
        Assert.True(transport.BalanceCalls >= 2);
        Assert.True(transport.ChildOrderCalls >= 2);
        Assert.True(transport.ExecutionCalls >= 1);
    }

    private sealed class DegradedBitflyerTransport : IHttpTransport
    {
        public int BalanceCalls { get; private set; }
        public int SendOrderCalls { get; private set; }
        public int ChildOrderCalls { get; private set; }
        public int ExecutionCalls { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            if (path.Contains("/v1/me/getbalance", StringComparison.OrdinalIgnoreCase))
            {
                BalanceCalls++;
                if (BalanceCalls == 1)
                {
                    return Task.FromResult(new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("{\"error_message\":\"too many\"}", Encoding.UTF8, "application/json")
                    });
                }

                var json = "[{\"currency_code\":\"JPY\",\"amount\":1000000,\"available\":1000000},{\"currency_code\":\"BTC\",\"amount\":1,\"available\":1}]";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (path.Contains("/v1/me/sendchildorder", StringComparison.OrdinalIgnoreCase))
            {
                SendOrderCalls++;
                var json = "{\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\"}";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            if (path.Contains("/v1/me/getchildorders", StringComparison.OrdinalIgnoreCase))
            {
                ChildOrderCalls++;
                if (ChildOrderCalls == 1)
                {
                    return Task.FromResult(new HttpResponseMessage((HttpStatusCode)429)
                    {
                        Content = new StringContent("{\"error_message\":\"too many\"}", Encoding.UTF8, "application/json")
                    });
                }

                var now = DateTime.UtcNow;
                var json = $"[{{\"id\":1,\"child_order_id\":\"JFX123\",\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"child_order_type\":\"MARKET\",\"price\":0,\"average_price\":4000000,\"size\":0.001,\"child_order_state\":\"COMPLETED\",\"expire_date\":\"{now:O}\",\"child_order_date\":\"{now:O}\",\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\",\"outstanding_size\":0,\"cancel_size\":0,\"executed_size\":0.001,\"total_commission\":0}}]";

                // 少し遅延を挟んで劣化環境を模擬
                return Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken).ContinueWith(_ =>
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    }, cancellationToken);
            }

            if (path.Contains("/v1/me/getexecutions", StringComparison.OrdinalIgnoreCase))
            {
                ExecutionCalls++;
                var now = DateTime.UtcNow;
                var json = $"[{{\"id\":1,\"child_order_acceptance_id\":\"JRF20240101-000000-abcdef\",\"product_code\":\"BTC_JPY\",\"side\":\"BUY\",\"price\":4000000,\"size\":0.001,\"exec_date\":\"{now:O}\"}}]";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }

            // デフォルト: 200 ok
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }
    }
}
