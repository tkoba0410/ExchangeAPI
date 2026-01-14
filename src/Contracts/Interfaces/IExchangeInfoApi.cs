using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.CallCommon;
namespace ExchangeApi.Contracts.Interfaces;

/// <summary>
/// 取引所の機能や市場情報を提供する抽象インターフェース。
/// </summary>
public interface IExchangeInfoApi
{
    Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default);
}
