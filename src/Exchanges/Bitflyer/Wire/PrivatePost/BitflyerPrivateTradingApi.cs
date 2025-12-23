using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

/// <summary>
/// bitFlyer Private Trading REST API の実装（発注・キャンセル系）。
/// </summary>
internal sealed class BitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateTradingApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.SendChildOrder;

        return _restClient.PostAsync<CreateChildOrderRequest, CreateChildOrderResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.CancelChildOrder;

        return _restClient.PostAsync<CancelChildOrderRequest, EmptyResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        const string path = BitflyerConstants.Paths.CancelAllChildOrders;

        return _restClient.PostAsync<CancelAllChildOrdersRequest, EmptyResponse>(
            path,
            request,
            cancellationToken);
    }

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        const string path = BitflyerConstants.Paths.SendParentOrder;
        return _restClient.PostAsync<CreateParentOrderRequest, CreateParentOrderResponse>(path, request, cancellationToken);
    }

    public Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        const string path = BitflyerConstants.Paths.CancelParentOrder;
        return _restClient.PostAsync<CancelParentOrderRequest, EmptyResponse>(path, request, cancellationToken);
    }

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        const string path = BitflyerConstants.Paths.Withdraw;
        return _restClient.PostAsync<CreateWithdrawalRequest, CreateWithdrawalResponse>(path, request, cancellationToken);
    }
}
