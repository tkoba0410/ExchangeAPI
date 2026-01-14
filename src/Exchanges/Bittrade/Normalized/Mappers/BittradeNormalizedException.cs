using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Mappers;

internal sealed class BittradeNormalizedException : Exception
{
    public BittradeNormalizedException(string message) : base(message)
    {
    }

    public BittradeNormalizedException(string message, Exception inner) : base(message, inner)
    {
    }
}
