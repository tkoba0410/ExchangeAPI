using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Common.Interfaces;

public interface IExchangeMarketResolver
{
    Task<ExchangeMarketInfo> ResolveAsync(Symbol symbol, CancellationToken ct = default);
}
