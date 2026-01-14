using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
using ExchangeApi.Contracts.Common.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Account;

internal sealed class BitflyerAccountApi : IAccountApi
{
    private readonly IBitflyerNormalizedAccountApi _accountApi;
    public BitflyerAccountApi(
        IBitflyerNormalizedAccountApi accountApi)
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

}
