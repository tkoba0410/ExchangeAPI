using System.Text.Json;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Errors;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;

internal static class GetBalanceResponseConverter
{
    public static bool TryConvert(
        JsonElement root,
        out IReadOnlyList<GetBalanceItemCandidate>? candidates,
        out CallError? error)
    {
        var items = new List<GetBalanceItemCandidate>();

        foreach (var element in root.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                candidates = null;
                error = BitflyerErrorFactory.Mapping("Each balance item must be a JSON object.");
                return false;
            }

            if (!JsonScalarReader.TryReadString(element, "currency_code", out var currencyCode, out error) ||
                !JsonScalarReader.TryReadDecimal(element, "amount", out var amount, out error) ||
                !JsonScalarReader.TryReadDecimal(element, "available", out var available, out error))
            {
                candidates = null;
                return false;
            }

            items.Add(new GetBalanceItemCandidate
            {
                CurrencyCode = currencyCode,
                Amount = amount,
                Available = available,
            });
        }

        candidates = items;
        error = null;
        return true;
    }
}

internal sealed class GetBalanceItemCandidate
{
    public string? CurrencyCode { get; init; }

    public decimal? Amount { get; init; }

    public decimal? Available { get; init; }
}
