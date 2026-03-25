using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;

public interface IGetCorporateLeverageNativeEndpoint
{
    Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> CallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetCorporateLeverageNativeEndpoint : IGetCorporateLeverageNativeEndpoint
{
    private readonly IGetCorporateLeverageProtocolEndpoint _protocolEndpoint;

    public GetCorporateLeverageNativeEndpoint(IGetCorporateLeverageProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> CallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetCorporateLeverageRequest, GetCorporateLeverageResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetCorporateLeverage,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetCorporateLeverageRequest, GetCorporateLeverageResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetCorporateLeverage,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetCorporateLeverageResponse
            {
                CurrentMax = JsonValueReader.ReadRequiredDecimal(root, "current_max"),
                CurrentStartDate = JsonValueReader.ReadRequiredUtcTimestamp(root, "current_startdate"),
                NextMax = root.TryGetProperty("next_max", out var nextMax) ? JsonValueReader.ReadDecimal(nextMax, "next_max") : null,
                NextStartDate = JsonValueReader.ReadOptionalUtcTimestamp(root, "next_startdate"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetCorporateLeverageRequest, GetCorporateLeverageResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetCorporateLeverage,
                "Public",
                "None");
        }
    }
}
