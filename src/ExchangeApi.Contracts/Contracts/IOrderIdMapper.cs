using System;

namespace ExchangeApi.Contracts.Contracts;

/// <summary>
/// ローカル注文IDとサーバー注文IDの対応を保持するためのマッパー。
/// ライブラリは保存を強制しないが、利用者が任意で差し込める拡張ポイントとして提供する。
/// </summary>
public interface IOrderIdMapper
{
    /// <summary>
    /// 対応を保存する。
    /// </summary>
    void Save(string localOrderId, string serverOrderId, string productCode);

    /// <summary>
    /// ローカルIDからサーバーIDを取得する。
    /// </summary>
    bool TryGetServerOrderId(string localOrderId, string productCode, out string? serverOrderId);

    /// <summary>
    /// サーバーIDからローカルIDを取得する。
    /// </summary>
    bool TryGetLocalOrderId(string serverOrderId, string productCode, out string? localOrderId);
}
