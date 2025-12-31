using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

public interface IBitflyerWireMarketDataApi
{
    Task<WireResponse> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<WireResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
