using System.Collections.Generic;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record BitflyerRawRequest(
    string Operation,
    IReadOnlyDictionary<string, string?> Parameters);

public sealed record BitflyerRawCall<TOk, TErr>(
    BitflyerRawRequest Request,
    CallResult<TOk, TErr> Result,
    CallMeta Meta);
