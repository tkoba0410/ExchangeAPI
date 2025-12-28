using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

public interface IBittradeWireCommonApi
{
    Task<RawTimestampResponse> GetTimestampAsync(CancellationToken ct = default);
    Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken ct = default);
    Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken ct = default);
}
