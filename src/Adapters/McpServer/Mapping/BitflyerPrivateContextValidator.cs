using ExchangeApi.Adapters.McpServer.Schema;

namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerPrivateContextValidator
{
    public static bool TryNormalize(
        string venue,
        string accountContext,
        out string normalizedVenue,
        out string normalizedAccountContext,
        out McpToolError? error)
    {
        normalizedVenue = venue.Trim().ToLowerInvariant();
        if (!string.Equals(normalizedVenue, McpVenueIds.Bitflyer, StringComparison.Ordinal))
        {
            normalizedAccountContext = string.Empty;
            error = ValidationError(
                "invalid_venue",
                "Venue must be bitflyer for this tool.",
                new Dictionary<string, string?> { ["venue"] = venue });
            return false;
        }

        normalizedAccountContext = accountContext.Trim().ToLowerInvariant();
        if (!string.Equals(normalizedAccountContext, McpAccountContextIds.Default, StringComparison.Ordinal))
        {
            error = ValidationError(
                "invalid_account_context",
                "Account context must be default for this tool.",
                new Dictionary<string, string?> { ["accountContext"] = accountContext });
            return false;
        }

        error = null;
        return true;
    }

    private static McpToolError ValidationError(
        string errorCode,
        string message,
        IReadOnlyDictionary<string, string?> details)
    {
        return new McpToolError
        {
            ErrorCategory = "validation_error",
            ErrorCode = errorCode,
            Message = message,
            Details = details,
            Retryable = false,
        };
    }
}
