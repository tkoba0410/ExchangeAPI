using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IReadOnlyList<BitflyerBalanceResponse> _response;

    public FakeBitflyerPrivateApi(IReadOnlyList<BitflyerBalanceResponse> response)
    {
        _response = response;
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(_response);
}
