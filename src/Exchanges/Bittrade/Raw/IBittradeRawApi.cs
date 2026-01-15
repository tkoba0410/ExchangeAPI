using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    IBittradeRawMarketDataApi MarketData { get; }
    IBittradeRawTradingApi Trading { get; }

    Task<Call<GetRawSymbolsRequest, RawSymbolsResponse>> GetSymbolsAsync(
        GetRawSymbolsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRawTimestampRequest, RawTimestampResponse>> GetTimestampAsync(
        GetRawTimestampRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default);
}
