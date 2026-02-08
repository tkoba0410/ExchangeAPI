using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record GetDepositWithdrawResponse(
    IReadOnlyList<DepositWithdrawNormalized> Items);
