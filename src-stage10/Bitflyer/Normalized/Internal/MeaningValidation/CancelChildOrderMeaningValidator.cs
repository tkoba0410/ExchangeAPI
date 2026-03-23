using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.MeaningValidation;

internal static class CancelChildOrderMeaningValidator
{
    public static bool TryValidate(
        CancelChildOrderResponseCandidate candidate,
        out CancelChildOrderResponse? response,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        response = new CancelChildOrderResponse();
        error = null;
        return true;
    }
}
