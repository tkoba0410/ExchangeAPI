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

public sealed record CreateRetailOrderRequest(RawCreateRetailOrderRequest Body);
