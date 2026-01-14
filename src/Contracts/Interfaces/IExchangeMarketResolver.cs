using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Contracts.Interfaces;

public interface IExchangeMarketResolver
{
    Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken ct = default);
}
