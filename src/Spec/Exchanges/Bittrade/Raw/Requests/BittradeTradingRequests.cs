using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Requests;

public sealed record CreateOrderRequest(RawCreateOrderRequest Body);

public sealed record CancelOrderRequest(RawOrderId OrderId);

public sealed record CancelOrdersRequest(RawCancelOrdersRequest Body);

public sealed record CancelOpenOrdersRequest(RawCancelOpenOrdersRequest Body);

public sealed record CreateWithdrawRequest(RawCreateWithdrawRequest Body);

public sealed record CancelWithdrawRequest(string WithdrawId);

public sealed record CreateRetailOrderRequest(RawCreateRetailOrderRequest Body);
