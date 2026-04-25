using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;

public interface IGetMarketsNativeEndpoint
{
    Task<CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> CallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetMarketsNativeEndpoint : IGetMarketsNativeEndpoint
{
    private readonly IGetMarketsProtocolEndpoint _protocolEndpoint;

    public GetMarketsNativeEndpoint(IGetMarketsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> CallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetMarkets,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetMarkets,
                "Public",
                "None");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetMarkets.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetMarkets.Item
                {
                    ProductCode = JsonValueReader.ReadRequiredString(item, "product_code"),
                    MarketType = JsonValueReader.ReadRequiredEnum<BitflyerMarketType>(item, "market_type"),
                });
            }

            return NativeCallFactory.Success<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>(request, items, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetMarkets,
                "Public",
                "None");
        }
    }
}
