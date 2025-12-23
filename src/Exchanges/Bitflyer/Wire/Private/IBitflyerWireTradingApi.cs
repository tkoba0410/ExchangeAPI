using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private;

public interface IBitflyerWireTradingApi
{
    Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}
