using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.MeaningValidation;

internal static class GetBalanceMeaningValidator
{
    public static bool TryValidate(
        IReadOnlyList<GetBalanceItemCandidate> candidates,
        out IReadOnlyList<GetBalance.Item>? response,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        var items = new GetBalance.Item[candidates.Count];
        for (var index = 0; index < candidates.Count; index++)
        {
            var candidate = candidates[index];
            if (string.IsNullOrWhiteSpace(candidate.CurrencyCode) ||
                candidate.Amount is null ||
                candidate.Available is null)
            {
                response = null;
                error = BitflyerErrorFactory.Semantic(
                    $"GetBalance response item at index {index} is missing one or more required fields.");
                return false;
            }

            items[index] = new GetBalance.Item
            {
                CurrencyCode = candidate.CurrencyCode,
                Amount = candidate.Amount.Value,
                Available = candidate.Available.Value,
            };
        }

        response = items;
        error = null;
        return true;
    }
}
