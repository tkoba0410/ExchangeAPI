using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.NotSupported;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;

internal sealed class BittradeNormalizedAccountApi
{
    private readonly IBittradeRawApi _account;
    private readonly string? _accountId;

    internal BittradeNormalizedAccountApi(IBittradeRawApi raw, string? accountId)
    {
        _account = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public async Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_accountId))
        {
            return BittradeNotSupportedNormalizedCalls.Create<
                NormalizedRequests.GetBalancesRequest,
                IReadOnlyList<BittradeBalanceEntryNormalized>>(
                new NormalizedRequests.GetBalancesRequest(string.Empty),
                "AccountIdRequired");
        }

        var rawCall = await _account
            .GetAccountsBalanceByAccountIdCallAsync(new RawPrivateRequests.GetAccountBalanceRequest(_accountId), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBalancesRequest(_accountId!);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetAccountBalance",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    throw new InvalidOperationException("Bittrade balance response invalid.");
                }

                return BittradeNormalizer.NormalizeBalances(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _account
            .GetAccountsCallAsync(new RawPrivateRequests.GetAccountsRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetAccountsRequest();

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetAccounts",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade accounts response invalid.");
                }

                return BittradeNormalizer.NormalizeAccounts(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        NormalizedRequests.GetDepositWithdrawRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _account
            .GetDepositWithdrawCallAsync(new RawPrivateRequests.GetDepositWithdrawsRequest(
                request.Type,
                request.Currency,
                request.From,
                request.Size,
                request.Direct), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetDepositWithdraws",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade deposit/withdraw response invalid.");
                }

                return BittradeNormalizer.NormalizeDepositWithdraws(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _account
            .GetWithdrawVirtualAddressesCallAsync(new RawPrivateRequests.GetWithdrawVirtualAddressesRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetWithdrawVirtualAddressesRequest();

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetWithdrawVirtualAddresses",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade withdraw addresses response invalid.");
                }

                return BittradeNormalizer.NormalizeWithdrawVirtualAddresses(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _account
            .GetRetailAccountBalanceCallAsync(new RawPrivateRequests.GetRetailAccountBalanceRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetRetailAccountBalanceRequest();

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetRetailAccountBalance",
            ok =>
            {
                if (ok.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail balance response invalid.");
                }

                return BittradeNormalizer.NormalizeRetailBalances(ok.Data);
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
