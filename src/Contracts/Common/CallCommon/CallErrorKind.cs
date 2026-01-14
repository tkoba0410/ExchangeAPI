namespace ExchangeApi.Contracts.Common.CallCommon;

public enum CallErrorKind
{
    Transport,
    Http,
    Codec,
    Mapping,
    Semantic,
    Unknown
}
