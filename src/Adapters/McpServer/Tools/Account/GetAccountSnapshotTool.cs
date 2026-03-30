using System.Globalization;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Account;

public sealed class GetAccountSnapshotTool
{
    private const string SpotProductCode = "BTC_JPY";
    private const string FxProductCode = "FX_BTC_JPY";

    private readonly IBitflyerAccountSnapshotGateway _gateway;

    public GetAccountSnapshotTool(IBitflyerAccountSnapshotGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<McpToolExecutionResult<GetAccountSnapshotResponse>> ExecuteAsync(
        GetAccountSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = request;

        var balanceTask = _gateway.GetBalanceCallAsync(cancellationToken);
        var collateralTask = _gateway.GetCollateralCallAsync(cancellationToken);
        var spotOrdersTask = _gateway.GetActiveChildOrdersCallAsync(SpotProductCode, cancellationToken);
        var fxOrdersTask = _gateway.GetActiveChildOrdersCallAsync(FxProductCode, cancellationToken);
        var positionsTask = _gateway.GetPositionsCallAsync(FxProductCode, cancellationToken);
        var permissionsTask = _gateway.GetPermissionsCallAsync(cancellationToken);

        await Task.WhenAll(balanceTask, collateralTask, spotOrdersTask, fxOrdersTask, positionsTask, permissionsTask);

        var balanceCall = await balanceTask;
        if (!balanceCall.IsSuccess || balanceCall.Response is null)
        {
            return McpToolExecutionResult<GetAccountSnapshotResponse>.Failure(
                UpstreamError("GetBalance", balanceCall.Error));
        }

        var collateralCall = await collateralTask;
        if (!collateralCall.IsSuccess || collateralCall.Response is null)
        {
            return McpToolExecutionResult<GetAccountSnapshotResponse>.Failure(
                UpstreamError("GetCollateral", collateralCall.Error));
        }

        var spotOrdersCall = await spotOrdersTask;
        if (!spotOrdersCall.IsSuccess || spotOrdersCall.Response is null)
        {
            return McpToolExecutionResult<GetAccountSnapshotResponse>.Failure(
                UpstreamError("GetChildOrders", spotOrdersCall.Error, SpotProductCode));
        }

        var fxOrdersCall = await fxOrdersTask;
        if (!fxOrdersCall.IsSuccess || fxOrdersCall.Response is null)
        {
            return McpToolExecutionResult<GetAccountSnapshotResponse>.Failure(
                UpstreamError("GetChildOrders", fxOrdersCall.Error, FxProductCode));
        }

        var positionsCall = await positionsTask;
        if (!positionsCall.IsSuccess || positionsCall.Response is null)
        {
            return McpToolExecutionResult<GetAccountSnapshotResponse>.Failure(
                UpstreamError("GetPositions", positionsCall.Error, FxProductCode));
        }

        var permissionsCall = await permissionsTask;
        var accountReadiness = permissionsCall.IsSuccess && permissionsCall.Response is not null
            ? BitflyerAccountReadinessMapper.Map(permissionsCall.Response)
            : "unknown";

        var response = new GetAccountSnapshotResponse
        {
            PermissionModel = PermissionModelIds.BitflyerPrivateReadV1,
            Balance = MapBalance(balanceCall.Response),
            Positions = MapPositions(positionsCall.Response),
            OpenOrdersSummary = new OpenOrdersSummary
            {
                Count = spotOrdersCall.Response.Count + fxOrdersCall.Response.Count,
            },
            Margin = new AccountMarginSnapshot
            {
                DerivedAvailable = FormatDecimal(
                    collateralCall.Response.Collateral
                    + collateralCall.Response.OpenPositionPnl
                    - collateralCall.Response.RequireCollateral),
            },
            AccountReadiness = accountReadiness,
        };

        return McpToolExecutionResult<GetAccountSnapshotResponse>.Success(response);
    }

    private static IReadOnlyDictionary<string, string> MapBalance(IReadOnlyList<ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance.GetBalance.Item> items)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            result[item.CurrencyCode] = FormatDecimal(item.Available);
        }

        return result;
    }

    private static IReadOnlyList<AccountPositionSnapshot> MapPositions(IReadOnlyList<ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions.GetPositions.Item> items)
    {
        return items
            .Select(item => new AccountPositionSnapshot
            {
                Symbol = item.ProductCode,
                Side = MapSide(item.Side),
                Size = FormatDecimal(item.Size),
                AvgPrice = FormatDecimal(item.Price),
            })
            .ToArray();
    }

    private static string MapSide(BitflyerOrderSide side)
    {
        return side switch
        {
            OrderSides.Buy => "buy",
            OrderSides.Sell => "sell",
            _ => throw new InvalidOperationException($"Unsupported bitFlyer order side: {side}."),
        };
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static McpToolError UpstreamError(
        string endpoint,
        CallError? error,
        string? productCode = null)
    {
        var details = new Dictionary<string, string?>
        {
            ["endpoint"] = endpoint,
            ["callErrorKind"] = error?.Kind,
            ["callErrorMessage"] = error?.Message,
        };

        if (productCode is not null)
        {
            details["productCode"] = productCode;
        }

        return new McpToolError
        {
            ErrorCategory = "upstream_error",
            ErrorCode = "account_unavailable",
            Message = "Failed to load account snapshot from upstream.",
            Details = details,
            Retryable = true,
        };
    }
}
