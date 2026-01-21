using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Api;

public sealed partial class BitflyerRawApi
{
    public Task<Call<string, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            bodyJson,
            "Bitflyer.SendChildOrder",
            BitflyerEndpoints.SendChildOrder(
                bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                json,
                "Bitflyer.SendChildOrder"));

    public Task<Call<string, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            bodyJson,
            "Bitflyer.SendParentOrder",
            BitflyerEndpoints.SendParentOrder(
                bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendParentOrderResponse>(
                json,
                "Bitflyer.SendParentOrder"));

    public Task<Call<Requests.CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelChildOrder",
            BitflyerEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(
                    request,
                    "Bitflyer.CancelChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                json,
                "Bitflyer.CancelChildOrder"));

    public Task<Call<Requests.CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelParentOrder",
            BitflyerEndpoints.CancelParentOrder(
                BitflyerRawJson.SerializeOrThrow(
                    request,
                    "Bitflyer.CancelParentOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelParentOrderResponse>(
                json,
                "Bitflyer.CancelParentOrder"));

    public Task<Call<Requests.GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        Requests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetChildOrders",
            BitflyerEndpoints.GetChildOrders(
                request.ProductCode,
                request.ChildOrderStatusState,
                request.ChildOrderAcceptanceId,
                request.ChildOrderId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.ParentOrderId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(
                json,
                "Bitflyer.GetChildOrders"));

    public Task<Call<Requests.GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        Requests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrders",
            BitflyerEndpoints.GetParentOrders(
                request.ProductCode,
                request.ParentOrderState,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetParentOrdersResponse>>(
                json,
                "Bitflyer.GetParentOrders"));

    public Task<Call<Requests.GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        Requests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrder",
            BitflyerEndpoints.GetParentOrder(
                request.ParentOrderId,
                request.ParentOrderAcceptanceId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawGetParentOrderResponse>(
                json,
                "Bitflyer.GetParentOrder"));
}
