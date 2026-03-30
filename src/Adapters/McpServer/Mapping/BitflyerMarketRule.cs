namespace ExchangeApi.Adapters.McpServer.Mapping;

public sealed record BitflyerMarketRule(
    string Symbol,
    string MinSize,
    string SizeStep,
    string PriceStep,
    string MinSizeSourceKind,
    string SizeStepSourceKind,
    string PriceStepSourceKind,
    string SourceNote);
