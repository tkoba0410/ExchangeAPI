using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Common.Application.Adapter.Internal;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Operations;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade の口座 API 実装（Balances/Executions）。
/// </summary>
internal sealed class AccountApi
{
    private static readonly string OpGetBalance = OperationComponent.WithExchange("Bittrade", ContractOperations.Account.GetBalance);

    private readonly NormalizedPrivateApi _account;

    public AccountApi(NormalizedPrivateApi account)
    {
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken);

    public async Task<Call<BalanceRequest, BalanceResponse>> GetAccountsBalanceByAccountIdCallAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                OpGetBalance,
                ct => _account.GetAccountsBalanceByAccountIdCallAsync(ct),
                ok => new BalanceResponse(Mapper.MapBalances(ok.Items)),
                cancellationToken)
            .ConfigureAwait(false);
    }

}
