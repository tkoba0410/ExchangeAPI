using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

public interface IBitflyerWireMarketDataApi
{
    Task<WireCall> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCorporateLeverageAsync(CancellationToken cancellationToken = default);

    Task<WireCall> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);
}
