using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
using ExchangeApi.Utilities.Account;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

internal sealed class BitflyerAccountApi
{
    private readonly IBitflyerNormalizedApi _normalized;
    public BitflyerAccountApi(
        IBitflyerNormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetBalanceRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetBalanceCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Account.GetBalance,
                ok => new GetBalanceResponse(MapBalances(ok)));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetBalanceRequest, GetBalanceResponse>(
                request,
                startedAt,
                BitflyerOperations.Account.GetBalance,
                ex);
        }
    }

    private static IReadOnlyList<GetBalanceEntry> MapBalances(IReadOnlyList<BitflyerBalanceEntryNormalized> balances) =>
        balances
            .Select(b => BalanceFactory.Create(
                currency: b.CurrencyCode,
                amount: b.Amount,
                available: b.Available))
            .ToArray();

}
