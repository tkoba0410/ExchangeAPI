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
    public sealed class SendChildOrderRequest
    {
        [JsonPropertyName("product_code")]
        public string? ProductCode { get; init; }

        [JsonPropertyName("child_order_type")]
        public string? ChildOrderType { get; init; }

        [JsonPropertyName("side")]
        public string? Side { get; init; }

        [JsonPropertyName("size")]
        public decimal Size { get; init; }

        [JsonPropertyName("price")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Price { get; init; }

        [JsonPropertyName("minute_to_expire")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinuteToExpire { get; init; }

        [JsonPropertyName("time_in_force")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TimeInForce { get; init; }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos
{
    public sealed class SendChildOrderResponse
    {
        [JsonPropertyName("child_order_acceptance_id")]
        public string ChildOrderAcceptanceId { get; init; } = string.Empty;
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api
{
    public partial interface IBitflyerPrivateNormalizedApi
    {
        Task<Call<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>> SendChildOrderAsync(
            Requests.SendChildOrderRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNormalizedApi
    {
        public async Task<Call<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>> SendChildOrderAsync(
            Requests.SendChildOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!SendChildOrderRequestEncoder.TryEncode(request, out var encodedRequest, out var error))
            {
                return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var wireCall = await _wire
                .SendChildOrderAsync(encodedRequest.BodyJson, cancellationToken)
                .ConfigureAwait(false);

            if (wireCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: wireCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)wireCall.Result).Response;

            if (!WireJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: wireCall);
            }

            if (!WireJsonValidator.TryValidateObjectResponse(wireResponse, out var json, out error))
            {
                return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: wireCall);
            }

            using (json.Document)
            {
                if (!SendChildOrderResponseConverter.TryConvert(json.Root, out var candidate, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.SendChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: wireCall);
                }

                if (!SendChildOrderMeaningValidator.TryValidate(candidate!, out var response, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.SendChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: wireCall);
                }

                return NormalizedCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: wireCall);
            }
        }
    }
}
