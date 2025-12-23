using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Wire;

public interface IBitflyerWireApi
{
    Task<Ticker> GetTickerAsync(string productCode, CancellationToken cancellationToken = default);
    Task<Board> GetBoardAsync(string productCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default);
    Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(string productCode, string? childOrderStatusState = null, string? childOrderAcceptanceId = null, CancellationToken cancellationToken = default);
    Task<CreateChildOrderResponse> CreateChildOrderAsync(CreateChildOrderRequest request, CancellationToken cancellationToken = default);
    Task<EmptyResponse> CancelChildOrderAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default);
    Task<EmptyResponse> CancelAllChildOrdersAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default);
    Task<CreateParentOrderResponse> CreateParentOrderAsync(CreateParentOrderRequest request, CancellationToken cancellationToken = default);
    Task<EmptyResponse> CancelParentOrderAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default);
    Task<CreateWithdrawalResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request, CancellationToken cancellationToken = default);
}
