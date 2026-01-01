using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Spec.CallCommon;
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

    public async Task<ApiCall<GetBalancesRequest, IReadOnlyList<Balance>, ApiError>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalancesRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<Balance>, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<IReadOnlyList<Balance>, JsonElement> err => ApiCallMapper.Err<GetBalancesRequest, IReadOnlyList<Balance>>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalancesRequest, IReadOnlyList<Balance>>(
                _exchange,
                request,
                startedAt,
                ex);
        }
    }

    public async Task<ApiCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>, ApiError>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountExecutionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetAccountExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<ExecutionAccount>, JsonElement> ok => ApiCallMapper.Ok(
                    _exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    ok.Value),
                Err<IReadOnlyList<ExecutionAccount>, JsonElement> err => ApiCallMapper.Err<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
                    _exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
                _exchange,
                request,
                startedAt,
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

    private static TOk Unwrap<TReq, TOk>(ApiCall<TReq, TOk, ApiError> call, string operation)
    {
        return call.Result switch
        {
            ApiOk<TOk, ApiError> ok => ok.Value,
            ApiErr<TOk, ApiError> err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: call.Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.StatusCode),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error.Kind)),
            _ => throw new InvalidOperationException("Unsupported ApiCallResult type.")
        };
    }

}
