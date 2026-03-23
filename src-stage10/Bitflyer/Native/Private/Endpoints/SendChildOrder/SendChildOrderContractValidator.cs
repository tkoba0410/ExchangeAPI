using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.SendChildOrder;

internal static class SendChildOrderContractValidator
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
