using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

internal interface IBittradeRawExchangeInfoApi
{
    Task<Call<GetRawSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetRawSymbolsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRawTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetRawTimestampRequest request,
        CancellationToken cancellationToken = default);
}
