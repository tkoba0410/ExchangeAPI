using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 取引所とアカウントに対応する API キー/シークレットを提供する。
/// </summary>
public interface IApiCredentialProvider
{
    /// <summary>
    /// exchange/account に紐づく API 認証情報を返す。
    /// </summary>
    ApiCredentials Get(string exchangeId, string accountId);
}
