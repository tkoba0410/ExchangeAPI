using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Common.Contract.Extensions;

/// <summary>Side/Sizeを持つ型向けの符号付きサイズ計算。</summary>
public static class SideSizeExtensions
{
    /// <summary>OrderSideと数量から符号付き数量を計算する（Sellなら負値）。</summary>
    public static decimal SignedSize(this OrderSide side, decimal size) =>
        side == OrderSide.Sell ? -size : size;

    public static decimal SignedSize(this OrderRequest order) =>
        order.Side.SignedSize(order.Size);

    public static decimal SignedSize(this OpenOrder order) =>
        order.Side.SignedSize(order.Size);

    public static decimal SignedSize(this ExecutionMarket execution) =>
        execution.Side.SignedSize(execution.Size);

    public static decimal SignedSize(this ExecutionAccount execution) =>
        execution.Side.SignedSize(execution.Size);
}
