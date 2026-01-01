using System.Collections.Generic;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeRawRequest(
    string Operation,
    IReadOnlyDictionary<string, string?> Parameters);

public sealed record BittradeRawCall<TOk, TErr>(
    BittradeRawRequest Request,
    CallResult<TOk, TErr> Result,
    CallMeta Meta);
