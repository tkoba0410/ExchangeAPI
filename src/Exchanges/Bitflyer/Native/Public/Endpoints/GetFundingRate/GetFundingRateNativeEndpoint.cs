using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;

public interface IGetFundingRateNativeEndpoint
{
    Task<Call<GetFundingRateRequest, GetFundingRateResponse>> CallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetFundingRateNativeEndpoint : IGetFundingRateNativeEndpoint
{
    private readonly IGetFundingRateProtocolEndpoint _protocolEndpoint;

    public GetFundingRateNativeEndpoint(IGetFundingRateProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetFundingRateRequest, GetFundingRateResponse>> CallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return NativeCallFactory.Failure<GetFundingRateRequest, GetFundingRateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." },
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetFundingRate,
                scope: "Public",
                auth: "None");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetFundingRateRequest, GetFundingRateResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetFundingRate,
                "Public",
                "None");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetFundingRateRequest, GetFundingRateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetFundingRate,
                "Public",
                "None");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetFundingRateResponse
            {
                CurrentFundingRate = JsonValueReader.ReadRequiredDecimal(root, "current_funding_rate"),
                NextFundingRateSettleDate = JsonValueReader.ReadRequiredUtcTimestamp(root, "next_funding_rate_settledate"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Public");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetFundingRateRequest, GetFundingRateResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetFundingRate,
                "Public",
                "None");
        }
    }
}
