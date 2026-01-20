using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;

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
