using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Contracts.Dtos.Trading;

/// <summary>
/// 抽象注文リクエスト。
/// <remarks>
/// 必須組み合わせ: MARKET は Size のみ必須、LIMIT は Price を必須。
/// </remarks>
/// </summary>
public sealed record OrderRequest(
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    Size Size,
    Price? Price = null)
{
    /// <summary>成行注文を生成するユーティリティ。</summary>
    public static OrderRequest Market(Symbol symbol, Side side, Size size) =>
        new(symbol, side, OrderType.Market, size);

    /// <summary>指値注文を生成するユーティリティ。</summary>
    public static OrderRequest Limit(Symbol symbol, Side side, Size size, Price price) =>
        new(symbol, side, OrderType.Limit, size, price);
}
