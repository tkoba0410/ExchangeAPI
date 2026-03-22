using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Conversion;
using ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.MeaningValidation;

internal static class GetTickerMeaningValidator
{
    public static bool TryValidate(
        GetTickerResponseCandidate candidate,
        out GetTickerResponse? response,
        out CallError? error)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (string.IsNullOrWhiteSpace(candidate.ProductCode))
        {
            response = null;
            error = BitflyerErrorFactory.Semantic("ProductCode is required in GetTicker response.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(candidate.State))
        {
            response = null;
            error = BitflyerErrorFactory.Semantic("State is required in GetTicker response.");
            return false;
        }

        if (candidate.Timestamp is null ||
            candidate.TickId is null ||
            candidate.BestBid is null ||
            candidate.BestAsk is null ||
            candidate.BestBidSize is null ||
            candidate.BestAskSize is null ||
            candidate.TotalBidDepth is null ||
            candidate.TotalAskDepth is null ||
            candidate.MarketBidSize is null ||
            candidate.MarketAskSize is null ||
            candidate.Ltp is null ||
            candidate.Volume is null ||
            candidate.VolumeByProduct is null)
        {
            response = null;
            error = BitflyerErrorFactory.Semantic("GetTicker response is missing one or more required fields.");
            return false;
        }

        response = new GetTickerResponse
        {
            ProductCode = candidate.ProductCode,
            State = candidate.State,
            Timestamp = candidate.Timestamp.Value,
            TickId = candidate.TickId.Value,
            BestBid = candidate.BestBid.Value,
            BestAsk = candidate.BestAsk.Value,
            BestBidSize = candidate.BestBidSize.Value,
            BestAskSize = candidate.BestAskSize.Value,
            TotalBidDepth = candidate.TotalBidDepth.Value,
            TotalAskDepth = candidate.TotalAskDepth.Value,
            MarketBidSize = candidate.MarketBidSize.Value,
            MarketAskSize = candidate.MarketAskSize.Value,
            Ltp = candidate.Ltp.Value,
            Volume = candidate.Volume.Value,
            VolumeByProduct = candidate.VolumeByProduct.Value,
        };
        error = null;
        return true;
    }
}
