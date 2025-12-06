using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer Private REST API の実装。
/// </summary>
public sealed class BitflyerPrivateApi : IBitflyerPrivateApi, IBitflyerPrivateTradingApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getbalance";

        // Stage2 時点ではクエリなし
        return _restClient.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/me/getpositions";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerPositionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/me/getexecutions";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerExecutionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<BitflyerCollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getcollateral";

        return _restClient.GetAsync<BitflyerCollateralResponse>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = "/v1/me/getchildorders";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
            ["child_order_state"] = childOrderState,
            ["child_order_acceptance_id"] = childOrderAcceptanceId,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerChildOrderResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/sendchildorder";

        return _restClient.PostAsync<BitflyerSendChildOrderRequest, BitflyerSendChildOrderResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/cancelchildorder";

        return _restClient.PostAsync<BitflyerCancelChildOrderRequest, BitflyerEmptyResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = "/v1/me/cancelallchildorders";

        return _restClient.PostAsync<BitflyerCancelAllChildOrdersRequest, BitflyerEmptyResponse>(
            path,
            request,
            cancellationToken);
    }
}
