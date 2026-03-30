namespace ExchangeApi.Adapters.McpServer.Mapping;

public sealed record BitflyerMarginRule(
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
    string RequireCollateralModel,
    string RequireCollateralModelSourceKind,
    string RequireCollateralModelSourceRef,
    string MaintenanceModel,
    string MaintenanceModelSourceKind,
    string MaintenanceModelSourceRef,
    string MinimumKeepRate,
    string FeeModel,
    string FeeModelSourceKind,
    string FeeModelSourceRef,
    string SourceNote);
