using ExchangeApi.Adapters.McpServer.Schema.Inspection;
using ExchangeApi.Adapters.McpServer.Tools.Inspection;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using NativeBalanceHistory = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using NativeChildOrders = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using NativeCollateralAccounts = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using NativeCollateralHistory = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class BitflyerInspectionToolsTests
{
    [Fact]
    public async Task GetCollateralAccountsAsync_ReturnsMappedReadOnlyResponse()
    {
        var tool = new BitflyerInspectionTools(
            new FakeInspectionGateway
            {
                CollateralAccountsCall = Success(
                    new NativeCollateralAccounts.GetCollateralAccountsRequest(),
                    (IReadOnlyList<NativeCollateralAccounts.GetCollateralAccounts.Item>)
                    [
                        new NativeCollateralAccounts.GetCollateralAccounts.Item
                        {
                            CurrencyCode = "JPY",
                            Amount = 1000.5m,
                        },
                    ],
                    "GetCollateralAccounts"),
            });

        var result = await tool.GetCollateralAccountsAsync(new GetCollateralAccountsRequest());

        Assert.True(result.IsSuccess);
        var account = Assert.Single(result.Response!.Accounts);
        Assert.Equal("JPY", account.CurrencyCode);
        Assert.Equal("1000.5", account.Amount);
    }

    [Fact]
    public async Task GetChildOrdersAsync_MapsFilterToNativeReadRequest()
    {
        var gateway = new FakeInspectionGateway
        {
            ChildOrdersCall = Success(
                new NativeChildOrders.GetChildOrdersRequest(),
                (IReadOnlyList<NativeChildOrders.GetChildOrders.Item>)
                [
                    new NativeChildOrders.GetChildOrders.Item
                    {
                        Id = 1,
                        ChildOrderId = "JRF",
                        ProductCode = "BTC_JPY",
                        Side = OrderSides.Buy,
                        ChildOrderType = ChildOrderTypes.Limit,
                        Price = 100m,
                        AveragePrice = 99m,
                        Size = 0.01m,
                        ChildOrderState = ChildOrderStates.Active,
                        ExpireDate = DateTimeOffset.Parse("2026-04-26T00:00:00Z"),
                        ChildOrderDate = DateTimeOffset.Parse("2026-04-25T00:00:00Z"),
                        ChildOrderAcceptanceId = "JRF-ACCEPT",
                        OutstandingSize = 0.01m,
                        CancelSize = 0m,
                        ExecutedSize = 0m,
                        TotalCommission = 0m,
                        TimeInForce = TimeInForces.Gtc,
                    },
                ],
                "GetChildOrders"),
        };
        var tool = new BitflyerInspectionTools(gateway);

        var result = await tool.GetChildOrdersAsync(
            new GetChildOrdersRequest
            {
                ProductCode = "BTC_JPY",
                Count = 10,
                ChildOrderState = "ACTIVE",
            });

        Assert.True(result.IsSuccess);
        Assert.Equal("BTC_JPY", gateway.LastChildOrdersRequest!.ProductCode);
        Assert.Equal(10, gateway.LastChildOrdersRequest.Count);
        Assert.Equal(ChildOrderStates.Active, gateway.LastChildOrdersRequest.ChildOrderState);
        var order = Assert.Single(result.Response!.Orders);
        Assert.Equal("BUY", order.Side);
        Assert.Equal("LIMIT", order.ChildOrderType);
        Assert.Equal("ACTIVE", order.ChildOrderState);
    }

    [Fact]
    public async Task GetBalanceHistoryAsync_DoesNotExposeSecretFromUpstreamError()
    {
        var tool = new BitflyerInspectionTools(
            new FakeInspectionGateway
            {
                BalanceHistoryCall = Failure<NativeBalanceHistory.GetBalanceHistoryRequest, IReadOnlyList<NativeBalanceHistory.GetBalanceHistory.Item>>("GetBalanceHistory"),
            });

        var result = await tool.GetBalanceHistoryAsync(new GetBalanceHistoryRequest());

        Assert.False(result.IsSuccess);
        Assert.Equal("inspection_read_unavailable", result.Error!.ErrorCode);
        Assert.DoesNotContain("apiKey", result.Error.Details.Keys);
        Assert.DoesNotContain("signature", result.Error.Details.Keys);
    }

    [Fact]
    public async Task GetChildOrdersAsync_RejectsUnsupportedStateBeforeGatewayCall()
    {
        var gateway = new FakeInspectionGateway();
        var tool = new BitflyerInspectionTools(gateway);

        var result = await tool.GetChildOrdersAsync(new GetChildOrdersRequest { ChildOrderState = "BAD" });

        Assert.False(result.IsSuccess);
        Assert.Equal("validation_error", result.Error!.ErrorCategory);
        Assert.Null(gateway.LastChildOrdersRequest);
    }

    private static CallResult<TRequest, TResponse> Success<TRequest, TResponse>(
        TRequest request,
        TResponse response,
        string endpointId)
    {
        return new CallResult<TRequest, TResponse>
        {
            Request = request,
            Response = response,
            IsSuccess = true,
            Error = null,
            Meta = TestCallMeta(endpointId),
        };
    }

    private static CallResult<TRequest, TResponse> Failure<TRequest, TResponse>(string endpointId)
    {
        return new CallResult<TRequest, TResponse>
        {
            Request = Activator.CreateInstance<TRequest>(),
            Response = default,
            IsSuccess = false,
            Error = new CallError { Kind = CallErrorKinds.Transport, Message = "upstream failed" },
            Meta = TestCallMeta(endpointId),
        };
    }

    private static CallMeta TestCallMeta(string endpointId)
    {
        return new CallMeta
        {
            Layer = CallLayers.Tests,
            Component = CallComponents.Factory,
            EndpointId = endpointId,
            Scope = "Private",
            Auth = "KeySecret",
            Children = null,
        };
    }

    private sealed class FakeInspectionGateway : IBitflyerInspectionGateway
    {
        public CallResult<NativeCollateralAccounts.GetCollateralAccountsRequest, IReadOnlyList<NativeCollateralAccounts.GetCollateralAccounts.Item>>? CollateralAccountsCall { get; init; }

        public CallResult<NativeBalanceHistory.GetBalanceHistoryRequest, IReadOnlyList<NativeBalanceHistory.GetBalanceHistory.Item>>? BalanceHistoryCall { get; init; }

        public CallResult<NativeCollateralHistory.GetCollateralHistoryRequest, IReadOnlyList<NativeCollateralHistory.GetCollateralHistory.Item>>? CollateralHistoryCall { get; init; }

        public CallResult<NativeChildOrders.GetChildOrdersRequest, IReadOnlyList<NativeChildOrders.GetChildOrders.Item>>? ChildOrdersCall { get; init; }

        public NativeChildOrders.GetChildOrdersRequest? LastChildOrdersRequest { get; private set; }

        public Task<CallResult<NativeCollateralAccounts.GetCollateralAccountsRequest, IReadOnlyList<NativeCollateralAccounts.GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(CollateralAccountsCall ?? throw new InvalidOperationException("CollateralAccountsCall must be configured."));
        }

        public Task<CallResult<NativeBalanceHistory.GetBalanceHistoryRequest, IReadOnlyList<NativeBalanceHistory.GetBalanceHistory.Item>>> GetBalanceHistoryAsync(
            NativeBalanceHistory.GetBalanceHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            _ = request;
            _ = cancellationToken;
            return Task.FromResult(BalanceHistoryCall ?? throw new InvalidOperationException("BalanceHistoryCall must be configured."));
        }

        public Task<CallResult<NativeCollateralHistory.GetCollateralHistoryRequest, IReadOnlyList<NativeCollateralHistory.GetCollateralHistory.Item>>> GetCollateralHistoryAsync(
            NativeCollateralHistory.GetCollateralHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            _ = request;
            _ = cancellationToken;
            return Task.FromResult(CollateralHistoryCall ?? throw new InvalidOperationException("CollateralHistoryCall must be configured."));
        }

        public Task<CallResult<NativeChildOrders.GetChildOrdersRequest, IReadOnlyList<NativeChildOrders.GetChildOrders.Item>>> GetChildOrdersAsync(
            NativeChildOrders.GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            LastChildOrdersRequest = request;
            return Task.FromResult(ChildOrdersCall ?? throw new InvalidOperationException("ChildOrdersCall must be configured."));
        }
    }
}
