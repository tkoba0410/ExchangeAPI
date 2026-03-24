using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public interface IBitflyerPublicProtocolApi
{
    Task<Call<ProtocolRequest, ProtocolResponse>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetBoardCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetBoardStateCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetHealthCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetFundingRateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetChatsCallAsync(
        string? fromDate,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default);
}
