namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerParentOrderAcceptance(string ParentOrderAcceptanceId);

public sealed record BitflyerParentOrderCancelResult(bool IsSuccess);
