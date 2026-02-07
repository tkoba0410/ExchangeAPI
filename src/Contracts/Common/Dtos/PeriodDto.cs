namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record PeriodDto(string Code)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Code);
}
