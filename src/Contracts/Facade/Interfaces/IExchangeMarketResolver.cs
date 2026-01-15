using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Contracts.Facade.Interfaces;

public interface IExchangeMarketResolver
{
    Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken ct = default);
}
