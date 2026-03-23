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
    public sealed class GetBalanceRequest
    {
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos
{
    public static class GetBalance
    {
        public sealed class Item
        {
            [JsonPropertyName("currency_code")]
            public string CurrencyCode { get; init; } = string.Empty;

            [JsonPropertyName("amount")]
            public decimal Amount { get; init; }

            [JsonPropertyName("available")]
            public decimal Available { get; init; }
        }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Api
{
    public partial interface IBitflyerPrivateNativeApi
    {
        Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> GetBalanceAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNativeApi
    {
        public async Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> GetBalanceAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!GetBalanceRequestEncoder.TryEncode(request, out var error))
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var protocolCall = await _protocol
                .GetBalanceAsync(cancellationToken)
                .ConfigureAwait(false);

            if (protocolCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: protocolCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)protocolCall.Result).Response;

            if (!ProtocolJsonValidator.TryValidateArrayResponse(wireResponse, out var json, out error))
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            using (json.Document)
            {
                if (!GetBalanceResponseConverter.TryConvert(json.Root, out var candidates, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: protocolCall);
                }

                if (!GetBalanceMeaningValidator.TryValidate(candidates!, out var response, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: protocolCall);
                }

                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: protocolCall);
            }
        }
    }
}
