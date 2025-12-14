using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Enums;
using Common.Contract.Dtos;

namespace Common.Contract.Interfaces;

/// <summary>
/// 簡易な注文台帳を管理するためのインターフェース。
/// </summary>
public interface IOrderLedger
{
    Task<OrderLedgerEntry> CreatePendingAsync(OrderRequest request, CancellationToken cancellationToken = default);

    Task MarkSubmittedAsync(string localId, string serverOrderId, CancellationToken cancellationToken = default);

    Task MarkStatusAsync(string localId, OrderState status, string? lastError = null, CancellationToken cancellationToken = default);

    Task<bool> TryGetByLocalIdAsync(string localId, CancellationToken cancellationToken, out OrderLedgerEntry? entry);

    Task<bool> TryGetByServerIdAsync(string serverOrderId, CancellationToken cancellationToken, out OrderLedgerEntry? entry);

    Task<IReadOnlyList<OrderLedgerEntry>> ListActiveAsync(CancellationToken cancellationToken = default);
}
