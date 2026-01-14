using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Shared.Wire;

/// <summary>
/// Wire 層の送信インターフェース。
/// HTTP ステータス(4xx/5xx)では例外を投げず、Raw 層での解釈に委ねる。
/// 例外化するのは transport レベル（接続失敗、タイムアウト、TLS、キャンセル等）のみ。
/// </summary>
public interface IWireTransport
{
    /// <summary>
    /// 送信結果を Call として返す。JSON のパースや成否判定は Raw の責務。
    /// </summary>
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        ExchangeCode exchange,
        WireCallSpec request,
        CancellationToken ct = default);
}
