using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawMarketDataApi
{
    Task<Call<GetTickerRequest, RawMergedResponse>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderBookRequest, RawDepthResponse>> GetOrderBookAsync(
        GetOrderBookRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMarketTradesRequest, RawTradeResponse>> GetTradesAsync(
        GetMarketTradesRequest request,
        CancellationToken cancellationToken = default);
}
