using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

internal interface IBittradeWireCommonApi
{
    Task<TimestampResponse> GetTimestampAsync(CancellationToken ct = default);
    Task<SymbolsResponse> GetSymbolsAsync(CancellationToken ct = default);
    Task<CurrenciesResponse> GetCurrenciesAsync(CancellationToken ct = default);
}
