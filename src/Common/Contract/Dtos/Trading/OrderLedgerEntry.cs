using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>注文台帳の1エントリ。</summary>
public sealed record OrderLedgerEntry(
    string LocalId,
    ExchangeCode Exchange,
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    decimal Size,
    decimal? Price,
    decimal? TriggerPrice,
    string? ServerOrderId,
    OrderState Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? LastError = null)
{
    /// <summary>注文リクエストから台帳エントリを生成するユーティリティ。</summary>
    public static OrderLedgerEntry CreatePending(
        string localId,
        ExchangeCode exchange,
        OrderRequest request,
        string? serverOrderId = null,
        DateTimeOffset? createdAt = null,
        OrderState status = OrderState.Active)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var now = createdAt ?? DateTimeOffset.UtcNow;
        return new OrderLedgerEntry(
            LocalId: localId,
            Exchange: exchange,
            Symbol: request.Symbol,
            Side: request.Side,
            OrderType: request.OrderType,
            Size: request.Size,
            Price: request.Price,
            TriggerPrice: request.TriggerPrice,
            ServerOrderId: serverOrderId,
            Status: status,
            CreatedAt: now,
            UpdatedAt: now);
    }
}
