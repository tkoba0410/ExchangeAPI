using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

public interface IGetBalanceHistoryNativeEndpoint
{
    Task<Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> CallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetBalanceHistoryNativeEndpoint : IGetBalanceHistoryNativeEndpoint
{
    private readonly IGetBalanceHistoryProtocolEndpoint _protocolEndpoint;

    public GetBalanceHistoryNativeEndpoint(IGetBalanceHistoryProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> CallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.GetBalanceHistory,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.CurrencyCode,
            request.Count,
            request.Before,
            request.After,
            cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetBalanceHistory,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetBalanceHistory,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetBalanceHistory.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetBalanceHistory.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    TradeDate = JsonValueReader.ReadRequiredTimestamp(item, "trade_date"),
                    EventDate = JsonValueReader.ReadRequiredTimestamp(item, "event_date"),
                    ProductCode = JsonValueReader.ReadOptionalString(item, "product_code"),
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    TradeType = JsonValueReader.ReadRequiredString(item, "trade_type"),
                    Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    Quantity = JsonValueReader.ReadRequiredDecimal(item, "quantity"),
                    Commission = JsonValueReader.ReadRequiredDecimal(item, "commission"),
                    Balance = JsonValueReader.ReadRequiredDecimal(item, "balance"),
                    OrderId = JsonValueReader.ReadOptionalString(item, "order_id"),
                });
            }

            return NativeCallFactory.Success<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetBalanceHistory,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(GetBalanceHistoryRequest request)
    {
        if (request.CurrencyCode is not null && string.IsNullOrWhiteSpace(request.CurrencyCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "CurrencyCode must not be blank." };
        }

        if (request.Count is not null && request.Count <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Count must be greater than zero." };
        }

        if (request.Before is not null && request.Before <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Before must be greater than zero." };
        }

        if (request.After is not null && request.After <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "After must be greater than zero." };
        }

        return null;
    }
}
