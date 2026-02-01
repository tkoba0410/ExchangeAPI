using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos;

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
;
