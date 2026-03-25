using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;

public interface IGetTickerNativeEndpoint
{
    Task<Call<GetTickerRequest, GetTickerResponse>> CallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetTickerNativeEndpoint : IGetTickerNativeEndpoint
{
    private readonly IGetTickerProtocolEndpoint _protocolEndpoint;

    public GetTickerNativeEndpoint(IGetTickerProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetTickerRequest, GetTickerResponse>> CallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<GetTickerRequest, GetTickerResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode must not be blank." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetTicker,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetTickerRequest, GetTickerResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetTicker,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetTickerRequest, GetTickerResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetTicker,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetTickerResponse
            {
                ProductCode = JsonValueReader.ReadRequiredString(root, "product_code"),
                State = JsonValueReader.ReadRequiredString(root, "state"),
                Timestamp = JsonValueReader.ReadRequiredUtcTimestamp(root, "timestamp"),
                TickId = JsonValueReader.ReadRequiredLong(root, "tick_id"),
                BestBid = JsonValueReader.ReadRequiredDecimal(root, "best_bid"),
                BestAsk = JsonValueReader.ReadRequiredDecimal(root, "best_ask"),
                BestBidSize = JsonValueReader.ReadRequiredDecimal(root, "best_bid_size"),
                BestAskSize = JsonValueReader.ReadRequiredDecimal(root, "best_ask_size"),
                TotalBidDepth = JsonValueReader.ReadRequiredDecimal(root, "total_bid_depth"),
                TotalAskDepth = JsonValueReader.ReadRequiredDecimal(root, "total_ask_depth"),
                MarketBidSize = JsonValueReader.ReadRequiredDecimal(root, "market_bid_size"),
                MarketAskSize = JsonValueReader.ReadRequiredDecimal(root, "market_ask_size"),
                Ltp = JsonValueReader.ReadRequiredDecimal(root, "ltp"),
                Volume = JsonValueReader.ReadRequiredDecimal(root, "volume"),
                VolumeByProduct = JsonValueReader.ReadRequiredDecimal(root, "volume_by_product"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetTickerRequest, GetTickerResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetTicker,
                "Public",
                "None");
        }
    }
}
