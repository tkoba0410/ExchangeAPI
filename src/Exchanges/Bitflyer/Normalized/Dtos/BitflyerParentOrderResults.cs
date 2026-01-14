namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerParentOrderAcceptance(string ParentOrderAcceptanceId);

public sealed record BitflyerParentOrderCancelResult(bool IsSuccess);
