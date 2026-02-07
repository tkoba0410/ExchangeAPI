using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record ParentOrderAcceptance(AcceptanceId ParentOrderAcceptanceId);

public sealed record ParentOrderCancelResult(bool IsSuccess);
