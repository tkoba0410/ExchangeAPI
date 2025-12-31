using System;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Internal;

internal sealed class BittradeNormalizedException : Exception
{
    public BittradeNormalizedException(string message) : base(message)
    {
    }

    public BittradeNormalizedException(string message, Exception inner) : base(message, inner)
    {
    }
}
