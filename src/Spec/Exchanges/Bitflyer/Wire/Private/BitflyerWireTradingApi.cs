using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApi : IBitflyerWireTradingApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IRestClient _restClient;

    public BitflyerWireTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<WireResponse> CreateChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.SendChildOrder;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.CancelChildOrder;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.CancelAllChildOrders;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.SendParentOrder;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.CancelParentOrder;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.Withdraw;
        var meta = await _restClient.PostRawAsync(path, request, cancellationToken).ConfigureAwait(false);
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
