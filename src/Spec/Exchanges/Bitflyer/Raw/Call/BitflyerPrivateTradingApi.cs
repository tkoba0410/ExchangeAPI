using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

/// <summary>
/// bitFlyer Private Trading REST API の実装（発注・キャンセル系）。
/// </summary>
public sealed class BitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly IWireTransport _wire;

    public BitflyerPrivateTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<Requests.CreateChildOrderRequest, CreateChildOrderResponse>> CreateChildOrderAsync(
        Requests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var raw = BitflyerRawMappers.MapSendChildOrderRequest(request.Body);
        return SendAndParse(
            request,
            "Bitflyer.CreateChildOrder",
            BitflyerEndpoints.SendChildOrder(BitflyerRawJson.SerializeOrThrow(raw, "Bitflyer.CreateChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CreateChildOrderResponse>(
                json,
                "Bitflyer.CreateChildOrder"));
    }

    public Task<Call<Requests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelChildOrder",
            BitflyerEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(request.Body, "Bitflyer.CancelChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<EmptyResponse>(
                json,
                "Bitflyer.CancelChildOrder"));

    public Task<Call<Requests.CancelAllChildOrdersRequest, EmptyResponse>> CancelAllChildOrdersAsync(
        Requests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelAllChildOrders",
            BitflyerEndpoints.CancelAllChildOrders(
                BitflyerRawJson.SerializeOrThrow(request.Body, "Bitflyer.CancelAllChildOrders")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<EmptyResponse>(
                json,
                "Bitflyer.CancelAllChildOrders"));

    public Task<Call<Requests.CreateWithdrawalRequest, CreateWithdrawalResponse>> CreateWithdrawalAsync(
        Requests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CreateWithdrawal",
            BitflyerEndpoints.Withdraw(
                BitflyerRawJson.SerializeOrThrow(request.Body, "Bitflyer.CreateWithdrawal")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CreateWithdrawalResponse>(
                json,
                "Bitflyer.CreateWithdrawal"));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bitflyer, spec, cancellationToken).ConfigureAwait(false);
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
