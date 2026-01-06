using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Margin;

internal sealed class BitflyerMarginApi : IMarginAccountApi
{
    private readonly IBitflyerNormalizedMarginApi _accountApi;
    public BitflyerMarginApi(
        IBitflyerNormalizedMarginApi accountApi)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
    }

    public async Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalancesRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Margin.GetBalances);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalancesRequest, IReadOnlyList<Balance>>(
                request,
                startedAt,
                BitflyerOperations.Margin.GetBalances,
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
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Margin.GetAccountExecutions);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
                request,
                startedAt,
                BitflyerOperations.Margin.GetAccountExecutions,
                ex);
        }
    }

    public async Task<Call<GetOpenPositionsRequest, IReadOnlyList<Position>>> GetOpenPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOpenPositionsRequest(symbol);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetOpenPositionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Margin.GetOpenPositions);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetOpenPositionsRequest, IReadOnlyList<Position>>(
                request,
                startedAt,
                BitflyerOperations.Margin.GetOpenPositions,
                ex);
        }
    }

    public async Task<Call<GetCollateralRequest, Collateral>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetCollateralRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _accountApi.GetCollateralCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.FromCall(request, call, BitflyerOperations.Margin.GetCollateral);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetCollateralRequest, Collateral>(
                request,
                startedAt,
                BitflyerOperations.Margin.GetCollateral,
                ex);
        }
    }

}
