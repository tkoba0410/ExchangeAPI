namespace ExchangeApi.Shared.Adapter.ApiCallMapping;

internal interface IExchangeErrorExtractor<in TErr>
{
    string? TryGetExchangeErrorCode(TErr error);
    string? Summarize(TErr error);
}
