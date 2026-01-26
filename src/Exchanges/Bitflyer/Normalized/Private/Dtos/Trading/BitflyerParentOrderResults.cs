namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Trading;

public sealed record BitflyerParentOrderAcceptance(string ParentOrderAcceptanceId);

public sealed record BitflyerParentOrderCancelResult(bool IsSuccess);
