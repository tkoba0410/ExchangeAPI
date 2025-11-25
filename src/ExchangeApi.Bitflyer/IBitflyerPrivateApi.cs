using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer Private REST API への Raw アクセスインターフェース。
/// Stage2 では /v1/me/getbalance のみ対象とする。
/// </summary>
public interface IBitflyerPrivateApi
{
    Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default);
}
