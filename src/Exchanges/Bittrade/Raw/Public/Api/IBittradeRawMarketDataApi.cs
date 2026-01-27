using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using BittradeRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

internal interface IBittradeRawMarketDataApi
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
