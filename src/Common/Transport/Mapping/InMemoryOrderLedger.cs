using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Interfaces;

namespace Common.Transport.Mapping;

/// <summary>
/// インメモリ実装の簡易注文台帳。
/// <para>
/// 使い方の例:
/// 1) 発注前に <see cref="CreatePendingAsync"/> でエントリを作成し localId を得る
/// 2) 送信に成功したら <see cref="MarkSubmittedAsync"/> に serverOrderId を渡して紐付ける
/// 3) ポーリングなどで状態が変わったら <see cref="MarkStatusAsync"/> で更新する
/// 4) 未完了を確認したいときは <see cref="ListActiveAsync"/> を呼ぶ
/// </para>
/// 永続化や分散は行わないため、再起動で内容は失われる。必要に応じて別実装に差し替える。
/// </summary>
public sealed class InMemoryOrderLedger : IOrderLedger
{
    private readonly ConcurrentDictionary<string, OrderLedgerEntry> _byLocal = new();
    private readonly ConcurrentDictionary<string, string> _serverToLocal = new();

    public Task<OrderLedgerEntry> CreatePendingAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var localId = Guid.NewGuid().ToString("N");
        var entry = OrderLedgerEntry.CreatePending(
            localId: localId,
            exchange: ExchangeCode.Unknown,
            request: request);

        _byLocal[localId] = entry;
        return Task.FromResult(entry);
    }

    public Task MarkSubmittedAsync(string localId, string serverOrderId, CancellationToken cancellationToken = default)
    {
        if (!_byLocal.TryGetValue(localId, out var entry))
        {
            throw new KeyNotFoundException($"Local order id not found: {localId}");
        }

        var updated = entry with
        {
            ServerOrderId = serverOrderId,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        _byLocal[localId] = updated;
        _serverToLocal[serverOrderId] = localId;
        return Task.CompletedTask;
    }

    public Task MarkStatusAsync(string localId, OrderState status, string? lastError = null, CancellationToken cancellationToken = default)
    {
        if (!_byLocal.TryGetValue(localId, out var entry))
        {
            throw new KeyNotFoundException($"Local order id not found: {localId}");
        }

        var updated = entry with
        {
            Status = status,
            UpdatedAt = DateTimeOffset.UtcNow,
            LastError = lastError,
        };
        _byLocal[localId] = updated;
        return Task.CompletedTask;
    }

    public Task<bool> TryGetByLocalIdAsync(string localId, CancellationToken cancellationToken, out OrderLedgerEntry? entry)
    {
        var found = _byLocal.TryGetValue(localId, out var value);
        entry = value;
        return Task.FromResult(found);
    }

    public Task<bool> TryGetByServerIdAsync(string serverOrderId, CancellationToken cancellationToken, out OrderLedgerEntry? entry)
    {
        entry = null;
        if (_serverToLocal.TryGetValue(serverOrderId, out var localId))
        {
            if (_byLocal.TryGetValue(localId, out var value))
            {
                entry = value;
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }

    public Task<IReadOnlyList<OrderLedgerEntry>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var list = _byLocal.Values.Where(e => e.Status == OrderState.Active).ToList();
        return Task.FromResult<IReadOnlyList<OrderLedgerEntry>>(list);
    }
}
