using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawTradingApi
{
    Task<RawSendChildOrderResponse> SendChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<RawCancelChildOrderResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RawGetChildOrdersResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<RawGetChildOrdersResponse?> GetChildOrderAsync(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default);
}
