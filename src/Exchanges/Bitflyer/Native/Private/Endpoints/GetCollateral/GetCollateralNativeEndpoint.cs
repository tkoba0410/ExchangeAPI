using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;

public interface IGetCollateralNativeEndpoint
{
    Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsync(request, cancellationToken);
    }
}

public sealed class GetCollateralNativeEndpoint : IGetCollateralNativeEndpoint
{
    private readonly IGetCollateralProtocolEndpoint _protocolEndpoint;

    public GetCollateralNativeEndpoint(IGetCollateralProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, null, cancellationToken);
    }

    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return CallAsyncCore(request, credentialSession, cancellationToken);
    }

    private async Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsyncCore(
        GetCollateralRequest request,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var protocolCall = await (credentialSession is null
            ? _protocolEndpoint.SendAsync(cancellationToken)
            : _protocolEndpoint.SendAsync(credentialSession, cancellationToken));
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetCollateralRequest, GetCollateralResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetCollateral,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetCollateralRequest, GetCollateralResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetCollateral,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            var response = new GetCollateralResponse
            {
                Collateral = JsonValueReader.ReadRequiredDecimal(root, "collateral"),
                OpenPositionPnl = JsonValueReader.ReadRequiredDecimal(root, "open_position_pnl"),
                RequireCollateral = JsonValueReader.ReadRequiredDecimal(root, "require_collateral"),
                KeepRate = JsonValueReader.ReadRequiredDecimal(root, "keep_rate"),
                MarginCallAmount = ReadOptionalDecimal(root, "margin_call_amount"),
                MarginCallDueDate = JsonValueReader.ReadOptionalUtcTimestamp(root, "margin_call_due_date"),
            };

            return NativeCallFactory.Success(request, response, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetCollateralRequest, GetCollateralResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetCollateral,
                "Private",
                "KeySecret");
        }
    }

    private static decimal? ReadOptionalDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetDecimal(out var value))
        {
            throw new CodecException($"Property '{propertyName}' must be a decimal number.");
        }

        return value;
    }
}
