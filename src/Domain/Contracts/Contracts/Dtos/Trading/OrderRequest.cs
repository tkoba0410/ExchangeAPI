using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 抽象注文リクエスト。
/// <remarks>
/// 必須組み合わせ: MARKET は Size のみ必須、LIMIT は Price を必須、STOP/STOP_LIMIT は TriggerPrice を必須（STOP_LIMIT は Price も必須）。
/// TimeInForce は LIMIT/STOP_LIMIT のみ有効で、各取引所のサポート範囲に従う。
/// PriceIncrement/SizeIncrement/MinSize/MaxSize/MinNotional は ExchangeInfo 由来のバリデーションヒントであり、欠損時は取引所デフォルトに従う。
/// </remarks>
/// </summary>
public sealed record OrderRequest(
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    Size Size,
    Price? Price = null,
    Price? TriggerPrice = null,
    int? MinuteToExpire = null,
    TimeInForce? TimeInForce = null,
    Price? PriceIncrement = null,
    Size? SizeIncrement = null,
    Size? MinSize = null,
    Size? MaxSize = null,
    decimal? MinNotional = null)
{
    /// <summary>成行注文を生成するユーティリティ。</summary>
    public static OrderRequest Market(Symbol symbol, Side side, Size size) =>
        new(symbol, side, OrderType.Market, size);

    /// <summary>指値注文を生成するユーティリティ。</summary>
    public static OrderRequest Limit(Symbol symbol, Side side, Size size, Price price) =>
        new(symbol, side, OrderType.Limit, size, price);

    /// <summary>逆指値（成行）注文を生成するユーティリティ。</summary>
    public static OrderRequest Stop(Symbol symbol, Side side, Size size, Price triggerPrice) =>
        new(symbol, side, OrderType.Stop, size, null, triggerPrice);
}
