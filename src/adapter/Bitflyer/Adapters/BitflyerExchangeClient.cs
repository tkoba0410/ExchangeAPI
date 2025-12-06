using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer 用の抽象 API 実装（REST）。
/// </summary>
public sealed class BitflyerExchangeClient : IMarketDataApi, ITradingApi, IMarginAccountApi
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

    /// <summary>
    /// 指定されたシンボルの板情報を取得する。
    /// </summary>
    public async Task<OrderBook> GetOrderBookAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = MapSymbolToProductCode(symbol);

            var rawBoard = await _publicApi
                .GetBoardRawAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            var bids = rawBoard.Bids.Select(b => new OrderBookLevel(b.Price, b.Size)).ToArray();
            var asks = rawBoard.Asks.Select(a => new OrderBookLevel(a.Price, a.Size)).ToArray();

            return new OrderBook(bids, asks, rawBoard.MidPrice);
        }
        catch (SymbolNotSupportedException ex)
        {
            throw new ExchangeApiException(
                message: $"Symbol '{ex.Symbol}' is not supported by bitFlyer.",
                exchangeId: _exchangeId,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getboard API.",
                exchangeId: _exchangeId,
                operation: "GetOrderBook",
                statusCode: null,
                innerException: ex);
        }
    }

    #region IExchangeMarketClient (candlestick)

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        string symbol,
        string timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        // bitFlyer は公式OHLCV APIなし: 明示的に未サポートを通知
        throw new ExchangeApiException(
            message: "Candlestick is not supported by bitFlyer.",
            exchangeId: _exchangeId,
            operation: "GetCandlesticks",
            statusCode: System.Net.HttpStatusCode.NotImplemented,
            exchangeErrorCode: "UNSUPPORTED_OPERATION",
            errorCategory: ExchangeErrorCategory.Request);
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

    public async Task<OrderResult> SendOrderAsync(
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

            return new OrderResult(response.ChildOrderAcceptanceId, request.ClientOrderId);
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

            var response = await _privateTradingApi
                .CancelChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                throw new ExchangeApiException(
                    message: "bitFlyer cancelchildorder returned no response.",
                    exchangeId: _exchangeId,
                    operation: "CancelOrder",
                    statusCode: null);
            }

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

            var response = await _privateTradingApi
                .CancelAllChildOrdersAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                throw new ExchangeApiException(
                    message: "bitFlyer cancelallchildorders returned no response.",
                    exchangeId: _exchangeId,
                    operation: "CancelAllOrders",
                    statusCode: null);
            }

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

    public async Task<IReadOnlyList<Position>> GetOpenPositionsAsync(
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
            throw EnrichBitflyerException(ex, "GetOpenPositions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getpositions API.",
                exchangeId: _exchangeId,
                operation: "GetOpenPositions",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Execution>> GetExecutionsAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var productCode = MapSymbolToProductCode(symbol);

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

    public async Task<OrderStatus> PollOrderStatusAsync(
        string productCode,
        string childOrderAcceptanceId,
        TimeSpan? pollInterval = null,
        int maxAttempts = 30,
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

        var interval = pollInterval ?? TimeSpan.FromSeconds(1);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var orders = await _privateAccountApi
                .GetChildOrdersAsync(productCode, childOrderState: null, childOrderAcceptanceId, cancellationToken)
                .ConfigureAwait(false);

            var order = orders.FirstOrDefault();

            if (order is null)
            {
                // 見つからない場合は完了とみなす（履歴に移動した可能性）。
                return new OrderStatus(
                    ProductCode: productCode,
                    OrderAcceptanceId: childOrderAcceptanceId,
                    Status: OrderStatusType.Completed,
                    ExecutedSize: 0m,
                    OutstandingSize: 0m,
                    Price: null,
                    AveragePrice: null);
            }

            var status = MapOrderStatusType(order.ChildOrderState);
            var mapped = new OrderStatus(
                ProductCode: order.ProductCode,
                OrderAcceptanceId: order.ChildOrderAcceptanceId,
                Status: status,
                ExecutedSize: order.ExecutedSize,
                OutstandingSize: order.OutstandingSize,
                Price: order.Price == 0 ? null : order.Price,
                AveragePrice: order.AveragePrice == 0 ? null : order.AveragePrice);

            if (status is OrderStatusType.Completed or OrderStatusType.Canceled or OrderStatusType.Expired)
            {
                return mapped;
            }

            if (attempt == maxAttempts - 1)
            {
                return mapped with { Status = OrderStatusType.Active };
            }

            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
        }

        // ここには到達しない想定
        throw new InvalidOperationException("Polling loop exited unexpectedly.");
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        try
        {
            var rawOrders = await _privateAccountApi
                .GetChildOrdersAsync(productCode, childOrderState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken)
                .ConfigureAwait(false);

            var mapped = rawOrders.Select(o => new OpenOrder(
                ProductCode: o.ProductCode,
                OrderId: o.ChildOrderId,
                OrderAcceptanceId: o.ChildOrderAcceptanceId,
                Side: MapSide(o.Side),
                OrderType: MapOrderTypeFromExchange(o.ChildOrderType),
                Size: o.Size,
                OutstandingSize: o.OutstandingSize,
                ExecutedSize: o.ExecutedSize,
                Price: o.Price == 0 ? null : o.Price,
                ClientOrderId: null)).ToArray();

            return mapped;
        }
        catch (ExchangeApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getchildorders API.",
                exchangeId: _exchangeId,
                operation: "GetOpenOrders",
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

    private static OrderType MapOrderTypeFromExchange(string childOrderType)
    {
        return childOrderType.ToUpperInvariant() switch
        {
            "LIMIT" => OrderType.Limit,
            "MARKET" => OrderType.Market,
            "STOP" or "STOP_LIMIT" => OrderType.Stop,
            _ => OrderType.Market,
        };
    }

    private static OrderStatusType MapOrderStatusType(string childOrderState)
    {
        return childOrderState.ToUpperInvariant() switch
        {
            "ACTIVE" => OrderStatusType.Active,
            "COMPLETED" => OrderStatusType.Completed,
            "CANCELED" => OrderStatusType.Canceled,
            "EXPIRED" => OrderStatusType.Expired,
            _ => OrderStatusType.Unknown,
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
}
