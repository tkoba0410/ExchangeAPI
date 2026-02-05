namespace ExchangeApi.Primitives.CallCommon;

public enum CallErrorKind
{
    Transport,
    Http,
    Codec,
    Mapping,
    Semantic,
    Unknown
}
