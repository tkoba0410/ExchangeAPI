using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;

public interface IGetPositionsNativeEndpoint
{
    Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> CallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetPositionsNativeEndpoint : IGetPositionsNativeEndpoint
{
    private readonly IGetPositionsProtocolEndpoint _protocolEndpoint;

    public GetPositionsNativeEndpoint(IGetPositionsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> CallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetPositions,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(request.ProductCode, cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetPositions,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetPositions,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetPositions.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetPositions.Item
                {
                    ProductCode = JsonValueReader.ReadRequiredString(item, "product_code"),
                    Side = JsonValueReader.ReadRequiredEnum<BitflyerOrderSide>(item, "side"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                    Commission = JsonValueReader.ReadRequiredDecimal(item, "commission"),
                    SwapPointAccumulate = JsonValueReader.ReadRequiredDecimal(item, "swap_point_accumulate"),
                    RequireCollateral = JsonValueReader.ReadRequiredDecimal(item, "require_collateral"),
                    OpenDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "open_date"),
                    Leverage = JsonValueReader.ReadRequiredDecimal(item, "leverage"),
                    Pnl = JsonValueReader.ReadRequiredDecimal(item, "pnl"),
                    Sfd = JsonValueReader.ReadRequiredDecimal(item, "sfd"),
                });
            }

            return NativeCallFactory.Success<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetPositions,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetPositionsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "ProductCode is required." };
        }

        if (!string.Equals(request.ProductCode, ProductCodes.FxBtcJpy, StringComparison.Ordinal))
        {
            return new CallError
            {
                Kind = CallErrorKinds.Semantic,
                Message = $"ProductCode must be {ProductCodes.FxBtcJpy}.",
            };
        }

        return null;
    }
}
