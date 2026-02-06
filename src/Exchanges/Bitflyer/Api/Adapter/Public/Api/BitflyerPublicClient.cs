using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.GetExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.GetTickerResponse;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;

/// <summary>
/// bitFlyer の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BitflyerPublicClient : IPublicApi, IExchangeClient
{
    private readonly MarketApi _marketApi;
    private readonly BitflyerExchangeInfoApi _exchangeInfoApi;

    public IPublicApi? Public => this;
    public IPrivateApi? Private => null;

    internal BitflyerPublicClient(IBitflyerNormalizedApi normalized, BitflyerExchangeInfoApi exchangeInfo)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _exchangeInfoApi = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        var markets = new ExchangeInfoMarketResolver(_exchangeInfoApi);
        _marketApi = new MarketApi(normalized, markets);
    }

    public Task<Call<GetTickerRequest, CommonTicker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerCallAsync(symbol, cancellationToken);

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardCallAsync(symbol, cancellationToken);

    public Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicCallAsync(symbol, cancellationToken);

    public Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoCallAsync(cancellationToken);

    // Raw access removed from public facade.
}
