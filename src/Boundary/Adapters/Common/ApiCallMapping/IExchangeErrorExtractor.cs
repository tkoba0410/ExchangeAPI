namespace ExchangeApi.Boundary.Adapters.Common.ApiCallMapping;

internal interface IExchangeErrorExtractor<in TErr>
{
    string? TryGetExchangeErrorCode(TErr error);
    string? Summarize(TErr error);
}
