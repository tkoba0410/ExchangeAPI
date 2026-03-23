using System.Text.Json.Serialization;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.ContractValidation;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Requests
{
    public sealed class GetTickerRequest
    {
        [JsonPropertyName("product_code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ProductCode { get; init; }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos
{
    public sealed class GetTickerResponse
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; init; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; init; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }

        [JsonPropertyName("tick_id")]
        public long TickId { get; init; }

        [JsonPropertyName("best_bid")]
        public decimal BestBid { get; init; }

        [JsonPropertyName("best_ask")]
        public decimal BestAsk { get; init; }

        [JsonPropertyName("best_bid_size")]
        public decimal BestBidSize { get; init; }

        [JsonPropertyName("best_ask_size")]
        public decimal BestAskSize { get; init; }

        [JsonPropertyName("total_bid_depth")]
        public decimal TotalBidDepth { get; init; }

        [JsonPropertyName("total_ask_depth")]
        public decimal TotalAskDepth { get; init; }

        [JsonPropertyName("market_bid_size")]
        public decimal MarketBidSize { get; init; }

        [JsonPropertyName("market_ask_size")]
        public decimal MarketAskSize { get; init; }

        [JsonPropertyName("ltp")]
        public decimal Ltp { get; init; }

        [JsonPropertyName("volume")]
        public decimal Volume { get; init; }

        [JsonPropertyName("volume_by_product")]
        public decimal VolumeByProduct { get; init; }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Endpoints.GetTicker
{
    public interface IGetTickerNativeEndpoint
    {
        Task<Call<Requests.GetTickerRequest, Dtos.GetTickerResponse>> CallAsync(
            Requests.GetTickerRequest request,
            CancellationToken cancellationToken = default);
    }

    public sealed class GetTickerNativeEndpoint : IGetTickerNativeEndpoint
    {
        private static readonly string[] RequiredProperties =
        [
            "product_code",
            "state",
            "timestamp",
            "tick_id",
            "best_bid",
            "best_ask",
            "best_bid_size",
            "best_ask_size",
            "total_bid_depth",
            "total_ask_depth",
            "market_bid_size",
            "market_ask_size",
            "ltp",
            "volume",
            "volume_by_product",
        ];

        private readonly IGetTickerProtocolEndpoint _protocol;

        public GetTickerNativeEndpoint(IGetTickerProtocolEndpoint protocol)
        {
            _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        }

        public async Task<Call<Requests.GetTickerRequest, Dtos.GetTickerResponse>> CallAsync(
            Requests.GetTickerRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!GetTickerRequestEncoder.TryEncode(request, out var encodedRequest, out var error))
            {
                return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                    request,
                    Vocabulary.EndpointIds.GetTicker,
                    component: "PublicEndpointModule",
                    scope: "Public",
                    auth: "None",
                    error: error!,
                    stage: "InputValidation");
            }

            var protocolCall = await _protocol
                .SendAsync(encodedRequest.ProductCode, cancellationToken)
                .ConfigureAwait(false);

            if (protocolCall.Result is CallResult<WireResponse>.Err wireError)
            {
                return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                    request,
                    Vocabulary.EndpointIds.GetTicker,
                    component: "PublicEndpointModule",
                    scope: "Public",
                    auth: "None",
                    error: wireError.Error,
                    child: protocolCall);
            }

            var wireResponse = ((CallResult<WireResponse>.Ok)protocolCall.Result).Response;

            if (!ProtocolJsonValidator.TryValidateExpectedStatus(wireResponse, 200, out error))
            {
                return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                    request,
                    Vocabulary.EndpointIds.GetTicker,
                    component: "PublicEndpointModule",
                    scope: "Public",
                    auth: "None",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            if (!ProtocolJsonValidator.TryValidateObjectResponse(wireResponse, out var json, out error))
            {
                return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                    request,
                    Vocabulary.EndpointIds.GetTicker,
                    component: "PublicEndpointModule",
                    scope: "Public",
                    auth: "None",
                    error: error!,
                    stage: "JsonValidation",
                    child: protocolCall);
            }

            using (json.Document)
            {
                if (!ProtocolJsonValidator.TryValidateRequiredProperties(json.Root, RequiredProperties, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                        request,
                        Vocabulary.EndpointIds.GetTicker,
                        component: "PublicEndpointModule",
                        scope: "Public",
                        auth: "None",
                        error: error!,
                        stage: "JsonValidation",
                        child: protocolCall);
                }

                if (!GetTickerResponseConverter.TryConvert(json.Root, out var candidate, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                        request,
                        Vocabulary.EndpointIds.GetTicker,
                        component: "PublicEndpointModule",
                        scope: "Public",
                        auth: "None",
                        error: error!,
                        stage: "Conversion",
                        child: protocolCall);
                }

                if (!GetTickerContractValidator.TryValidate(candidate!, out var response, out error))
                {
                    return NativeCallFactory.CreateError<Requests.GetTickerRequest, Dtos.GetTickerResponse>(
                        request,
                        Vocabulary.EndpointIds.GetTicker,
                        component: "PublicEndpointModule",
                        scope: "Public",
                        auth: "None",
                        error: error!,
                        stage: "ContractValidation",
                        child: protocolCall);
                }

                return NativeCallFactory.CreateSuccess(
                    request,
                    Vocabulary.EndpointIds.GetTicker,
                    component: "PublicEndpointModule",
                    scope: "Public",
                    auth: "None",
                    response: response!,
                    child: protocolCall);
            }
        }
    }
}
