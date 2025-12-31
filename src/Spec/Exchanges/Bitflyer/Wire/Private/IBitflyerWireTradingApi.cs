using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal interface IBitflyerWireTradingApi
{
    Task<WireResponse<CreateChildOrderResponse>> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<WireResponse<EmptyResponse>> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<WireResponse<EmptyResponse>> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<WireResponse<CreateParentOrderResponse>> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<WireResponse<EmptyResponse>> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<WireResponse<CreateWithdrawalResponse>> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}
