using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Spec.Wire;

public sealed record WireCall(
    WireRequest Request,
    WireResponse Response,
    CallMeta Meta);
