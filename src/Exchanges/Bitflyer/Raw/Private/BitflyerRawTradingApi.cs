using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

internal sealed class BitflyerRawTradingApi : IBitflyerRawTradingApi
{
    private readonly IRestClient _restClient;

    public BitflyerRawTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<RawSendChildOrderResponse> SendChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.SendChildOrder;
        return _restClient.PostAsync<RawSendChildOrderRequest, RawSendChildOrderResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<RawCancelChildOrderResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.CancelChildOrder;
        return _restClient.PostAsync<RawCancelChildOrderRequest, RawCancelChildOrderResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<IReadOnlyList<RawGetChildOrdersResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetChildOrders;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode.Value,
            [BitflyerConstants.QueryKeys.ChildOrderStatusState] = childOrderStatusState,
            [BitflyerConstants.QueryKeys.ChildOrderAcceptanceId] = childOrderAcceptanceId,
            [BitflyerConstants.QueryKeys.ChildOrderId] = childOrderId,
            [BitflyerConstants.QueryKeys.ParentOrderId] = parentOrderId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<RawGetChildOrdersResponse>>(path, query, cancellationToken);
    }

    public async Task<RawGetChildOrdersResponse?> GetChildOrderAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var list = await GetChildOrdersAsync(
                productCode,
                childOrderAcceptanceId: childOrderAcceptanceId,
                childOrderId: childOrderId,
                count: 1,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return list.FirstOrDefault();
    }
}
