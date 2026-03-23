using System.Text.Json;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.ContractValidation;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalance;
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

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.GetBalance
{
    public interface IGetBalanceNativeEndpoint
    {
        Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> CallAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed class GetBalanceNativeEndpoint : IGetBalanceNativeEndpoint
    {
        private static readonly string[] RequiredProperties = ["currency_code", "amount", "available"];

        private readonly IGetBalanceProtocolEndpoint _protocol;

        public GetBalanceNativeEndpoint(IGetBalanceProtocolEndpoint protocol)
        {
            _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        }

        public async Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> CallAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!GetBalanceRequestEncoder.TryEncode(request, out var error))
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateEndpointModule",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "InputValidation");
            }

            var protocolCall = await _protocol
                .SendAsync(cancellationToken)
                .ConfigureAwait(false);

            if (protocolCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateEndpointModule",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: protocolCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)protocolCall.Result).Response;

            if (!ProtocolJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateEndpointModule",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            if (!ProtocolJsonValidator.TryValidateArrayResponse(wireResponse, out var json, out error))
            {
                return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateEndpointModule",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            using (json.Document)
            {
                if (!TryValidateRawArrayContract(json.Root, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateEndpointModule",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "JsonValidation",
                        child: protocolCall);
                }

                if (!GetBalanceResponseConverter.TryConvert(json.Root, out var candidates, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateEndpointModule",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: protocolCall);
                }

                if (!GetBalanceContractValidator.TryValidate(candidates!, out var response, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateEndpointModule",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "ContractValidation",
                        child: protocolCall);
                }

                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateEndpointModule",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: protocolCall);
            }
        }

        private static bool TryValidateRawArrayContract(JsonElement root, out CallError? error)
        {
            var index = 0;
            foreach (var element in root.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object)
                {
                    error = BitflyerErrorFactory.Codec(
                        $"GetBalance response item at index {index} must be a JSON object.");
                    return false;
                }

                if (!ProtocolJsonValidator.TryValidateRequiredProperties(element, RequiredProperties, out error))
                {
                    return false;
                }

                index++;
            }

            error = null;
            return true;
        }
    }
}
