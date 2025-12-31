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

    public async Task<WireResponse> GetTimestampAsync(CancellationToken ct = default)
    {
        var meta = await _restClient.GetRawAsync("v1/common/timestamp", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetSymbolsAsync(CancellationToken ct = default)
    {
        var meta = await _restClient.GetRawAsync("v1/common/symbols", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetCurrenciesAsync(CancellationToken ct = default)
    {
        var meta = await _restClient.GetRawAsync("v1/common/currencys", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetRetailMaintainTimeAsync(CancellationToken ct = default)
    {
        var meta = await _restClient.GetRawAsync("v1/retail/maintain/time", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
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
}
