using ExchangeApi.Common.Enums;

namespace ExchangeApi.Contracts.Interfaces;

public interface IExchangeClient
{
    ExchangeCode ExchangeCode { get; }
}
