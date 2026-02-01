using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Composition.Extensions;

/// <summary>Side/Sizeを持つ型向けの符号付きサイズ計算。</summary>
public static class SideSizeExtensions
{
    /// <summary>Sideと数量から符号付き数量を計算する（Sellなら負値）。</summary>
    public static Size SignedSize(this Side side, Size size) =>
        new(side == Side.Sell ? -size.Value : size.Value);

    public static Size SignedSize(this OrderRequest order) =>
        order.Side.SignedSize(order.Size);

    public static Size SignedSize(this OpenOrder order) =>
        order.Side.SignedSize(order.Size);

    public static Size SignedSize(this ExecutionMarket execution) =>
        execution.Side.SignedSize(execution.Size);

    public static Size SignedSize(this ExecutionAccount execution) =>
        execution.Side.SignedSize(execution.Size);
}
