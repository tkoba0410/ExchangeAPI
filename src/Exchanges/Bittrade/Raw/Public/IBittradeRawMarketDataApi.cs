using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using BittradeRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public;

public interface IBittradeRawMarketDataApi
{
    Task<Call<BittradeRequests.GetTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        BittradeRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BittradeRequests.GetOrderBookRequest, RawDepthResponse>> GetDepthCallAsync(
        BittradeRequests.GetOrderBookRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BittradeRequests.GetMarketTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        BittradeRequests.GetMarketTradesRequest request,
        CancellationToken cancellationToken = default);
}
