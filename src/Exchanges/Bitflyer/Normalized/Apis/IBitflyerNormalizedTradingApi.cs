using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;

public interface IBitflyerNormalizedTradingApi
{
    Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

}
