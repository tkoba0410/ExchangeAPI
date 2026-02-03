using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record CreateOrderRequest(RawCreateOrderRequest Body);

public sealed record CancelOrderRequest(OrderId OrderId);

public sealed record CancelOrdersRequest(RawCancelOrdersRequest Body);

public sealed record CancelOpenOrdersRequest(RawCancelOpenOrdersRequest Body);

public sealed record CreateWithdrawRequest(RawCreateWithdrawRequest Body);

public sealed record CancelWithdrawRequest(WithdrawId WithdrawId);

public sealed record CreateWithdrawVirtualByAddressIdRequest(AddressId AddressId);

public sealed record PlaceWithdrawVirtualRequest(WithdrawId WithdrawId);

public sealed record CreateRetailOrderRequest(RawCreateRetailOrderRequest Body);

public sealed record CancelRetailOrderRequest(OrderId OrderId);

public sealed record PostRetailOrderHistoryRequest(RawRetailOrderHistoryRequest Body);

public sealed record PostRetailOrderDetailRequest(RawRetailOrderDetailRequest Body);
