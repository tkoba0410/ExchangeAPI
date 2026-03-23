using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;

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
