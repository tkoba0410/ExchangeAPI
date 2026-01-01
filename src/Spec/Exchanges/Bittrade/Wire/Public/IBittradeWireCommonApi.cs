using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

public interface IBittradeWireCommonApi
{
    Task<WireCall> GetTimestampAsync(CancellationToken ct = default);
    Task<WireCall> GetSymbolsAsync(CancellationToken ct = default);
    Task<WireCall> GetCurrenciesAsync(CancellationToken ct = default);
    Task<WireCall> GetRetailMaintainTimeAsync(CancellationToken ct = default);
}
