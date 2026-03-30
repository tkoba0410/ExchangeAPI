namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class MarketRuleSourceKinds
{
    public const string OfficialDocumented = "official_documented";
    public const string OfficialApiContract = "official_api_contract";
    public const string AdapterInferred = "adapter_inferred";
    public const string PinnedOperational = "pinned_operational";

    public static bool IsDefined(string value)
    {
        return value is OfficialDocumented or OfficialApiContract or AdapterInferred or PinnedOperational;
    }
}
