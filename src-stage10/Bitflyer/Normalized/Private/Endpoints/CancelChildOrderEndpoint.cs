using System.Text.Json.Serialization;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Encoder;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.JsonValidation;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.MeaningValidation;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Requests
{
    public sealed class CancelChildOrderRequest
    {
        [JsonPropertyName("product_code")]
        public string? ProductCode { get; init; }

        [JsonPropertyName("child_order_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChildOrderId { get; init; }

        [JsonPropertyName("child_order_acceptance_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChildOrderAcceptanceId { get; init; }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos
{
    public sealed class CancelChildOrderResponse
    {
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api
{
    public partial interface IBitflyerPrivateNormalizedApi
    {
        Task<Call<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>> CancelChildOrderAsync(
            Requests.CancelChildOrderRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNormalizedApi
    {
        public async Task<Call<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>> CancelChildOrderAsync(
            Requests.CancelChildOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!CancelChildOrderRequestEncoder.TryEncode(request, out var encodedRequest, out var error))
            {
                return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var wireCall = await _wire
                .CancelChildOrderAsync(encodedRequest.BodyJson, cancellationToken)
                .ConfigureAwait(false);

            if (wireCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: wireCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)wireCall.Result).Response;

            if (!WireJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: wireCall);
            }

            if (string.IsNullOrWhiteSpace(wireResponse.Json))
            {
                return NormalizedCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: new Dtos.CancelChildOrderResponse(),
                    child: wireCall);
            }

            if (!WireJsonValidator.TryValidateObjectResponse(wireResponse, out var json, out error))
            {
                return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: wireCall);
            }

            using (json.Document)
            {
                if (!CancelChildOrderResponseConverter.TryConvert(json.Root, out var candidate, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.CancelChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: wireCall);
                }

                if (!CancelChildOrderMeaningValidator.TryValidate(candidate!, out var response, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.CancelChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: wireCall);
                }

                return NormalizedCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: wireCall);
            }
        }
    }
}
