using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.ContractValidation;

internal static class CancelChildOrderContractValidator
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
