using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record CreateOrderRequest(RawCreateOrderRequest Body);

public sealed record CancelOrderRequest(string OrderId);

public sealed record CancelOrdersRequest(RawCancelOrdersRequest Body);

public sealed record CancelOpenOrdersRequest(RawCancelOpenOrdersRequest Body);

public sealed record CreateWithdrawRequest(RawCreateWithdrawRequest Body);

public sealed record CancelWithdrawRequest(string WithdrawId);

public sealed record CreateWithdrawVirtualByAddressIdRequest(string AddressId);

public sealed record PlaceWithdrawVirtualRequest(string WithdrawId);

public sealed record CreateRetailOrderRequest(RawCreateRetailOrderRequest Body);

public sealed record CancelRetailOrderRequest(string OrderId);

public sealed record PostRetailOrderHistoryRequest(RawRetailOrderHistoryRequest Body);

public sealed record PostRetailOrderDetailRequest(RawRetailOrderDetailRequest Body);
