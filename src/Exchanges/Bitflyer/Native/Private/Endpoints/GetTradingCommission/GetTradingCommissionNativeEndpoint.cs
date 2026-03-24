using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;

public interface IGetTradingCommissionNativeEndpoint
{
    Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> CallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetTradingCommissionNativeEndpoint : IGetTradingCommissionNativeEndpoint
{
    private readonly IGetTradingCommissionProtocolEndpoint _protocolEndpoint;

    public GetTradingCommissionNativeEndpoint(IGetTradingCommissionProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> CallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetTradingCommissionRequest, GetTradingCommissionResponse>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetTradingCommission,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetTradingCommissionRequest, GetTradingCommissionResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetTradingCommission,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetTradingCommissionRequest, GetTradingCommissionResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetTradingCommission,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetTradingCommissionResponse
            {
                CommissionRate = JsonValueReader.ReadRequiredDecimal(root, "commission_rate"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetTradingCommissionRequest, GetTradingCommissionResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetTradingCommission,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetTradingCommissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = "ProductCode is required.",
            };
        }

        return null;
    }
}
