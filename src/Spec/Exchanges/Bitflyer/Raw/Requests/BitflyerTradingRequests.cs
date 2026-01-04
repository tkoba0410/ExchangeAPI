using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed record CreateChildOrderRequest(PrivatePost.CreateChildOrderRequest Body);

public sealed record CancelChildOrderRequest(PrivatePost.CancelChildOrderRequest Body);

public sealed record CancelAllChildOrdersRequest(PrivatePost.CancelAllChildOrdersRequest Body);

public sealed record CreateParentOrderRequest(PrivatePost.CreateParentOrderRequest Body);

public sealed record CancelParentOrderRequest(PrivatePost.CancelParentOrderRequest Body);

public sealed record CreateWithdrawalRequest(PrivatePost.CreateWithdrawalRequest Body);

public sealed record SendChildOrderRequest(RawSendChildOrderRequest Body);

public sealed record CancelChildOrderRawRequest(RawCancelChildOrderRequest Body);
