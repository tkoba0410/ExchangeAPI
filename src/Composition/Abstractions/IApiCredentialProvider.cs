using ExchangeApi.Composition.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Composition.Abstractions;

/// <summary>
/// Composition（配線）層の責務として、取引所とアカウントに対応する API キー/シークレットを提供する。
/// </summary>
public interface IApiCredentialProvider
{
    /// <summary>
    /// account に紐づく API 認証情報を返す。
    /// </summary>
    ApiCredentials Get(AccountId accountId);
}
