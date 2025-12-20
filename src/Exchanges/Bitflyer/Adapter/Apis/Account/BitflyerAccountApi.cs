using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Account;

internal sealed class BitflyerAccountApi : IAccountApi
{
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly ExchangeCode _exchange;

    public BitflyerAccountApi(IBitflyerPrivateApi privateApi, ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _exchange = exchange;
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
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetBalances");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchange: _exchange,
                operation: "GetBalances",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
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
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, "GetAccountExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchange: _exchange,
                operation: "GetAccountExecutions",
                statusCode: null,
                innerException: ex);
        }
    }
}
