namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerParentOrderAcceptance(string ParentOrderAcceptanceId);

public sealed record BitflyerParentOrderCancelResult(bool IsSuccess);
