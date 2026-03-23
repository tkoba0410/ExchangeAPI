using System.Text.Json.Serialization;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.JsonValidation;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.MeaningValidation;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Requests
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

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos
{
    public sealed class CancelChildOrderResponse
    {
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Api
{
    public partial interface IBitflyerPrivateNativeApi
    {
        Task<Call<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>> CancelChildOrderAsync(
            Requests.CancelChildOrderRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNativeApi
    {
        public async Task<Call<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>> CancelChildOrderAsync(
            Requests.CancelChildOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!CancelChildOrderRequestEncoder.TryEncode(request, out var encodedRequest, out var error))
            {
                return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var protocolCall = await _protocol
                .CancelChildOrderAsync(encodedRequest.BodyJson, cancellationToken)
                .ConfigureAwait(false);

            if (protocolCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: protocolCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)protocolCall.Result).Response;

            if (!ProtocolJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            if (string.IsNullOrWhiteSpace(wireResponse.Json))
            {
                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: new Dtos.CancelChildOrderResponse(),
                    child: protocolCall);
            }

            if (!ProtocolJsonValidator.TryValidateObjectResponse(wireResponse, out var json, out error))
            {
                return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            using (json.Document)
            {
                if (!CancelChildOrderResponseConverter.TryConvert(json.Root, out var candidate, out error))
                {
                    return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.CancelChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: protocolCall);
                }

                if (!CancelChildOrderMeaningValidator.TryValidate(candidate!, out var response, out error))
                {
                    return NativeCallFactory.CreateError<Requests.CancelChildOrderRequest, Dtos.CancelChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.CancelChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: protocolCall);
                }

                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.CancelChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: protocolCall);
            }
        }
    }
}
