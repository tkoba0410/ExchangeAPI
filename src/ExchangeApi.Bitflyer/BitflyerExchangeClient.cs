using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer 用の IExchangeClient 実装。
/// Stage1 では Public GET（ticker）、Stage2 では Private GET（getbalance）をサポートする。
/// </summary>
public sealed class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateAccountApi;
    private readonly IBitflyerPrivateTradingApi _privateTradingApi;
    private readonly string _exchangeId;
    private readonly string _accountId;

    /// <summary>
    /// 取引所 ID（例: "bitFlyer"）。
    /// </summary>
    public string ExchangeId => _exchangeId;

    /// <summary>
    /// アカウント ID。Stage1/2 時点では固定 "default"。
    /// </summary>
    public string AccountId => _accountId;

    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateAccountApi,
        IBitflyerPrivateTradingApi privateTradingApi)
        : this(publicApi, privateAccountApi, privateTradingApi, exchangeId: "bitFlyer", accountId: "default")
    {
    }

    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateAccountApi,
        IBitflyerPrivateTradingApi privateTradingApi,
        string exchangeId,
        string accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateAccountApi = privateAccountApi ?? throw new ArgumentNullException(nameof(privateAccountApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        _exchangeId = exchangeId ?? throw new ArgumentNullException(nameof(exchangeId));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    #region IExchangeMarketClient (ticker)

    /// <summary>
    /// 指定されたシンボルのティッカーを取得する。
    /// </summary>
    public async Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // シンボル→bitFlyer product_code 変換
            var productCode = MapSymbolToProductCode(symbol);

            var rawTicker = await _publicApi
                .GetTickerRawAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            return MapToTicker(symbol, rawTicker);
        }
        catch (SymbolNotSupportedException ex)
        {
            // ドメインエラー：サポートしていないシンボル
            throw new ExchangeApiException(
                message: $"Symbol '{ex.Symbol}' is not supported by bitFlyer.",
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException)
        {
            // RestClient / Transport 層からの E1 例外はそのまま伝播
            throw;
        }
        catch (Exception ex)
        {
            // 予期しない例外は Adapter 層で文脈付き ExchangeApiException に統一
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getticker API.",
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
    }

    #endregion

    #region IExchangeAccountClient (balances)

    /// <summary>
    /// 口座残高一覧を取得する。
    /// Stage2 では /v1/me/getbalance のみを対象とする。
    /// </summary>
    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rawBalances = await _privateAccountApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = rawBalances
                .Select(b => new Balance(
                    b.CurrencyCode,
                    b.Amount,
                    b.Available))
                .ToArray();

            return result;
        }
        catch (ExchangeApiException)
        {
            // RestClient / Transport 層からの E1 例外はそのまま
            throw;
        }
        catch (Exception ex)
        {
            // 予期しない例外は文脈付きにラップ
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchangeId: _exchangeId,
                operation: "GetBalances",
                statusCode: null,
                innerException: ex);
        }
    }

    #endregion

    #region IExchangeTradingClient (send order)

    public async Task<OrderResult> PlaceOrderAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        ValidateOrderRequest(request);

        try
        {
            var dto = new BitflyerSendChildOrderRequest
            {
                ProductCode = request.ProductCode,
                Side = request.Side == OrderSide.Buy ? "BUY" : "SELL",
                ChildOrderType = MapOrderType(request.OrderType, request.Price),
                Size = request.Size,
                Price = request.Price,
                TriggerPrice = request.TriggerPrice,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = MapTimeInForce(request.TimeInForce),
            };

            var response = await _privateTradingApi
                .SendChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            return new OrderResult(response.ChildOrderAcceptanceId);
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "SendOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer sendchildorder API.",
                exchangeId: _exchangeId,
                operation: "SendOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(
        string productCode,
        string childOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        if (string.IsNullOrWhiteSpace(childOrderAcceptanceId))
        {
            throw new ArgumentException("childOrderAcceptanceId is required.", nameof(childOrderAcceptanceId));
        }

        try
        {
            var dto = new BitflyerCancelChildOrderRequest
            {
                ProductCode = productCode,
                ChildOrderAcceptanceId = childOrderAcceptanceId,
            };

            _ = await _privateTradingApi
                .CancelChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "CancelOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelchildorder API.",
                exchangeId: _exchangeId,
                operation: "CancelOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelAllOrdersAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        try
        {
            var dto = new BitflyerCancelAllChildOrdersRequest
            {
                ProductCode = productCode,
            };

            _ = await _privateTradingApi
                .CancelAllChildOrdersAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "CancelAllOrders");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelallchildorders API.",
                exchangeId: _exchangeId,
                operation: "CancelAllOrders",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _privateAccountApi
                .GetPositionsAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            var mapped = raw
                .Select(p => new Position(
                    ProductCode: p.ProductCode,
                    Side: MapSide(p.Side),
                    Size: p.Size,
                    Price: p.Price,
                    OpenDate: p.OpenDate,
                    Pnl: p.Pnl))
                .ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "GetPositions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getpositions API.",
                exchangeId: _exchangeId,
                operation: "GetPositions",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Execution>> GetExecutionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _privateAccountApi
                .GetExecutionsAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            var mapped = raw
                .Select(e => new Execution(
                    ProductCode: e.ProductCode,
                    Id: e.Id,
                    Side: MapSide(e.Side),
                    Price: e.Price,
                    Size: e.Size,
                    ExecutedAt: e.ExecDate,
                    ChildOrderAcceptanceId: e.ChildOrderAcceptanceId))
                .ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "GetExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchangeId: _exchangeId,
                operation: "GetExecutions",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<Collateral> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _privateAccountApi
                .GetCollateralAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Collateral(
                Amount: raw.Collateral,
                OpenPositionPnl: raw.OpenPositionPnl,
                RequireCollateral: raw.RequireCollateral,
                KeepRate: raw.KeepRate);
        }
        catch (ExchangeApiException ex)
        {
            throw EnrichBitflyerException(ex, "GetCollateral");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getcollateral API.",
                exchangeId: _exchangeId,
                operation: "GetCollateral",
                statusCode: null,
                innerException: ex);
        }
    }

    #endregion

    #region private mapping helpers

    /// <summary>
    /// 抽象シンボル（例: BTC/JPY）を bitFlyer の product_code にマップする。
    /// Stage1/2 時点では BTC/JPY のみサポート。
    /// </summary>
    private static string MapSymbolToProductCode(string symbol)
    {
        // Stage1/2 ではシンプルな静的マッピングのみ
        if (string.Equals(symbol, Symbols.BtcJpy, StringComparison.Ordinal))
        {
            return "BTC_JPY";
        }

        // Stage1/2 では BTC/JPY 以外はサポートしない
        throw new SymbolNotSupportedException(symbol);
    }

    private static string MapOrderType(OrderType orderType, decimal? price)
    {
        return orderType switch
        {
            OrderType.Market => "MARKET",
            OrderType.Limit => "LIMIT",
            OrderType.Stop => price is null ? "STOP" : "STOP_LIMIT",
            _ => "MARKET",
        };
    }

    private static string? MapTimeInForce(TimeInForce? tif)
    {
        return tif switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => null,
        };
    }

    private static OrderSide MapSide(string side)
    {
        return string.Equals(side, "BUY", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;
    }

    private static void ValidateOrderRequest(OrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            throw new ArgumentException("ProductCode is required.", nameof(request));
        }

        if (request.Size <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.", nameof(request));
        }

        if (request.MinuteToExpire is { } mte && mte <= 0)
        {
            throw new ArgumentException("MinuteToExpire must be positive when specified.", nameof(request));
        }

        if (request.Price is { } price && price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero when specified.", nameof(request));
        }

        if (request.TriggerPrice is { } tp && tp <= 0)
        {
            throw new ArgumentException("TriggerPrice must be greater than zero when specified.", nameof(request));
        }

        switch (request.OrderType)
        {
            case OrderType.Market:
                if (request.Price is not null || request.TriggerPrice is not null)
                {
                    throw new ArgumentException("Market order must not specify Price or TriggerPrice.", nameof(request));
                }
                break;
            case OrderType.Limit:
                if (request.Price is null)
                {
                    throw new ArgumentException("Limit order requires Price.", nameof(request));
                }
                if (request.TriggerPrice is not null)
                {
                    throw new ArgumentException("Limit order must not specify TriggerPrice.", nameof(request));
                }
                break;
            case OrderType.Stop:
                if (request.TriggerPrice is null)
                {
                    throw new ArgumentException("Stop order requires TriggerPrice.", nameof(request));
                }
                if (request.Price is not null && request.Price <= 0)
                {
                    throw new ArgumentException("Stop order Price must be greater than zero when specified.", nameof(request));
                }
                // price is optional: null -> STOP (market), specified -> STOP_LIMIT
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }

    private ExchangeApiException EnrichBitflyerException(ExchangeApiException ex, string operation)
    {
        if (ex.ExchangeId == _exchangeId && ex.Operation == operation)
        {
            return ex;
        }

        var category = MapErrorCategory(ex.StatusCode, ex.ExchangeErrorCode);

        return new ExchangeApiException(
            message: ex.Message,
            exchangeId: _exchangeId,
            operation: operation,
            statusCode: ex.StatusCode,
            exchangeErrorCode: ex.ExchangeErrorCode,
            errorCategory: category,
            innerException: ex);
    }

    private static ExchangeErrorCategory? MapErrorCategory(System.Net.HttpStatusCode? statusCode, string? exchangeCode)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(exchangeCode)
            ? null
            : exchangeCode.Trim().ToUpperInvariant();

        if (normalizedCode is not null)
        {
            return normalizedCode switch
            {
                "INSUFFICIENT_FUNDS" => ExchangeErrorCategory.Balance,
                "NO_POSITION" => ExchangeErrorCategory.Balance,
                "INVALID_ORDER" or "INVALID_PRODUCT" or "PRODUCT_NOT_FOUND" => ExchangeErrorCategory.Request,
                "LIMIT_OVER" or "ORDER_NOT_ACCEPTABLE" => ExchangeErrorCategory.Request,
                "INVALID_REQUEST" => ExchangeErrorCategory.Request,
                "PARAM_ERROR" => ExchangeErrorCategory.Request,
                "AUTHENTICATION_ERROR" or "PERMISSION_DENIED" => ExchangeErrorCategory.Auth,
                "TOO_MANY_REQUESTS" => ExchangeErrorCategory.RateLimit,
                "SERVICE_UNAVAILABLE" or "INTERNAL_ERROR" => ExchangeErrorCategory.Server,
                "TIMEOUT" => ExchangeErrorCategory.Network,
                _ => ExchangeErrorCategory.Unknown,
            };
        }

        if (statusCode is null) return null;

        return statusCode.Value switch
        {
            System.Net.HttpStatusCode.BadRequest => ExchangeErrorCategory.Request,
            System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden => ExchangeErrorCategory.Auth,
            (System.Net.HttpStatusCode)429 => ExchangeErrorCategory.RateLimit,
            System.Net.HttpStatusCode.InternalServerError or System.Net.HttpStatusCode.ServiceUnavailable => ExchangeErrorCategory.Server,
            _ => ExchangeErrorCategory.Unknown,
        };
    }

    /// <summary>
    /// bitFlyer の Raw ティッカーを抽象層の Ticker に変換する。
    /// </summary>
    private static Ticker MapToTicker(string symbol, BitflyerTickerRaw raw)
    {
        // ここは既存の実装に合わせて調整してください。
        // 例：Bid/Ask/Last/BestBidSize/BestAskSize/Volume 等のマッピング。

        return new Ticker(
            Symbol: symbol,
            BestBid: raw.BestBid,
            BestAsk: raw.BestAsk,
            LastTradedPrice: raw.LastTradedPrice,
            Timestamp: raw.Timestamp);
    }

    #endregion
}
