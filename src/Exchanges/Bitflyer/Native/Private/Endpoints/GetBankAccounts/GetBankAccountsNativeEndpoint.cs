using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;

public interface IGetBankAccountsNativeEndpoint
{
    Task<Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> CallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetBankAccountsNativeEndpoint : IGetBankAccountsNativeEndpoint
{
    private readonly IGetBankAccountsProtocolEndpoint _protocolEndpoint;

    public GetBankAccountsNativeEndpoint(IGetBankAccountsProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> CallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default)
    {
        var protocolCall = await _protocolEndpoint.SendAsync(cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.GetBankAccounts,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.GetBankAccounts,
                "Private",
                "KeySecret");
        }

        try
        {
            var array = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var items = new List<GetBankAccounts.Item>();
            foreach (var item in array.EnumerateArray())
            {
                if (item.ValueKind != System.Text.Json.JsonValueKind.Object)
                {
                    throw new CodecException("Array item must be an object.");
                }

                items.Add(new GetBankAccounts.Item
                {
                    Id = JsonValueReader.ReadRequiredLong(item, "id"),
                    IsVerified = JsonValueReader.ReadRequiredBoolean(item, "is_verified"),
                    BankName = JsonValueReader.ReadRequiredString(item, "bank_name"),
                    BranchName = JsonValueReader.ReadRequiredString(item, "branch_name"),
                    AccountType = JsonValueReader.ReadRequiredString(item, "account_type"),
                    AccountNumber = JsonValueReader.ReadRequiredString(item, "account_number"),
                    AccountName = JsonValueReader.ReadRequiredString(item, "account_name"),
                });
            }

            return NativeCallFactory.Success<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>(request, items, protocolCall, "Private");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.GetBankAccounts,
                "Private",
                "KeySecret");
        }
    }
}
