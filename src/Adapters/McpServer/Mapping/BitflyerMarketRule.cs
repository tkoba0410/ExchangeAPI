namespace ExchangeApi.Adapters.McpServer.Mapping;

public sealed record BitflyerMarketRule(
    string Symbol,
    string MinSize,
    string SizeStep,
    string PriceStep,
    string MinSizeSourceKind,
    string MinSizeSourceRef,
    string SizeStepSourceKind,
    string SizeStepSourceRef,
    string PriceStepSourceKind,
    string PriceStepSourceRef,
    string SourceNote);
