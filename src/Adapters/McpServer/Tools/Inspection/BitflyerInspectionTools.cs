using System.Globalization;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using SchemaInspection = ExchangeApi.Adapters.McpServer.Schema.Inspection;
using NativeBalanceHistory = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using NativeChildOrders = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using NativeCollateralAccounts = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using NativeCollateralHistory = ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;

namespace ExchangeApi.Adapters.McpServer.Tools.Inspection;

public sealed class BitflyerInspectionTools
{
    private readonly IBitflyerInspectionGateway _gateway;

    public BitflyerInspectionTools(IBitflyerInspectionGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<McpToolExecutionResult<SchemaInspection.GetCollateralAccountsResponse>> GetCollateralAccountsAsync(
        SchemaInspection.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidateContext(request, out var validationError))
        {
            return McpToolExecutionResult<SchemaInspection.GetCollateralAccountsResponse>.Failure(validationError!);
        }

        var call = await _gateway.GetCollateralAccountsAsync(cancellationToken);
        if (!call.IsSuccess || call.Response is null)
        {
            return McpToolExecutionResult<SchemaInspection.GetCollateralAccountsResponse>.Failure(UpstreamError("GetCollateralAccounts", call.Error));
        }

        return McpToolExecutionResult<SchemaInspection.GetCollateralAccountsResponse>.Success(
            new SchemaInspection.GetCollateralAccountsResponse
            {
                Accounts = call.Response
                    .Select(item => new SchemaInspection.CollateralAccountItem
                    {
                        CurrencyCode = item.CurrencyCode,
                        Amount = FormatDecimal(item.Amount),
                    })
                    .ToArray(),
            });
    }

    public async Task<McpToolExecutionResult<SchemaInspection.GetBalanceHistoryResponse>> GetBalanceHistoryAsync(
        SchemaInspection.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidateContext(request, out var validationError))
        {
            return McpToolExecutionResult<SchemaInspection.GetBalanceHistoryResponse>.Failure(validationError!);
        }

        var nativeRequest = new NativeBalanceHistory.GetBalanceHistoryRequest
        {
            CurrencyCode = request.CurrencyCode,
            Count = request.Count,
            Before = request.Before,
            After = request.After,
        };
        var call = await _gateway.GetBalanceHistoryAsync(nativeRequest, cancellationToken);
        if (!call.IsSuccess || call.Response is null)
        {
            return McpToolExecutionResult<SchemaInspection.GetBalanceHistoryResponse>.Failure(UpstreamError("GetBalanceHistory", call.Error));
        }

