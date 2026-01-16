using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public;

public interface IBittradeRawExchangeInfoApi
{
    Task<Call<GetRawSymbolsRequest, RawSymbolsResponse>> GetSymbolsAsync(
        GetRawSymbolsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRawTimestampRequest, RawTimestampResponse>> GetTimestampAsync(
        GetRawTimestampRequest request,
        CancellationToken cancellationToken = default);
}
