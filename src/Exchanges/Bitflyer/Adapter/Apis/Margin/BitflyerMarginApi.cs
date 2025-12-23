using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Wire.Private;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Margin;

internal sealed class BitflyerMarginApi : IMarginAccountApi
{
    private readonly IBitflyerWireAccountApi _accountApi;
    private readonly ExchangeCode _exchange;

    public BitflyerMarginApi(IBitflyerWireAccountApi accountApi, ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchange = exchange;
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Margin.GetBalances;
        try
        {
            var rawBalances = await _accountApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);

            return rawBalances
                .Select(b => Balance.Create(
                    exchange: ExchangeCode.Bitflyer,
                    currency: b.CurrencyCode,
                    amount: b.Amount,
                    available: b.Available))
                .ToArray();
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Margin.GetAccountExecutions;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            var raw = await _accountApi
                .GetExecutionsAsync(productCode, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return BitflyerAccountMapper.MapAccountExecutions(productCode, raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Margin.GetOpenPositions;
        try
        {
            var productCode = BitflyerCommonMapper.ToApiProductCode(BitflyerCommonMapper.MapSymbolToProductCode(symbol));
            var raw = await _accountApi
                .GetPositionsAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            return BitflyerMarginMapper.MapPositions(raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getpositions API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Margin.GetCollateral;
        try
        {
            var raw = await _accountApi
                .GetCollateralAsync(cancellationToken)
                .ConfigureAwait(false);

            return BitflyerMarginMapper.MapCollateral(raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getcollateral API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }
}
