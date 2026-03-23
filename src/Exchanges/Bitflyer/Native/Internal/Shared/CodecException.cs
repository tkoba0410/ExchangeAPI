namespace ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;

internal sealed class CodecException : Exception
{
    public CodecException(string message)
        : base(message)
    {
    }
}
