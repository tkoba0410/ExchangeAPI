using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Tools.Account;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class GetAccountSnapshotToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsAccountSnapshotUsingBitflyerV1Derivation()
    {
        var tool = new GetAccountSnapshotTool(new FakeBitflyerAccountSnapshotGateway
        {
            BalanceCall = CallFactory.Success(
                new GetBalanceRequest(),
                (IReadOnlyList<GetBalance.Item>)
                [
                    new GetBalance.Item { CurrencyCode = "JPY", Amount = 5000000m, Available = 4800000m },
                    new GetBalance.Item { CurrencyCode = "BTC", Amount = 0.5m, Available = 0.4m },
                ],
                TestCallMeta("GetBalance")),
            CollateralCall = CallFactory.Success(
                new GetCollateralRequest(),
                new GetCollateralResponse
                {
                    Collateral = 5000000m,
                    OpenPositionPnl = 100000m,
                    RequireCollateral = 600000m,
                    KeepRate = 8m,
                },
                TestCallMeta("GetCollateral")),
            SpotOrdersCall = SuccessOrders("BTC_JPY", 2),
            FxOrdersCall = SuccessOrders("FX_BTC_JPY", 1),
            PositionsCall = CallFactory.Success(
                new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
                (IReadOnlyList<GetPositions.Item>)
                [
                    new GetPositions.Item
                    {
                        ProductCode = "FX_BTC_JPY",
                        Side = OrderSides.Buy,
                        Price = 12000000m,
                        Size = 0.1m,
                        Commission = 0m,
                        SwapPointAccumulate = 0m,
                        RequireCollateral = 100000m,
                        OpenDate = new DateTimeOffset(2026, 03, 29, 10, 00, 00, TimeSpan.Zero),
                        Leverage = 2m,
                        Pnl = 10000m,
                        Sfd = 0m,
                    },
                ],
                TestCallMeta("GetPositions")),
            PermissionsCall = CallFactory.Success(
                new GetPermissionsRequest(),
                (IReadOnlyList<string>)BitflyerAccountReadinessMapper.RequiredPermissions.ToArray(),
                TestCallMeta("GetPermissions")),
        });

        var result = await tool.ExecuteAsync(new GetAccountSnapshotRequest());

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        var response = Assert.IsType<GetAccountSnapshotResponse>(result.Response);
        Assert.Equal("4800000", response.Balance["JPY"]);
        Assert.Equal("0.4", response.Balance["BTC"]);
        var position = Assert.Single(response.Positions);
        Assert.Equal("FX_BTC_JPY", position.Symbol);
        Assert.Equal("buy", position.Side);
        Assert.Equal("0.1", position.Size);
        Assert.Equal("12000000", position.AvgPrice);
        Assert.Equal(3, response.OpenOrdersSummary.Count);
        Assert.Equal("4500000", response.Margin.DerivedAvailable);
        Assert.Equal("ready", response.AccountReadiness);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsRestrictedWhenRequiredPermissionIsMissing()
    {
        var tool = new GetAccountSnapshotTool(
            CreateHappyPathGateway(
                permissions:
                [
                    "/v1/me/getpermissions",
                    "/v1/me/getbalance",
                    "/v1/me/getcollateral",
                    "/v1/me/getchildorders",
                ]));

        var result = await tool.ExecuteAsync(new GetAccountSnapshotRequest());

        Assert.True(result.IsSuccess);
        Assert.Equal("restricted", result.Response!.AccountReadiness);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnknownWhenPermissionsCannotBeLoaded()
    {
        var tool = new GetAccountSnapshotTool(
            CreateHappyPathGateway(
                permissionsCall: CallFactory.Failure<GetPermissionsRequest, IReadOnlyList<string>>(
                    new GetPermissionsRequest(),
                    new CallError
                    {
                        Kind = CallErrorKinds.Transport,
                        Message = "permission endpoint unavailable",
                    },
                    TestCallMeta("GetPermissions"))));

        var result = await tool.ExecuteAsync(new GetAccountSnapshotRequest());

        Assert.True(result.IsSuccess);
        Assert.Equal("unknown", result.Response!.AccountReadiness);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUpstreamErrorWhenBalanceFails()
    {
        var tool = new GetAccountSnapshotTool(
            CreateHappyPathGateway(
                balanceCall: CallFactory.Failure<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(
                    new GetBalanceRequest(),
                    new CallError
                    {
                        Kind = CallErrorKinds.Http,
                        Message = "500",
                    },
                    TestCallMeta("GetBalance"))));

        var result = await tool.ExecuteAsync(new GetAccountSnapshotRequest());

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("upstream_error", error.ErrorCategory);
        Assert.Equal("account_unavailable", error.ErrorCode);
        Assert.Equal("GetBalance", error.Details["endpoint"]);
        Assert.Equal("Http", error.Details["callErrorKind"]);
    }

    [Theory]
    [InlineData(new[] { "/v1/me/getpermissions", "/v1/me/getbalance", "/v1/me/getcollateral", "/v1/me/getchildorders", "/v1/me/getpositions" }, "ready")]
    [InlineData(new[] { "/v1/me/getpermissions" }, "restricted")]
    public void BitflyerAccountReadinessMapper_MapsPermissions(string[] permissions, string expected)
    {
        var actual = BitflyerAccountReadinessMapper.Map(permissions);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BitflyerAccountReadinessMapper_ReturnsUnknownWhenPermissionsAreMissing()
    {
        var actual = BitflyerAccountReadinessMapper.Map(null);

        Assert.Equal("unknown", actual);
    }

    private static FakeBitflyerAccountSnapshotGateway CreateHappyPathGateway(
        Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>? balanceCall = null,
        Call<GetPermissionsRequest, IReadOnlyList<string>>? permissionsCall = null,
        IReadOnlyList<string>? permissions = null)
    {
        var grantedPermissions = permissions ?? BitflyerAccountReadinessMapper.RequiredPermissions.ToArray();

        return new FakeBitflyerAccountSnapshotGateway
        {
            BalanceCall = balanceCall ?? CallFactory.Success(
                new GetBalanceRequest(),
                (IReadOnlyList<GetBalance.Item>)[new GetBalance.Item { CurrencyCode = "JPY", Amount = 1m, Available = 1m }],
                TestCallMeta("GetBalance")),
            CollateralCall = CallFactory.Success(
                new GetCollateralRequest(),
                new GetCollateralResponse
                {
                    Collateral = 1m,
                    OpenPositionPnl = 0m,
                    RequireCollateral = 0m,
                    KeepRate = 1m,
                },
                TestCallMeta("GetCollateral")),
            SpotOrdersCall = SuccessOrders("BTC_JPY", 0),
            FxOrdersCall = SuccessOrders("FX_BTC_JPY", 0),
            PositionsCall = CallFactory.Success(
                new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
                (IReadOnlyList<GetPositions.Item>)Array.Empty<GetPositions.Item>(),
                TestCallMeta("GetPositions")),
            PermissionsCall = permissionsCall ?? CallFactory.Success(
                new GetPermissionsRequest(),
                grantedPermissions,
                TestCallMeta("GetPermissions")),
        };
    }

    private static Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> SuccessOrders(string productCode, int count)
    {
        var items = Enumerable.Range(0, count)
            .Select(index => new GetChildOrders.Item
            {
                Id = index + 1,
                ChildOrderId = $"CO-{productCode}-{index + 1}",
                ProductCode = productCode,
                Side = OrderSides.Buy,
                ChildOrderType = ChildOrderTypes.Limit,
                Price = 100m,
                AveragePrice = 0m,
                Size = 0.01m,
                ChildOrderState = ChildOrderStates.Active,
                ExpireDate = new DateTimeOffset(2026, 03, 29, 12, 00, 00, TimeSpan.Zero),
                ChildOrderDate = new DateTimeOffset(2026, 03, 29, 10, 00, 00, TimeSpan.Zero),
                ChildOrderAcceptanceId = $"ACCEPT-{productCode}-{index + 1}",
                OutstandingSize = 0.01m,
                CancelSize = 0m,
                ExecutedSize = 0m,
                TotalCommission = 0m,
                TimeInForce = TimeInForces.Gtc,
            })
            .ToArray();

        return CallFactory.Success(
            new GetChildOrdersRequest
            {
                ProductCode = productCode,
                ChildOrderState = ChildOrderStates.Active,
            },
            (IReadOnlyList<GetChildOrders.Item>)items,
            TestCallMeta("GetChildOrders"));
    }

    private static CallMeta TestCallMeta(string endpointId)
    {
        return new CallMeta
        {
            Layer = CallLayers.Tests,
            Component = CallComponents.Factory,
            EndpointId = endpointId,
            Scope = "Private",
            Auth = "ApiKey",
            Children = null,
        };
    }

    private sealed class FakeBitflyerAccountSnapshotGateway : IBitflyerAccountSnapshotGateway
    {
        public required Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> BalanceCall { get; init; }

        public required Call<GetCollateralRequest, GetCollateralResponse> CollateralCall { get; init; }

        public required Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> SpotOrdersCall { get; init; }

        public required Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> FxOrdersCall { get; init; }

        public required Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> PositionsCall { get; init; }

        public required Call<GetPermissionsRequest, IReadOnlyList<string>> PermissionsCall { get; init; }

        public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(BalanceCall);
        }

        public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(CollateralCall);
        }

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersCallAsync(
            string productCode,
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(productCode == "BTC_JPY" ? SpotOrdersCall : FxOrdersCall);
        }

        public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
            string productCode,
            CancellationToken cancellationToken = default)
        {
            _ = productCode;
            _ = cancellationToken;
            return Task.FromResult(PositionsCall);
        }

        public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(PermissionsCall);
        }
    }
}
