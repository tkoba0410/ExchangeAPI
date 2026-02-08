using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade の口座 API 実装（Balances/Executions）。
/// </summary>
internal sealed class AccountApi
{
    private readonly NormalizedPrivateApi _account;

    public AccountApi(NormalizedPrivateApi account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        CancellationToken cancellationToken = default) =>
        GetAccountsBalanceByAccountIdCallAsync(cancellationToken);

    public async Task<Call<BalanceRequest, BalanceResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new BalanceRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _account.GetAccountsBalanceByAccountIdCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.Account.GetBalance,
                ok => new BalanceResponse(Mapper.MapBalances(ok.Items)));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<BalanceRequest, BalanceResponse>(
                request,
                startedAt,
                Operations.Account.GetBalance,
                ex);
        }
    }

}
