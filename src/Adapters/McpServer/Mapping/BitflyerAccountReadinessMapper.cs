namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerAccountReadinessMapper
{
    private static readonly string[] RequiredReadPermissions =
    [
        "/v1/me/getpermissions",
        "/v1/me/getbalance",
        "/v1/me/getcollateral",
        "/v1/me/getchildorders",
        "/v1/me/getpositions",
    ];

    public static IReadOnlyList<string> RequiredPermissions => RequiredReadPermissions;

    public static string Map(IReadOnlyCollection<string>? permissions)
    {
        if (permissions is null)
        {
            return "unknown";
        }

        var granted = new HashSet<string>(permissions, StringComparer.Ordinal);
        return RequiredReadPermissions.All(granted.Contains)
            ? "ready"
            : "restricted";
    }
}
