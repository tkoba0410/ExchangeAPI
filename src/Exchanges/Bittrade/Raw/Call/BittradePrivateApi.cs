using System;
using System.Text.Json;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateApi : IBittradePrivateApi
{
    private readonly IWireTransport _wire;

    public BittradePrivateApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetAccounts",
            BittradeEndpoints.GetAccounts(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawAccountsResponse>(json, "Bittrade.GetAccounts"));

    public Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetAccountBalance",
            BittradeEndpoints.GetAccountBalance(request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawBalancesResponse>(
                json,
                "Bittrade.GetAccountBalance"));

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetOpenOrders",
            BittradeEndpoints.GetOpenOrders(request.Symbol, request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOpenOrdersResponse>(
                json,
                "Bittrade.GetOpenOrders"));

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetOrder",
            BittradeEndpoints.GetOrder(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(json, "Bittrade.GetOrder"));

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrderMatchResultsAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetOrderMatchResults",
            BittradeEndpoints.GetOrderMatchResults(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderMatchResultsResponse>(
                json,
                "Bittrade.GetOrderMatchResults"));

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetMatchResults",
            BittradeEndpoints.GetMatchResults(
                request.Symbol,
                request.Types,
                request.StartDate,
                request.EndDate,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Direct,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawMatchResultsResponse>(
                json,
                "Bittrade.GetMatchResults"));

    public Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawsAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetDepositWithdraws",
            BittradeEndpoints.GetDepositWithdraws(
                request.Type,
                request.Currency,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepositWithdrawsResponse>(
                json,
                "Bittrade.GetDepositWithdraws"));

    public Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrdersAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetRetailOrders",
            BittradeEndpoints.GetRetailOrders(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.GetRetailOrders"));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bittrade, spec, cancellationToken).ConfigureAwait(false);
        return CreateCall(request, component, wireCall, parse);
    }

    private static Call<TReq, TRes> CreateCall<TReq, TRes>(
        TReq request,
        string component,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: component,
            Tags: null,
            Children: new[] { wireCall.Id });

        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse, meta),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse,
        CallMeta meta)
    {
        var metaWithRaw = meta with { RawJson = response.Json };
        if (response.StatusCode is < 200 or >= 300)
        {
            var error = new CallError(
                CallErrorKind.Http,
                $"{component} failed with status {response.StatusCode}.",
                HttpStatus: response.StatusCode,
                BodySnippet: Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: metaWithRaw);
        }

        try
        {
            var parsed = parse(response.Json);
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Ok(parsed),
                Meta: metaWithRaw);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            var error = new CallError(
                CallErrorKind.Codec,
                $"{component} failed to parse response.",
                ex,
                response.StatusCode,
                Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: metaWithRaw);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
