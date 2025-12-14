using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Common.Contract.Interfaces;
using Common.Contract.Enums;

namespace Common.Transport.Mapping;

/// <summary>
/// メモリ内でローカルIDとサーバーIDを対応付ける簡易実装。
/// Transport 層で提供するサンプル/テスト用途の補助実装。永続化や分散は考慮しない。
/// </summary>
public sealed class InMemoryOrderIdMapper : IOrderIdMapper
{
    private readonly ConcurrentDictionary<(string localId, Symbol symbol), string> _localToServer = new();
    private readonly ConcurrentDictionary<(string serverId, Symbol symbol), string> _serverToLocal = new();
    private readonly Queue<(string localId, Symbol symbol)> _orderQueue = new();
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

    public void Save(string localOrderId, string serverOrderId, Symbol symbol)
    {
        if (string.IsNullOrWhiteSpace(localOrderId)) throw new ArgumentNullException(nameof(localOrderId));
        if (string.IsNullOrWhiteSpace(serverOrderId)) throw new ArgumentNullException(nameof(serverOrderId));
        if (symbol == Symbol.Unknown) throw new ArgumentNullException(nameof(symbol));

        var localKey = (localOrderId, symbol);
        var serverKey = (serverOrderId, symbol);

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
                        var removedServerKey = (removedServerId, oldest.symbol);
                        _ = _serverToLocal.TryRemove(removedServerKey, out _);
                    }
                }
            }
        }
    }

    public bool TryGetServerOrderId(string localOrderId, Symbol symbol, out string? serverOrderId)
    {
        if (string.IsNullOrWhiteSpace(localOrderId) || symbol == Symbol.Unknown)
        {
            serverOrderId = null;
            return false;
        }

        var key = (localOrderId, symbol);
        if (_localToServer.TryGetValue(key, out var value))
        {
            serverOrderId = value;
            return true;
        }

        serverOrderId = null;
        return false;
    }

    public bool TryGetLocalOrderId(string serverOrderId, Symbol symbol, out string? localOrderId)
    {
        if (string.IsNullOrWhiteSpace(serverOrderId) || symbol == Symbol.Unknown)
        {
            localOrderId = null;
            return false;
        }

        var key = (serverOrderId, symbol);
        if (_serverToLocal.TryGetValue(key, out var value))
        {
            localOrderId = value;
            return true;
        }

        localOrderId = null;
        return false;
    }
}
