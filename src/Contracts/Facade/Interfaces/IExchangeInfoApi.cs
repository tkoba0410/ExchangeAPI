using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// 取引所の機能や市場情報を提供する抽象インターフェース。
/// </summary>
public interface IExchangeInfoApi
{
    Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default);
}
