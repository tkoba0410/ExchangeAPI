using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;

internal sealed class BitflyerNormalizedAccountApi
{
    private readonly IBitflyerRawApi _raw;
    private readonly IBitflyerMarketResolver _markets;

    public BitflyerNormalizedAccountApi(IBitflyerRawApi raw, IBitflyerMarketResolver markets)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<PrivateRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceCallAsync(new RawPrivateRequests.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalancesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBalances", BitflyerAccountMapper.MapBalances);
    }

    public async Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetPermissionsCallAsync(new RawPrivateRequests.GetPermissionsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetPermissionsRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetPermissions", raw => raw);
    }

    public async Task<Call<PrivateRequests.GetCollateralRequest, BitflyerCollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralCallAsync(new RawPrivateRequests.GetCollateralRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralRequest();
        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetCollateral",
            raw => new BitflyerCollateralNormalized(raw.Collateral, raw.OpenPositionPnl, raw.RequireCollateral, raw.KeepRate));
    }

    public async Task<Call<PrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<BitflyerCollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralAccountsCallAsync(new RawPrivateRequests.GetCollateralAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralAccountsRequest();
        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetCollateralAccounts",
            raw =>
            {
                IReadOnlyList<BitflyerCollateralAccountNormalized> mapped = raw
                    .Select(item => new BitflyerCollateralAccountNormalized(item.CurrencyCode, item.Amount, item.Available))
                    .ToArray();
                return mapped;
            });
    }

    public async Task<Call<PrivateRequests.GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetAddressesCallAsync(new RawPrivateRequests.GetAddressesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetAddressesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetAddresses", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetCoinInsRequest, BitflyerRawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCoinInsCallAsync(new RawPrivateRequests.GetCoinInsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCoinInsRequest(count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetCoinIns", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCoinOutsCallAsync(new RawPrivateRequests.GetCoinOutsRequest(messageId, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCoinOutsRequest(messageId, count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetCoinOuts", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBankAccountsCallAsync(new RawPrivateRequests.GetBankAccountsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBankAccountsRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBankAccounts", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetDepositsRequest, BitflyerRawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetDepositsCallAsync(new RawPrivateRequests.GetDepositsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetDepositsRequest(count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetDeposits", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        string currencyCode,
        int bankAccountId,
        decimal amount,
        string? code = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .WithdrawCallAsync(new RawPrivateRequests.CreateWithdrawalRequest
            {
                CurrencyCode = currencyCode,
                BankAccountId = bankAccountId,
                Amount = amount,
                Code = code,
            }, cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.WithdrawRequest(currencyCode, bankAccountId, amount, code);
        return CreateCall(rawCall, request, "Bitflyer.Withdraw", raw => new BitflyerWithdrawResultNormalized(raw.MessageId));
    }

    public async Task<Call<PrivateRequests.GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetWithdrawalsCallAsync(new RawPrivateRequests.GetWithdrawalsRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetWithdrawalsRequest(count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetWithdrawals", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceHistoryCallAsync(new RawPrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetBalanceHistoryRequest(currencyCode, count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetBalanceHistory", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetPositionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetPositions",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetPositions",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetPositionsCallAsync(new RawPrivateRequests.GetPositionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetPositions",
            raw =>
            {
                IReadOnlyList<BitflyerPositionNormalized> mapped = raw
                    .Select(item => new BitflyerPositionNormalized(
                        item.ProductCode,
                        BitflyerCommonMapper.MapSide(item.Side),
                        item.Size,
                        item.Price,
                        item.Pnl,
                        item.OpenDate))
                    .ToArray();
                return mapped;
            });
    }

    public async Task<Call<PrivateRequests.GetCollateralHistoryRequest, BitflyerRawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCollateralHistoryCallAsync(new RawPrivateRequests.GetCollateralHistoryRequest(count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PrivateRequests.GetCollateralHistoryRequest(count, before, after);
        return CreateCall(rawCall, request, "Bitflyer.GetCollateralHistory", raw => new BitflyerRawJsonNormalized(raw.RawJson));
    }

    public async Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetAccountExecutionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetExecutionsPrivateCallAsync(new RawPrivateRequests.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new PrivateRequests.GetTradingCommissionRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetTradingCommissionCallAsync(new RawPrivateRequests.GetTradingCommissionRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetTradingCommission",
            raw => ParseTradingCommission(raw.RawJson, productCode));
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

    private static BitflyerTradingCommissionNormalized ParseTradingCommission(string? rawJson, string productCode)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new InvalidOperationException("Trading commission response is empty.");
        }

        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;
        string? parsedProductCode = null;
        decimal? commissionRate = null;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("product_code", out var productCodeElement)
                && productCodeElement.ValueKind == JsonValueKind.String)
            {
                parsedProductCode = productCodeElement.GetString();
            }

            if (root.TryGetProperty("commission_rate", out var commissionElement))
            {
                commissionRate = TryParseDecimal(commissionElement);
            }
        }

        return new BitflyerTradingCommissionNormalized(
            ProductCode: parsedProductCode ?? productCode,
            CommissionRate: commissionRate);
    }

    private static decimal? TryParseDecimal(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.String => TryParseDecimalString(element.GetString()),
            _ => null
        };
    }

    private static decimal? TryParseDecimalString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
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

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: marketCall.Meta);
    }
}
