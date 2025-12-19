using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Common.Interfaces;

/// <summary>
/// 取引所とアカウントに対応する API キー/シークレットを提供する。
/// スレッドセーフであることと、失効・ローテーション時の扱いは実装側で決定する。
/// </summary>
public interface IApiCredentialProvider
{
    /// <summary>
    /// exchange/account に紐づく API 認証情報を返す。
    /// </summary>
    ApiCredentials Get(ExchangeCode exchange, string accountId);
}
