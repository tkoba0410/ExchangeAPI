using System.Security.Cryptography;
using System.Text;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.PlainText;

internal sealed class PlainTextApiCredentialSession : IApiCredentialSession
{
    private readonly string _apiSecret;

    public PlainTextApiCredentialSession(string apiKey, string apiSecret)
    {
        ApiKey = ValidateSecretPart(apiKey, ApiCredentialErrorKind.InvalidApiKey, "apiKey");
        _apiSecret = ValidateSecretPart(apiSecret, ApiCredentialErrorKind.InvalidApiSecret, "apiSecret");
    }

    public string ApiKey { get; }

    public string Sign(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static string ValidateSecretPart(string value, ApiCredentialErrorKind kind, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value != value.Trim())
        {
            throw new ApiCredentialException(kind, $"{fieldName} is invalid.");
        }

        return value;
    }
}
