using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Contracts.Interfaces;

public interface ISpotHistoryApi
{
    Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default);
}
