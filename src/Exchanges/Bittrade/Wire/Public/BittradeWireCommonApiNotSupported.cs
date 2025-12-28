using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

internal sealed class BittradeWireCommonApiNotSupported : IBittradeWireCommonApi
{
    private static ExchangeFeatureNotSupportedException NotSupported() =>
        new(ExchangeCode.Bittrade, "WireCommon");

    public Task<RawTimestampResponse> GetTimestampAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken ct = default) =>
        throw NotSupported();
}
