namespace ExchangeApi.Contracts.Common.Dtos;

public enum BatchErrorKind
{
    Transient,
    Permanent,
    Canceled,
    Unknown
}
