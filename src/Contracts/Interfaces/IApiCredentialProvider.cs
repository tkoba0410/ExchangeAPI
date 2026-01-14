using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Interfaces;

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
