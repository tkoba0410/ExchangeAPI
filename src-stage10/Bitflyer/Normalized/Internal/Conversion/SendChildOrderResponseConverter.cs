using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;

internal static class SendChildOrderResponseConverter
{
    public static bool TryConvert(
        JsonElement root,
        out SendChildOrderResponseCandidate? candidate,
        out CallError? error)
    {
        if (!JsonScalarReader.TryReadString(root, "child_order_acceptance_id", out var childOrderAcceptanceId, out error))
        {
            candidate = null;
            return false;
        }

        candidate = new SendChildOrderResponseCandidate
        {
            ChildOrderAcceptanceId = childOrderAcceptanceId,
        };
        error = null;
        return true;
    }
}

internal sealed class SendChildOrderResponseCandidate
{
    public string? ChildOrderAcceptanceId { get; init; }
}
