using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public interface IBitflyerPublicProtocolApi
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetMarketsAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBoardAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBoardStateAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetHealthAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCorporateLeverageAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChatsAsync(
        string? fromDate,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTickerAsync(
        string? productCode,
        CancellationToken cancellationToken = default);
}
