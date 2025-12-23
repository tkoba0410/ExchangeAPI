using ExchangeApi.Common.Enums;

namespace ExchangeApi.Common.Interfaces;

public interface IExchangeClient
{
    ExchangeCode ExchangeCode { get; }
}
