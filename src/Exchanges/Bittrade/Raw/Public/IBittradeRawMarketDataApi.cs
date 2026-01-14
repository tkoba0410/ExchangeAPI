using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using BittradeRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public;

public interface IBittradeRawMarketDataApi
{
    Task<Call<BittradeRequests.GetTickerRequest, RawMergedResponse>> GetTickerAsync(
        BittradeRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BittradeRequests.GetOrderBookRequest, RawDepthResponse>> GetOrderBookAsync(
        BittradeRequests.GetOrderBookRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BittradeRequests.GetMarketTradesRequest, RawTradeResponse>> GetTradesAsync(
        BittradeRequests.GetMarketTradesRequest request,
        CancellationToken cancellationToken = default);
}
