using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

internal static class BitflyerEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static WireRequest GetTicker(RawProductCode productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerRawConstants.Paths.Ticker : BitflyerRawConstants.Paths.GetTicker,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBoard(RawProductCode productCode, bool useAliasPath) =>
        Get(useAliasPath ? BitflyerRawConstants.Paths.Board : BitflyerRawConstants.Paths.GetBoard,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetExecutions(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerRawConstants.Paths.Executions : BitflyerRawConstants.Paths.GetExecutions;
        return Get(path, BuildQuery(
            (BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerRawConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerRawConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerRawConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetMarkets(string? region = null, bool useAliasPath = false)
    {
        var path = useAliasPath ? BitflyerRawConstants.Paths.Markets : BitflyerRawConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, query: null);
    }

    public static WireRequest GetChats(string? fromDate = null, string? region = null)
    {
        var path = BitflyerRawConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return Get(path, BuildQuery((BitflyerRawConstants.QueryKeys.FromDate, fromDate)));
    }

    public static WireRequest GetHealth(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetHealth,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBoardState(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetBoardState,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetCorporateLeverage() =>
        Get(BitflyerRawConstants.Paths.GetCorporateLeverage, query: null);

    public static WireRequest GetFundingRate(RawProductCode productCode) =>
        Get(BitflyerRawConstants.Paths.GetFundingRate,
            BuildQuery((BitflyerRawConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetBalances() =>
        Get(BitflyerConstants.Paths.GetBalance, query: null);

    public static WireRequest GetExecutions(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetPrivateExecutions, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetPositions(RawProductCode productCode) =>
        Get(BitflyerConstants.Paths.GetPositions,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetCollateral() =>
        Get(BitflyerConstants.Paths.GetCollateral, query: null);

    public static WireRequest GetChildOrders(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetChildOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.ChildOrderStatusState, childOrderStatusState),
            (BitflyerConstants.QueryKeys.ChildOrderAcceptanceId, childOrderAcceptanceId),
            (BitflyerConstants.QueryKeys.ChildOrderId, childOrderId),
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetTradingCommission(RawProductCode productCode) =>
        Get(BitflyerConstants.Paths.GetTradingCommission,
            BuildQuery((BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode))));

    public static WireRequest GetPermissions() =>
        Get(BitflyerConstants.Paths.GetPermissions, query: null);

    public static WireRequest GetCollateralAccounts() =>
        Get(BitflyerConstants.Paths.GetCollateralAccounts, query: null);

    public static WireRequest GetParentOrders(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null)
    {
        return Get(BitflyerConstants.Paths.GetParentOrders, BuildQuery(
            (BitflyerConstants.QueryKeys.ProductCode, EnsureProductCode(productCode)),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString()),
            (BitflyerConstants.QueryKeys.ParentOrderStatusState, parentOrderStatusState)));
    }

    public static WireRequest GetParentOrder(string? parentOrderId = null, string? parentOrderAcceptanceId = null)
    {
        if (string.IsNullOrWhiteSpace(parentOrderId) && string.IsNullOrWhiteSpace(parentOrderAcceptanceId))
        {
            throw new ArgumentException("parentOrderId or parentOrderAcceptanceId is required.");
        }

        return Get(BitflyerConstants.Paths.GetParentOrder, BuildQuery(
            (BitflyerConstants.QueryKeys.ParentOrderId, parentOrderId),
            (BitflyerConstants.QueryKeys.ParentOrderAcceptanceId, parentOrderAcceptanceId)));
    }

    public static WireRequest GetBalanceHistory(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetBalanceHistory, BuildQuery(
            (BitflyerConstants.QueryKeys.CurrencyCode, currencyCode),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetCollateralHistory(
        int? count = null,
        long? before = null,
        long? after = null)
    {
        return Get(BitflyerConstants.Paths.GetCollateralHistory, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));
    }

    public static WireRequest GetAddresses() =>
        Get(BitflyerConstants.Paths.GetAddresses, query: null);

    public static WireRequest GetCoinIns(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinIns, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetCoinOuts(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetCoinOuts, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetDeposits(int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetDeposits, BuildQuery(
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetWithdrawals(string? messageId = null, int? count = null, long? before = null, long? after = null) =>
        Get(BitflyerConstants.Paths.GetWithdrawals, BuildQuery(
            (BitflyerConstants.QueryKeys.MessageId, messageId),
            (BitflyerConstants.QueryKeys.Count, count?.ToString()),
            (BitflyerConstants.QueryKeys.Before, before?.ToString()),
            (BitflyerConstants.QueryKeys.After, after?.ToString())));

    public static WireRequest GetBankAccounts() =>
        Get(BitflyerConstants.Paths.GetBankAccounts, query: null);

    public static WireRequest CreateChildOrder(CreateChildOrderRequest request) =>
        Post(BitflyerConstants.Paths.SendChildOrder, SerializeBody(MapSendChildOrderRequest(request)));

    public static WireRequest CreateChildOrder(RawSendChildOrderRequest request) =>
        Post(BitflyerConstants.Paths.SendChildOrder, SerializeBody(request));

    public static WireRequest CancelChildOrder(CancelChildOrderRequest request) =>
        Post(BitflyerConstants.Paths.CancelChildOrder, SerializeBody(MapCancelChildOrderRequest(request)));

    public static WireRequest CancelChildOrder(RawCancelChildOrderRequest request) =>
        Post(BitflyerConstants.Paths.CancelChildOrder, SerializeBody(request));

    public static WireRequest CancelAllChildOrders(CancelAllChildOrdersRequest request) =>
        Post(BitflyerConstants.Paths.CancelAllChildOrders, SerializeBody(request));

    public static WireRequest CreateParentOrder(CreateParentOrderRequest request) =>
        Post(BitflyerConstants.Paths.SendParentOrder, SerializeBody(request));

    public static WireRequest CancelParentOrder(CancelParentOrderRequest request) =>
        Post(BitflyerConstants.Paths.CancelParentOrder, SerializeBody(request));

    public static WireRequest CreateWithdrawal(CreateWithdrawalRequest request) =>
        Post(BitflyerConstants.Paths.Withdraw, SerializeBody(request));

    private static WireRequest Get(string path, string? query) =>
        new(Method: "GET", Path: path, Query: query);

    private static WireRequest Post(string path, string? bodyJson) =>
        new(Method: "POST", Path: path, Query: null, BodyJson: bodyJson);

    private static string? SerializeBody<T>(T body) =>
        body is null ? null : JsonSerializer.Serialize(body, JsonOptions);

    private static string? BuildQuery(params (string Key, string? Value)[] entries)
    {
        var parts = new List<string>();
        foreach (var (key, value) in entries)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }

    private static string EnsureProductCode(RawProductCode productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return productCode.Value;
    }

    private static RawSendChildOrderRequest MapSendChildOrderRequest(CreateChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderType = MapChildOrderType(request.ChildOrderType),
        Side = MapSide(request.Side),
        Size = request.Size,
        Price = request.Price,
        MinuteToExpire = request.MinuteToExpire,
        TimeInForce = request.TimeInForce is null ? null : MapTimeInForce(request.TimeInForce.Value),
        TriggerPrice = request.TriggerPrice,
    };

    private static RawCancelChildOrderRequest MapCancelChildOrderRequest(CancelChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderId = request.ChildOrderId,
        ChildOrderAcceptanceId = request.ChildOrderAcceptanceId,
    };

    private static string MapProductCode(RawProductCode productCode) =>
        string.IsNullOrWhiteSpace(productCode.Value)
            ? throw new ArgumentOutOfRangeException(nameof(productCode), productCode, "Unsupported product_code.")
            : productCode.Value;

    private static string MapChildOrderType(ChildOrderType childOrderType) =>
        childOrderType switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unsupported child_order_type."),
        };

    private static string MapSide(Side side) =>
        side switch
        {
            Side.Buy => "BUY",
            Side.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unsupported side."),
        };

    private static string MapTimeInForce(TimeInForce timeInForce) =>
        timeInForce switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => throw new ArgumentOutOfRangeException(nameof(timeInForce), timeInForce, "Unsupported time_in_force."),
        };
}
