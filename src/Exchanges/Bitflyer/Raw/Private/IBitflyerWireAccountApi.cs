using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawAccountApi
{
    Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawAccountApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireAccountApi : IBitflyerRawAccountApi
{
}
