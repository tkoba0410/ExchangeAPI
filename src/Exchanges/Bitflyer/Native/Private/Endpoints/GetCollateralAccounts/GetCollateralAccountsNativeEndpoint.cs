using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;

public interface IGetCollateralAccountsNativeEndpoint
{
    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> CallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetCollateralAccountsNativeEndpoint : IGetCollateralAccountsNativeEndpoint
{
    private readonly IGetCollateralAccountsProtocolEndpoint _protocolEndpoint;

    public GetCollateralAccountsNativeEndpoint(IGetCollateralAccountsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> CallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetCollateralAccounts,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetCollateralAccounts,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetCollateralAccounts.Item>();

            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetCollateralAccounts.Item
                {
                    CurrencyCode = JsonValueReader.ReadRequiredString(item, "currency_code"),
                    Amount = JsonValueReader.ReadRequiredDecimal(item, "amount"),
                });
            }

            return NativeCallFactory.Success<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetCollateralAccounts,
                "Private",
                "KeySecret");
        }
    }
}
