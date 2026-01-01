using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Account;

internal sealed class BitflyerAccountApi : IAccountApi
{
    private readonly IBitflyerNormalizedAccountApi _accountApi;
    private readonly ExchangeCode _exchange;

    public BitflyerAccountApi(
        IBitflyerNormalizedAccountApi accountApi,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _exchange = exchange;
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Account.GetBalances;
        try
        {
            return await _accountApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
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
        var operation = BitflyerOperations.Account.GetAccountExecutions;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            return await _accountApi
                .GetAccountExecutionsAsync(symbol, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
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

    public async Task<JsonElement> GetTradingCommissionAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Account.GetTradingCommission;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            return await _accountApi
                .GetTradingCommissionAsync(symbol, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer gettradingcommission API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

}
