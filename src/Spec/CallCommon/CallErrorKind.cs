namespace ExchangeApi.Spec.CallCommon;

public enum CallErrorKind
{
    Transport,
    Http,
    Codec,
    Mapping,
    Semantic,
    Unknown
}
