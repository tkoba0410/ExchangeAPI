using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
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
/// 現物口座情報（REST）の抽象インターフェース。
/// </summary>
public interface IAccountApi
{
    Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default);
}
