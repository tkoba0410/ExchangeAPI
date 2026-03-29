using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;

public interface IGetBoardStateNativeEndpoint
{
    Task<Call<GetBoardStateRequest, GetBoardStateResponse>> CallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetBoardStateNativeEndpoint : IGetBoardStateNativeEndpoint
{
    private readonly IGetBoardStateProtocolEndpoint _protocolEndpoint;

    public GetBoardStateNativeEndpoint(IGetBoardStateProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetBoardStateRequest, GetBoardStateResponse>> CallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ProductCode is not null && string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<GetBoardStateRequest, GetBoardStateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode must not be blank." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetBoardState,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetBoardStateRequest, GetBoardStateResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetBoardState,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetBoardStateRequest, GetBoardStateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetBoardState,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            GetBoardStateData? data = null;
            if (root.TryGetProperty("data", out var dataProperty) && dataProperty.ValueKind != System.Text.Json.JsonValueKind.Null)
            {
                if (dataProperty.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Property 'data' must be an object.");
                }

                data = new GetBoardStateData
                {
                    SpecialQuotation = dataProperty.TryGetProperty("special_quotation", out var specialQuotation)
                        ? JsonValueReader.ReadDecimal(specialQuotation, "data.special_quotation")
                        : null,
                };
            }

            var response = new GetBoardStateResponse
            {
                Health = JsonValueReader.ReadRequiredEnum<BitflyerHealthStatus>(root, "health"),
                State = JsonValueReader.ReadRequiredEnum<BitflyerTradingState>(root, "state"),
                Data = data,
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetBoardStateRequest, GetBoardStateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetBoardState,
                "Public",
                "None");
        }
    }
}
