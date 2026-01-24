using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw;
using RawPrivateModels = ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Call;

internal sealed class BittradeNormalizedAccountApi : IBittradeNormalizedAccountApi
{
    private readonly IBittradeRawApi _account;
    private readonly string _accountId;

    internal BittradeNormalizedAccountApi(IBittradeRawApi raw, string accountId)
    {
        _account = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _account
            .GetAccountsBalanceByAccountIdCallAsync(new RawPrivateModels.GetAccountBalanceRequest(_accountId), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBalancesRequest(_accountId);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetAccountBalance",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    throw new BittradeNormalizedException("Bittrade balance response invalid.");
                }

                return BittradeNormalizer.NormalizeBalances(ok.Data);
            });
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: rawCall.Meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: rawCall.Meta)
        };
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper)
    {
        try
        {
            var mapped = mapper(raw);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: rawCall.Meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map normalized response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: rawCall.Meta);
        }
    }

}
