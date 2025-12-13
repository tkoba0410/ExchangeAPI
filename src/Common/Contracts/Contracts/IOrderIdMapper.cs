using System;

namespace Common.Contract.Contracts;

/// <summary>
/// ローカル注文IDとサーバー注文IDの対応を保持するためのマッパー。
/// ライブラリは保存を強制しないが、利用者が任意で差し込める拡張ポイントとして提供する。
/// </summary>
/// <remarks>
/// 並行呼び出しが行われるため実装側での thread-safe を前提とする。
/// ライブラリは永続化や期限切れ処理を行わないので、メモリ/DB など用途に応じて利用者が選択する。
/// </remarks>
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
