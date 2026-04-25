using System.Text.Json;
using ExchangeApi.Optional.Credentials.PlainText;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.AgeFile;

internal static class AgeFileCredentialJsonParser
{
    public static IApiCredentialSession ParseSession(string decryptedJson, string expectedVenue)
    {
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(decryptedJson);
        }
        catch (JsonException ex)
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.JsonParseFailed, "Credentials JSON could not be parsed.", ex);
        }

        using (document)
        {
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new ApiCredentialException(ApiCredentialErrorKind.JsonParseFailed, "Credentials JSON must be an object.");
            }

            var root = document.RootElement;
            var version = ReadRequiredInt(root, "version");
            if (version != 1)
            {
                throw new ApiCredentialException(ApiCredentialErrorKind.UnsupportedVersion, "Credentials JSON version is unsupported.");
            }

            var venue = ReadRequiredString(root, "venue", ApiCredentialErrorKind.MissingRequiredField);
            if (!string.Equals(venue, expectedVenue, StringComparison.Ordinal))
            {
                throw new ApiCredentialException(ApiCredentialErrorKind.VenueMismatch, "Credentials JSON venue does not match provider venue.");
            }

            var apiKey = ReadRequiredString(root, "apiKey", ApiCredentialErrorKind.InvalidApiKey);
            var apiSecret = ReadRequiredString(root, "apiSecret", ApiCredentialErrorKind.InvalidApiSecret);

            return new PlainTextApiCredentialSession(apiKey, apiSecret);
        }
    }

    private static int ReadRequiredInt(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var result))
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.MissingRequiredField, $"Credentials JSON must contain integer {propertyName}.");
        }

        return result;
    }

    private static string ReadRequiredString(JsonElement root, string propertyName, ApiCredentialErrorKind invalidKind)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.String)
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.MissingRequiredField, $"Credentials JSON must contain string {propertyName}.");
        }

        var result = value.GetString();
        if (string.IsNullOrWhiteSpace(result) || result != result.Trim())
        {
            throw new ApiCredentialException(invalidKind, $"Credentials JSON {propertyName} is invalid.");
        }

        return result;
    }
}
