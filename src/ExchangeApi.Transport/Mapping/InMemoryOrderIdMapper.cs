using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ExchangeApi.Contracts.Contracts;

namespace ExchangeApi.Transport.Mapping;

/// <summary>
/// メモリ内でローカルIDとサーバーIDを対応付ける簡易実装。
/// テスト/サンプル用途を想定し、永続化や分散は考慮しない。
/// </summary>
public sealed class InMemoryOrderIdMapper : IOrderIdMapper
{
    private readonly ConcurrentDictionary<(string localId, string productCode), string> _localToServer = new();
    private readonly ConcurrentDictionary<(string serverId, string productCode), string> _serverToLocal = new();
    private readonly Queue<(string localId, string productCode)> _orderQueue = new();
    private readonly object _lock = new();
    private readonly int? _capacity;

    public InMemoryOrderIdMapper(int? capacity = null)
    {
        if (capacity is { } cap && cap <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive when specified.");
        }
        _capacity = capacity;
    }

    public void Save(string localOrderId, string serverOrderId, string productCode)
    {
        if (string.IsNullOrWhiteSpace(localOrderId)) throw new ArgumentNullException(nameof(localOrderId));
        if (string.IsNullOrWhiteSpace(serverOrderId)) throw new ArgumentNullException(nameof(serverOrderId));
        if (string.IsNullOrWhiteSpace(productCode)) throw new ArgumentNullException(nameof(productCode));

        var localKey = (localOrderId, productCode);
        var serverKey = (serverOrderId, productCode);

        _localToServer[localKey] = serverOrderId;
        _serverToLocal[serverKey] = localOrderId;

        if (_capacity is null) return;

        lock (_lock)
        {
            _orderQueue.Enqueue(localKey);
            while (_capacity is { } cap && _orderQueue.Count > cap)
            {
                if (_orderQueue.TryDequeue(out var oldest))
                {
                    if (_localToServer.TryRemove(oldest, out var removedServerId))
                    {
                        var removedServerKey = (removedServerId, oldest.productCode);
                        _ = _serverToLocal.TryRemove(removedServerKey, out _);
                    }
                }
            }
        }
    }

    public bool TryGetServerOrderId(string localOrderId, string productCode, out string? serverOrderId)
    {
        if (string.IsNullOrWhiteSpace(localOrderId) || string.IsNullOrWhiteSpace(productCode))
        {
            serverOrderId = null;
            return false;
        }

        var key = (localOrderId, productCode);
        if (_localToServer.TryGetValue(key, out var value))
        {
            serverOrderId = value;
            return true;
        }

        serverOrderId = null;
        return false;
    }

    public bool TryGetLocalOrderId(string serverOrderId, string productCode, out string? localOrderId)
    {
        if (string.IsNullOrWhiteSpace(serverOrderId) || string.IsNullOrWhiteSpace(productCode))
        {
            localOrderId = null;
            return false;
        }

        var key = (serverOrderId, productCode);
        if (_serverToLocal.TryGetValue(key, out var value))
        {
            localOrderId = value;
            return true;
        }

        localOrderId = null;
        return false;
    }
}
