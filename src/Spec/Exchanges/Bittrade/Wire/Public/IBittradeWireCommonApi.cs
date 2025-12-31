using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

public interface IBittradeWireCommonApi
{
    Task<WireResponse> GetTimestampAsync(CancellationToken ct = default);
    Task<WireResponse> GetSymbolsAsync(CancellationToken ct = default);
    Task<WireResponse> GetCurrenciesAsync(CancellationToken ct = default);
    Task<WireResponse> GetRetailMaintainTimeAsync(CancellationToken ct = default);
}
