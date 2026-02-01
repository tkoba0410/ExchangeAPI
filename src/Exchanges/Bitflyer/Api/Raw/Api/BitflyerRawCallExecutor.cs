using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;

internal sealed class BitflyerRawCallExecutor
{
    private readonly IWireTransport _wire;

    public BitflyerRawCallExecutor(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
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
        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: wireCall.Meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: wireCall.Meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
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
                Meta: wireCall.Meta);
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
                Meta: wireCall.Meta);
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
                Meta: wireCall.Meta);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
