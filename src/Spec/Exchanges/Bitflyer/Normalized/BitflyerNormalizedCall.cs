using System.Collections.Generic;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize;

public sealed record BitflyerNormalizedRequest(
    string Operation,
    IReadOnlyDictionary<string, string?> Parameters);

public sealed record BitflyerNormalizedCall<TOk, TErr>(
    BitflyerNormalizedRequest Request,
    CallResult<TOk, TErr> Result,
    CallMeta Meta);
