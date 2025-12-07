using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Adapters;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Apis.Account;

public sealed class BitflyerAccountApi : IAccountApi
{
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly string _exchangeId;

    public BitflyerAccountApi(IBitflyerPrivateApi privateApi, string exchangeId = "bitFlyer")
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _exchangeId = exchangeId;
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rawBalances = await _privateApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);

            return BitflyerAccountMapper.MapBalances(rawBalances);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetBalances");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchangeId: _exchangeId,
                operation: "GetBalances",
                statusCode: null,
                innerException: ex);
        }
    }
}
