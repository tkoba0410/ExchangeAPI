using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal interface IBittradeWireCommonApi
{
    Task<WireResponse<RawTimestampResponse>> GetTimestampAsync(CancellationToken ct = default);
    Task<WireResponse<RawSymbolsResponse>> GetSymbolsAsync(CancellationToken ct = default);
    Task<WireResponse<RawCurrenciesResponse>> GetCurrenciesAsync(CancellationToken ct = default);
}
