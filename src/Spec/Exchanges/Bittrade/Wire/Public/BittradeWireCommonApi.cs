using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApi : IBittradeWireCommonApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IRestClient _restClient;

    public BittradeWireCommonApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<WireCall> GetTimestampAsync(CancellationToken ct = default) =>
        GetAsync("v1/common/timestamp", ct);

    public Task<WireCall> GetSymbolsAsync(CancellationToken ct = default) =>
        GetAsync("v1/common/symbols", ct);

    public Task<WireCall> GetCurrenciesAsync(CancellationToken ct = default) =>
        GetAsync("v1/common/currencys", ct);

    public Task<WireCall> GetRetailMaintainTimeAsync(CancellationToken ct = default) =>
        GetAsync("v1/retail/maintain/time", ct);

    private async Task<WireCall> GetAsync(string path, CancellationToken ct)
    {
        var request = new WireRequest(
            Method: "GET",
            Path: path,
            Query: null);
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        var response = ToWire(meta);
        return new WireCall(request, response, CreateMeta(response));
    }

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }

    private static CallMeta CreateMeta(WireResponse response)
    {
        var elapsed = response.ElapsedMs is { } ms ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
        var startedAt = DateTimeOffset.UtcNow - elapsed;
        return new CallMeta(startedAt, elapsed, response.RequestId);
    }
}
