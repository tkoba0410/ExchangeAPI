using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using CommonTicker = ExchangeApi.Contracts.Common.Dtos.TickerResponse;
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

    public Task<Call<TickerRequest, CommonTicker>> GetTickerAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetBoardAsync(symbol, cancellationToken);

    public Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _marketApi.GetExecutionsPublicAsync(symbol, cancellationToken);

    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);

    // Raw access removed from public facade.
}
