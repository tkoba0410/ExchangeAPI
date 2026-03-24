using System.Globalization;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;

public interface IGetCollateralNativeEndpoint
{
    Task<Call<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetCollateralNativeEndpoint : IGetCollateralNativeEndpoint
{
    private readonly IGetCollateralProtocolEndpoint _protocolEndpoint;

    public GetCollateralNativeEndpoint(IGetCollateralProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetCollateralRequest, GetCollateralResponse>> CallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
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
                MarginCallDueDate = ReadOptionalTimestamp(root, "margin_call_due_date"),
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

    private static DateTimeOffset? ReadOptionalTimestamp(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String)
        {
            throw new CodecException($"Property '{propertyName}' must be a timestamp string.");
        }

        var raw = property.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
            return value;
        }

        throw new CodecException($"Property '{propertyName}' must be a timestamp.");
    }
}
