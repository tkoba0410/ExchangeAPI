using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;

public interface IGetPermissionsNativeEndpoint
{
    Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> CallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetPermissionsNativeEndpoint : IGetPermissionsNativeEndpoint
{
    private readonly IGetPermissionsProtocolEndpoint _protocolEndpoint;

    public GetPermissionsNativeEndpoint(IGetPermissionsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> CallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetPermissionsRequest, IReadOnlyList<string>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetPermissions,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetPermissionsRequest, IReadOnlyList<string>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetPermissions,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<string>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.String)
                {
                    throw new CodecException("Permission item must be a string.");
                }

                items.Add(item.GetString() ?? throw new CodecException("Permission item must not be null."));
            }

            return NativeCallFactory.Success<GetPermissionsRequest, IReadOnlyList<string>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetPermissionsRequest, IReadOnlyList<string>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetPermissions,
                "Private",
                "KeySecret");
        }
    }
}
