using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;

public interface IWithdrawNativeEndpoint
{
    Task<Call<WithdrawRequest, WithdrawResponse>> CallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class WithdrawNativeEndpoint : IWithdrawNativeEndpoint
{
    private readonly IWithdrawProtocolEndpoint _protocolEndpoint;

    public WithdrawNativeEndpoint(IWithdrawProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<WithdrawRequest, WithdrawResponse>> CallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return NativeCallFactory.Failure<WithdrawRequest, WithdrawResponse>(
                request,
                validationError,
                protocolCall: null,
                endpointId: BitflyerEndpointIds.Withdraw,
                scope: "Private",
                auth: "KeySecret");
        }

        var protocolCall = await _protocolEndpoint.SendAsync(JsonSerializer.Serialize(request), cancellationToken);
        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<WithdrawRequest, WithdrawResponse>(
                request,
                protocolCall.Error!,
                protocolCall,
                BitflyerEndpointIds.Withdraw,
                "Private",
                "KeySecret");
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<WithdrawRequest, WithdrawResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Http, Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}." },
                protocolCall,
                BitflyerEndpointIds.Withdraw,
                "Private",
                "KeySecret");
        }

        try
        {
            var root = JsonValueReader.EnsureObject(protocolCall.Response.BodyText);
            if (root.TryGetProperty("message_id", out var messageId))
            {
                var response = new WithdrawResponse
                {
                    MessageId = messageId.ValueKind == JsonValueKind.String
                        ? messageId.GetString() ?? throw new CodecException("Property 'message_id' must not be null.")
                        : throw new CodecException("Property 'message_id' must be a string."),
                };

                return NativeCallFactory.Success(request, response, protocolCall, "Private");
            }

            if (root.TryGetProperty("status", out var statusProperty) &&
                statusProperty.ValueKind == JsonValueKind.Number &&
                statusProperty.TryGetInt64(out var status) &&
                status < 0)
            {
                var errorMessage = JsonValueReader.ReadOptionalString(root, "error_message") ?? $"Venue returned error status {status}.";
                return NativeCallFactory.Failure<WithdrawRequest, WithdrawResponse>(
                    request,
                    new CallError { Kind = CallErrorKinds.Semantic, Message = errorMessage },
                    protocolCall,
                    BitflyerEndpointIds.Withdraw,
                    "Private",
                    "KeySecret");
            }

            throw new CodecException("Response must contain 'message_id' or a negative 'status'.");
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<WithdrawRequest, WithdrawResponse>(
                request,
                new CallError { Kind = CallErrorKinds.Codec, Message = ex.Message },
                protocolCall,
                BitflyerEndpointIds.Withdraw,
                "Private",
                "KeySecret");
        }
    }

    private static CallError? Validate(WithdrawRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrencyCode))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "CurrencyCode is required." };
        }

        if (!string.Equals(request.CurrencyCode, "JPY", StringComparison.Ordinal))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "CurrencyCode must be JPY." };
        }

        if (request.BankAccountId <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "BankAccountId must be greater than zero." };
        }

        if (request.Amount <= 0)
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Amount must be greater than zero." };
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return new CallError { Kind = CallErrorKinds.Semantic, Message = "Code is required." };
        }

        return null;
    }
}
