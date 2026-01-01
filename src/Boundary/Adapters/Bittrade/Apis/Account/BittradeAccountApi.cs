using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Enums;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Spec.CallCommon;
using System.Text.Json;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;

/// <summary>
/// Bittrade の口座 API 実装（Balances/Executions）。
/// </summary>
internal sealed class BittradeAccountApi : IAccountApi
{
    private readonly IBittradeNormalizedAccountApi _account;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeAccountApi(IBittradeNormalizedAccountApi account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var call = await GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.Account.GetBalances");
    }

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        throw new ExchangeFeatureNotSupportedException(Exchange, "AccountExecutions");
    }

    public async Task<ApiCall<GetBalancesRequest, IReadOnlyList<Balance>, ApiError>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalancesRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _account.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<BittradeBalanceEntryNormalized>, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    BittradeMapper.MapBalances(ok.Value)),
                Err<IReadOnlyList<BittradeBalanceEntryNormalized>, JsonElement> err => ApiCallMapper.Err<GetBalancesRequest, IReadOnlyList<Balance>>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalancesRequest, IReadOnlyList<Balance>>(
                Exchange,
                request,
                startedAt,
                ex);
        }
    }

    public Task<ApiCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>, ApiError>> GetAccountExecutionsCallAsync(
        CommonSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetAccountExecutionsRequest(symbol);
        var meta = ApiCallMapper.ToMeta(DateTimeOffset.UtcNow);
        return Task.FromResult(ApiCallMapper.Err<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
            Exchange,
            request,
            meta,
            0,
            "Feature not supported."));
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
