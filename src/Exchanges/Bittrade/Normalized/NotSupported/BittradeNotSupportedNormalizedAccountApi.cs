using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;

internal sealed class BittradeNotSupportedNormalizedAccountApi : IBittradeNormalizedAccountApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.NotSupported";
    private readonly string _accountId;

    public BittradeNotSupportedNormalizedAccountApi(string accountId)
    {
        _accountId = accountId;
    }

    public Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetBalancesCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>(
            Layer,
            Component,
            new GetBalancesRequest(_accountId),
            "Account.GetBalances"));
}
