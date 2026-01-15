using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
namespace ExchangeApi.Contracts.Facade.Interfaces;

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