        return McpToolExecutionResult<SchemaInspection.GetBalanceHistoryResponse>.Success(
            new SchemaInspection.GetBalanceHistoryResponse
            {
                Items = call.Response
                    .Select(item => new SchemaInspection.BalanceHistoryItem
                    {
                        Id = item.Id,
                        TradeDate = item.TradeDate,
                        EventDate = item.EventDate,
                        ProductCode = item.ProductCode,
                        CurrencyCode = item.CurrencyCode,
                        TradeType = ApiStringEnum<BitflyerTradeType>.Format(item.TradeType),
                        Price = FormatDecimal(item.Price),
                        Amount = FormatDecimal(item.Amount),
                        Quantity = FormatDecimal(item.Quantity),
                        Commission = FormatDecimal(item.Commission),
                        Balance = FormatDecimal(item.Balance),
                        OrderId = item.OrderId,
                    })
                    .ToArray(),
            });
    }

    public async Task<McpToolExecutionResult<SchemaInspection.GetCollateralHistoryResponse>> GetCollateralHistoryAsync(
        SchemaInspection.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidateContext(request, out var validationError))
        {
            return McpToolExecutionResult<SchemaInspection.GetCollateralHistoryResponse>.Failure(validationError!);
        }

        var nativeRequest = new NativeCollateralHistory.GetCollateralHistoryRequest
        {
            Count = request.Count,
            Before = request.Before,
            After = request.After,
        };
        var call = await _gateway.GetCollateralHistoryAsync(nativeRequest, cancellationToken);
        if (!call.IsSuccess || call.Response is null)
        {
            return McpToolExecutionResult<SchemaInspection.GetCollateralHistoryResponse>.Failure(UpstreamError("GetCollateralHistory", call.Error));
        }

        return McpToolExecutionResult<SchemaInspection.GetCollateralHistoryResponse>.Success(
            new SchemaInspection.GetCollateralHistoryResponse
            {
                Items = call.Response
                    .Select(item => new SchemaInspection.CollateralHistoryItem
                    {
                        Id = item.Id,
                        CurrencyCode = item.CurrencyCode,
                        Change = FormatDecimal(item.Change),
                        Amount = FormatDecimal(item.Amount),
                        ReasonCode = item.ReasonCode,
                        Date = item.Date,
                    })
                    .ToArray(),
            });
    }

    public async Task<McpToolExecutionResult<SchemaInspection.GetChildOrdersResponse>> GetChildOrdersAsync(
        SchemaInspection.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidateContext(request, out var validationError))
        {
            return McpToolExecutionResult<SchemaInspection.GetChildOrdersResponse>.Failure(validationError!);
        }

        if (!TryParseOrderState(request.ChildOrderState, out var orderState, out validationError))
        {
            return McpToolExecutionResult<SchemaInspection.GetChildOrdersResponse>.Failure(validationError!);
        }

        var nativeRequest = new NativeChildOrders.GetChildOrdersRequest
        {
            ProductCode = request.ProductCode,
            Count = request.Count,
            Before = request.Before,
            After = request.After,
            ChildOrderState = orderState,
            ChildOrderId = request.ChildOrderId,
            ChildOrderAcceptanceId = request.ChildOrderAcceptanceId,
            ParentOrderId = request.ParentOrderId,
        };
        var call = await _gateway.GetChildOrdersAsync(nativeRequest, cancellationToken);
        if (!call.IsSuccess || call.Response is null)
        {
            return McpToolExecutionResult<SchemaInspection.GetChildOrdersResponse>.Failure(UpstreamError("GetChildOrders", call.Error));
        }

        return McpToolExecutionResult<SchemaInspection.GetChildOrdersResponse>.Success(
            new SchemaInspection.GetChildOrdersResponse
            {
                Orders = call.Response
                    .Select(item => new SchemaInspection.ChildOrderItem
                    {
                        Id = item.Id,
                        ChildOrderId = item.ChildOrderId,
                        ProductCode = item.ProductCode,
                        Side = ApiStringEnum<BitflyerOrderSide>.Format(item.Side),
                        ChildOrderType = ApiStringEnum<BitflyerChildOrderType>.Format(item.ChildOrderType),
                        Price = FormatDecimal(item.Price),
                        AveragePrice = FormatDecimal(item.AveragePrice),
                        Size = FormatDecimal(item.Size),
                        ChildOrderState = ApiStringEnum<BitflyerOrderState>.Format(item.ChildOrderState),
                        ExpireDate = item.ExpireDate,
                        ChildOrderDate = item.ChildOrderDate,
                        ChildOrderAcceptanceId = item.ChildOrderAcceptanceId,
                        OutstandingSize = FormatDecimal(item.OutstandingSize),
                        CancelSize = FormatDecimal(item.CancelSize),
                        ExecutedSize = FormatDecimal(item.ExecutedSize),
                        TotalCommission = FormatDecimal(item.TotalCommission),
                        TimeInForce = ApiStringEnum<BitflyerTimeInForce>.Format(item.TimeInForce),
                    })
                    .ToArray(),
            });
    }

    private static bool TryValidateContext(SchemaInspection.BitflyerPrivateReadRequestBase request, out McpToolError? error)
    {
        return BitflyerPrivateContextValidator.TryNormalize(
            request.Venue,
            request.AccountContext,
            out _,
            out _,
            out error);
    }

    private static bool TryParseOrderState(string? value, out BitflyerOrderState? orderState, out McpToolError? error)
    {
        orderState = null;
        error = null;

        if (value is null)
        {
            return true;
        }

        if (ApiStringEnum<BitflyerOrderState>.TryParse(value, out var parsed))
        {
            orderState = parsed;
            return true;
        }

        error = new McpToolError
        {
            ErrorCategory = "validation_error",
            ErrorCode = "invalid_request",
            Message = "childOrderState must be ACTIVE, COMPLETED, CANCELED, EXPIRED, or REJECTED.",
            Details = new Dictionary<string, string?> { ["field"] = "childOrderState" },
            Retryable = false,
        };
        return false;
    }

    private static McpToolError UpstreamError(string endpoint, CallError? error)
    {
        return new McpToolError
        {
            ErrorCategory = "upstream_error",
            ErrorCode = "inspection_read_unavailable",
            Message = "Failed to load bitFlyer inspection data from upstream.",
            Details = new Dictionary<string, string?>
            {
                ["endpoint"] = endpoint,
                ["callErrorKind"] = error?.Kind,
                ["callErrorMessage"] = error?.Message,
            },
            Retryable = true,
        };
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
