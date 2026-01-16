using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private;

public interface IBittradeRawAccountApi
{
    Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default);
}
