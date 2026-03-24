using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;

public interface IGetHealthNativeEndpoint
{
    Task<Call<GetHealthRequest, GetHealthResponse>> CallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetHealthNativeEndpoint : IGetHealthNativeEndpoint
{
    private readonly IGetHealthProtocolEndpoint _protocolEndpoint;

    public GetHealthNativeEndpoint(IGetHealthProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetHealthRequest, GetHealthResponse>> CallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<GetHealthRequest, GetHealthResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode must not be blank." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetHealth,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetHealthRequest, GetHealthResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetHealth,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetHealthRequest, GetHealthResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetHealth,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetHealthResponse
            {
                Status = JsonValueReader.ReadRequiredString(root, "status"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetHealthRequest, GetHealthResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetHealth,
                "Public",
                "None");
        }
    }
}
