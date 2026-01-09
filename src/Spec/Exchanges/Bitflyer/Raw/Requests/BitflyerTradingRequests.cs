using PrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed record CreateChildOrderRequest(PrivateModels.CreateChildOrderRequest Body);

public sealed record CreateParentOrderRequest(PrivateModels.CreateParentOrderRequest Body);

public sealed record CancelChildOrderRequest(PrivateModels.CancelChildOrderRequest Body);

public sealed record CancelParentOrderRequest(PrivateModels.CancelParentOrderRequest Body);

public sealed record CancelAllChildOrdersRequest(PrivateModels.CancelAllChildOrdersRequest Body);

public sealed record CreateWithdrawalRequest(PrivateModels.CreateWithdrawalRequest Body);

public sealed record SendChildOrderRequest(RawSendChildOrderRequest Body);

public sealed record CancelChildOrderRawRequest(RawCancelChildOrderRequest Body);

public sealed record SendParentOrderRequest(RawSendParentOrderRequest Body);

public sealed record CancelParentOrderRawRequest(RawCancelParentOrderRequest Body);
