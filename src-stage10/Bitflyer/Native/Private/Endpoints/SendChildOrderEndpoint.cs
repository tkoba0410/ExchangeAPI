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

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos
{
    public sealed class SendChildOrderResponse
    {
        [JsonPropertyName("child_order_acceptance_id")]
        public string ChildOrderAcceptanceId { get; init; } = string.Empty;
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Api
{
    public partial interface IBitflyerPrivateNativeApi
    {
        Task<Call<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>> SendChildOrderAsync(
            Requests.SendChildOrderRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNativeApi
    {
        public async Task<Call<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>> SendChildOrderAsync(
            Requests.SendChildOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!SendChildOrderRequestEncoder.TryEncode(request, out var encodedRequest, out var error))
            {
                return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var protocolCall = await _protocol
                .SendChildOrderAsync(encodedRequest.BodyJson, cancellationToken)
                .ConfigureAwait(false);

            if (protocolCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: protocolCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)protocolCall.Result).Response;

            if (!ProtocolJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            if (!ProtocolJsonValidator.TryValidateObjectResponse(wireResponse, out var json, out error))
            {
                return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            using (json.Document)
            {
                if (!SendChildOrderResponseConverter.TryConvert(json.Root, out var candidate, out error))
                {
                    return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.SendChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: protocolCall);
                }

                if (!SendChildOrderMeaningValidator.TryValidate(candidate!, out var response, out error))
                {
                    return NativeCallFactory.CreateError<Requests.SendChildOrderRequest, Dtos.SendChildOrderResponse>(
                        request,
                        Vocabulary.EndpointIds.SendChildOrder,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: protocolCall);
                }

                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.SendChildOrder,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: protocolCall);
            }
        }
    }
}
