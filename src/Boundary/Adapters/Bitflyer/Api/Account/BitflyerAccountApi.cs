using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Account;

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
        var call = await GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
        return Unwrap(call, BitflyerOperations.Account.GetBalances);
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var call = await GetAccountExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, BitflyerOperations.Account.GetAccountExecutions);
    }

    public async Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalancesRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Account.GetBalances);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalancesRequest, IReadOnlyList<Balance>>(
                request,
                startedAt,
                BitflyerOperations.Account.GetBalances,
                ex);
        }
    }

    public async Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountExecutionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetAccountExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Account.GetAccountExecutions);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
                request,
                startedAt,
                BitflyerOperations.Account.GetAccountExecutions,
                ex);
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

    private static TOk Unwrap<TReq, TOk>(Call<TReq, TOk> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TOk>.Ok ok => ok.Response,
            CallResult<TOk>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.Error.HttpStatus),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error)),
            _ => throw new ExchangeApiException(
                message: "Unknown call result.",
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(new CallError(CallErrorKind.Unknown, "Unknown call result.")))
        };
    }

}
