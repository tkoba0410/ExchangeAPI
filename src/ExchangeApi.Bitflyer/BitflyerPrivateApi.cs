using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer.Models;
using ExchangeApi.Infrastructure.Protocol;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer Private REST API の実装。
/// </summary>
public sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getbalance";

        // Stage2 時点ではクエリなし
        return _restClient.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>(
            path,
            query: null,
            cancellationToken);
    }
}
