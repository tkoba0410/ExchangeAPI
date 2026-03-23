using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Encoder;

internal static class GetBalanceRequestEncoder
{
    public static bool TryEncode(
        GetBalanceRequest request,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(request);

        error = null;
        return true;
    }
}
