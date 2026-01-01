using System.Collections.Generic;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalize;

public sealed record BittradeNormalizedRequest(
    string Operation,
    IReadOnlyDictionary<string, string?> Parameters);

public sealed record BittradeNormalizedCall<TOk, TErr>(
    BittradeNormalizedRequest Request,
    CallResult<TOk, TErr> Result,
    CallMeta Meta);
