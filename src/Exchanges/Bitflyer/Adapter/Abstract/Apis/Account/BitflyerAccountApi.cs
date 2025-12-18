using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Account;

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

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol == Symbol.Unknown)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            var raw = await _privateApi
                .GetExecutionsAsync(productCode, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return BitflyerAccountMapper.MapAccountExecutions(productCode, raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetAccountExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchangeId: _exchangeId,
                operation: "GetAccountExecutions",
                statusCode: null,
                innerException: ex);
        }
    }
}
