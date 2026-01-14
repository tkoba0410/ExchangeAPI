using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using CommonSymbol = ExchangeApi.Contracts.Common.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Account;

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

    public async Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalancesRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _account.GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                "Bittrade.Account.GetBalances",
                BittradeMapper.MapBalances);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalancesRequest, IReadOnlyList<Balance>>(
                request,
                startedAt,
                "Bittrade.Account.GetBalances",
                ex);
        }
    }

}
