using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;

namespace ExchangeApi.Spec.Wire;

/// <summary>
/// Wire 層の送信インターフェース。
/// HTTP ステータス(4xx/5xx)では例外を投げず、Raw 層での解釈に委ねる。
/// 例外化するのは transport レベル（接続失敗、タイムアウト、TLS、キャンセル等）のみ。
/// </summary>
public interface IWireTransport
{
    /// <summary>
    /// 送信結果を WireCall として返す。JSON のパースや成否判定は Raw の責務。
    /// </summary>
    Task<WireCall> SendAsync(
        ExchangeCode exchange,
        WireRequest request,
        CancellationToken ct = default);
}
