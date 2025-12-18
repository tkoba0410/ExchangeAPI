using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Common.Extensions;

/// <summary>Side/Sizeを持つ型向けの符号付きサイズ計算。</summary>
public static class SideSizeExtensions
{
    /// <summary>Sideと数量から符号付き数量を計算する（Sellなら負値）。</summary>
    public static decimal SignedSize(this Side side, decimal size) =>
        side == Side.Sell ? -size : size;

    public static decimal SignedSize(this OrderRequest order) =>
        order.Side.SignedSize(order.Size);

    public static decimal SignedSize(this OpenOrder order) =>
        order.Side.SignedSize(order.Size);

    public static decimal SignedSize(this ExecutionMarket execution) =>
        execution.Side.SignedSize(execution.Size);

    public static decimal SignedSize(this ExecutionAccount execution) =>
        execution.Side.SignedSize(execution.Size);
}
