using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Operations;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;

/// <summary>
/// Bittrade の口座 API 実装（Balances/Executions）。
/// </summary>
internal sealed class BittradeAccountApi
{
    private readonly BittradeNormalizedPrivateApi _account;

    public BittradeAccountApi(BittradeNormalizedPrivateApi account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        GetAccountsBalanceByAccountIdCallAsync(cancellationToken);

    public async Task<Call<GetBalanceRequest, GetBalanceResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalanceRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _account.GetAccountsBalanceByAccountIdCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BittradeOperations.Account.GetBalance,
                ok => new GetBalanceResponse(BittradeMapper.MapBalances(ok)));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalanceRequest, GetBalanceResponse>(
                request,
                startedAt,
                BittradeOperations.Account.GetBalance,
                ex);
        }
    }

}
