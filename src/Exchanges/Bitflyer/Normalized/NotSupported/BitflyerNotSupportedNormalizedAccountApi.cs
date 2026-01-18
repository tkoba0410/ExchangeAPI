using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.NotSupported;

internal sealed class BitflyerNotSupportedNormalizedAccountApi : IBitflyerNormalizedAccountApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bitflyer.NotSupported";

    public Task<Call<GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>(
            Layer,
            Component,
            new GetBalancesRequest(),
            "Account.GetBalances"));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
            Layer,
            Component,
            new GetAccountExecutionsRequest(symbol),
            "Account.GetExecutions"));

    public Task<Call<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
            Layer,
            Component,
            new GetTradingCommissionRequest(symbol),
            "Account.GetTradingCommission"));
}
