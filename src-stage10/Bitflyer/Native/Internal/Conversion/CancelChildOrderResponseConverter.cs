using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;

internal static class CancelChildOrderResponseConverter
{
    public static bool TryConvert(
        JsonElement root,
        out CancelChildOrderResponseCandidate? candidate,
        out CallError? error)
    {
        candidate = new CancelChildOrderResponseCandidate();
        error = null;
        return true;
    }
}

internal sealed class CancelChildOrderResponseCandidate
{
}
