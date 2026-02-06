using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record PostOrdersPlaceRequest(RawPostOrdersPlaceRequest Body);

public sealed record PostOrdersSubmitCancelByOrderIdRequest(OrderId OrderId);

public sealed record PostOrdersBatchCancelRequest(RawPostOrdersBatchCancelRequest Body);

public sealed record PostOrdersBatchCancelOpenOrdersRequest(RawPostOrdersBatchCancelOpenOrdersRequest Body);

public sealed record PostWithdrawApiCreateRequest(RawPostWithdrawApiCreateRequest Body);

public sealed record PostWithdrawVirtualByWithdrawIdCancelRequest(WithdrawId WithdrawId);

public sealed record PostWithdrawVirtualByAddressIdCreateRequest(AddressId AddressId);

public sealed record PostWithdrawVirtualByWithdrawIdPlaceRequest(WithdrawId WithdrawId);

public sealed record PostRetailOrderPlaceRequest(RawPostRetailOrderPlaceRequest Body);

public sealed record PostRetailOrderCancelByOrderIdRequest(OrderId OrderId);

public sealed record PostRetailOrderHistoryRequest(RawPostRetailOrderHistoryRequest Body);

public sealed record PostRetailOrderDetailRequest(RawPostRetailOrderDetailRequest Body);

public sealed record PostRetailOrderCreateRequest(RawPostRetailOrderCreateRequest Body);
