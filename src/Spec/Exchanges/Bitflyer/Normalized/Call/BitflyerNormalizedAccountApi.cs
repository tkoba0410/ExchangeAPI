using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Call;

internal sealed class BitflyerNormalizedAccountApi : IBitflyerNormalizedAccountApi
{
    private readonly IBitflyerRawAccountApi _accountApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedAccountApi(IBitflyerRawAccountApi accountApi, IExchangeMarketResolver markets)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var rawBalances = await _accountApi.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerAccountMapper.MapBalances(rawBalances);
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var raw = await _accountApi
            .GetExecutionsAsync(productCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return BitflyerAccountMapper.MapAccountExecutions(symbol, raw);
    }

    public async Task<JsonElement> GetTradingCommissionAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        return await _accountApi.GetTradingCommissionAsync(productCode, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<Balance>, JsonElement>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _accountApi.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBalances", new Dictionary<string, string?>());
        return CreateCall(rawCall, request, BitflyerAccountMapper.MapBalances);
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<ExecutionAccount>, JsonElement>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _accountApi
            .GetExecutionsCallAsync(productCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetExecutions", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["productCode"] = productCode.Value,
        });

        return CreateCall(
            rawCall,
            request,
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<BitflyerNormalizedCall<JsonElement, JsonElement>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _accountApi.GetTradingCommissionCallAsync(productCode, cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetTradingCommission", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["productCode"] = productCode.Value,
        });

        return CreateCall(rawCall, request, raw => raw);
    }

    private static BitflyerNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BitflyerRawCall<TRaw, JsonElement> rawCall,
        BitflyerNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }

    private async Task<RawProductCode> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return new RawProductCode(market.ProductCode);
    }
}
