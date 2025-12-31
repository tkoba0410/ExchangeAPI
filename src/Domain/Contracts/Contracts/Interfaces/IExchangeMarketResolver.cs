using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Interfaces;

public interface IExchangeMarketResolver
{
    Task<ExchangeMarketInfo> ResolveAsync(Symbol symbol, CancellationToken ct = default);
}
