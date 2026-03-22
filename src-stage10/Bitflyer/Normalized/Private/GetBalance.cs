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
    public sealed class GetBalanceRequest
    {
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos
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

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api
{
    public partial interface IBitflyerPrivateNormalizedApi
    {
        Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> GetBalanceAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateNormalizedApi
    {
        public async Task<Call<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>> GetBalanceAsync(
            Requests.GetBalanceRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!GetBalanceRequestEncoder.TryEncode(request, out var error))
            {
                return NormalizedCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "Encoder");
            }

            var wireCall = await _wire
                .GetBalanceAsync(cancellationToken)
                .ConfigureAwait(false);

            if (wireCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NormalizedCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: wireError.Error,
                    child: wireCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)wireCall.Result).Response;

            if (!WireJsonValidator.TryValidateArrayResponse(wireResponse, out var json, out error))
            {
                return NormalizedCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    error: error!,
                    stage: "JsonValidation",
                    child: wireCall);
            }

            using (json.Document)
            {
                if (!GetBalanceResponseConverter.TryConvert(json.Root, out var candidates, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "Conversion",
                        child: wireCall);
                }

                if (!GetBalanceMeaningValidator.TryValidate(candidates!, out var response, out error))
                {
                    return NormalizedCallFactory.CreateError<Requests.GetBalanceRequest, IReadOnlyList<Dtos.GetBalance.Item>>(
                        request,
                        Vocabulary.EndpointIds.GetBalance,
                        component: "PrivateApi",
                        scope: "Private",
                        auth: "Required",
                        error: error!,
                        stage: "MeaningValidation",
                        child: wireCall);
                }

                return NormalizedCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.GetBalance,
                    component: "PrivateApi",
                    scope: "Private",
                    auth: "Required",
                    response: response!,
                    child: wireCall);
            }
        }
    }
}
