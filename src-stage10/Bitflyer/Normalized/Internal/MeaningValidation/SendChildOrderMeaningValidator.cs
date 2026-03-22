using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.MeaningValidation;

internal static class SendChildOrderMeaningValidator
{
    public static bool TryValidate(
        SendChildOrderResponseCandidate candidate,
        out SendChildOrderResponse? response,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (string.IsNullOrWhiteSpace(candidate.ChildOrderAcceptanceId))
        {
            response = null;
            error = BitflyerErrorFactory.Semantic("ChildOrderAcceptanceId is required in SendChildOrder response.");
            return false;
        }

        response = new SendChildOrderResponse
        {
            ChildOrderAcceptanceId = candidate.ChildOrderAcceptanceId,
        };
        error = null;
        return true;
    }
}
