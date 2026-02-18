using System;
using System.Security.Cryptography;
using System.Text;

namespace ExchangeApi.Transport.Observability;

internal static class ExceptionLogSanitizer
{
    internal const string RedactedMessage = "<redacted>";
    private const string ErrorReferencePrefix = "errp_v1_";
    private static readonly byte[] ErrorReferenceKey = CreateErrorReferenceKey();

    public static string CreateErrorReference(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var payload = Encoding.UTF8.GetBytes($"{exception.GetType().FullName}|{exception.Message}");
        using var hmac = new HMACSHA256(ErrorReferenceKey);
        var hash = hmac.ComputeHash(payload);
        return $"{ErrorReferencePrefix}{Convert.ToHexString(hash, 0, 8)}";
    }

    public static string CreateStatusDescription(Exception exception)
    {
        return $"error_ref={CreateErrorReference(exception)}";
    }

    private static byte[] CreateErrorReferenceKey()
    {
        var keyFromEnvironment = Environment.GetEnvironmentVariable("EXCHANGEAPI_LOG_MASK_KEY");
        if (!string.IsNullOrWhiteSpace(keyFromEnvironment))
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(keyFromEnvironment));
        }

        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
