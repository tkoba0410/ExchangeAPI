using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;

public interface IGetAddressesNativeEndpoint
{
    Task<Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> CallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetAddressesNativeEndpoint : IGetAddressesNativeEndpoint
{
    private readonly IGetAddressesProtocolEndpoint _protocolEndpoint;

    public GetAddressesNativeEndpoint(IGetAddressesProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> CallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetAddresses,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetAddresses,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetAddresses.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetAddresses.Item
                {
                    Type = JsonValueReader.ReadRequiredEnum<BitflyerAddressType>(item, "type"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Address = JsonValueReader.ReadRequiredString(item, "address"),
                });
            }

            return NativeCallFactory.Success<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetAddresses,
                "Private",
                "KeySecret");
        }
    }
}
