using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.NotSupported;

internal sealed class BittradePreconditionMissingNormalizedAccountApi : IBittradeNormalizedAccountApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.PreconditionMissing";
    private readonly string _accountId;

    public BittradePreconditionMissingNormalizedAccountApi(string accountId)
    {
        _accountId = accountId;
    }

    public Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>(
            new GetBalancesRequest(_accountId)));

    public Task<Call<GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>(
            new GetAccountsRequest()));

    public Task<Call<GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>(
            request));

    public Task<Call<GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>(
            new GetWithdrawVirtualAddressesRequest()));

    public Task<Call<GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>(
            new GetRetailAccountBalanceRequest()));

    private static Call<TReq, TOk> CreatePreconditionMissing<TReq, TOk>(TReq request)
    {
        var error = new CallError(CallErrorKind.Semantic, "PreconditionMissing:accountId");
        var meta = CallMeta.CreateInternal(Layer, Component);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }
}
