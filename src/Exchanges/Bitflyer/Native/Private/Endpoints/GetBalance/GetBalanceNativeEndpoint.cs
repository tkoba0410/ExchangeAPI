using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;

public interface IGetBalanceNativeEndpoint
{
    Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> CallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetBalanceNativeEndpoint : IGetBalanceNativeEndpoint
{
    private readonly IGetBalanceProtocolEndpoint _protocolEndpoint;

    public GetBalanceNativeEndpoint(IGetBalanceProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> CallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetBalance,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetBalance,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetBalance.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetBalance.Item
                {
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                    Available = JsonValueReader.ReadRequiredDecimal(item, "available"),
                });
            }

            return NativeCallFactory.Success<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetBalance,
                "Private",
                "KeySecret");
        }
    }
}
