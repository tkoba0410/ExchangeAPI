using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

internal sealed class BitflyerNormalizedMarginApi : IBitflyerNormalizedMarginApi
{
    private readonly IBitflyerRawAccountApi _accountApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedMarginApi(IBitflyerRawAccountApi accountApi, IExchangeMarketResolver markets)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var rawBalances = await _accountApi.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerAccountMapper.MapBalances(rawBalances);
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var raw = await _accountApi
            .GetExecutionsAsync(productCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return BitflyerAccountMapper.MapAccountExecutions(symbol, raw);
    }

    public async Task<IReadOnlyList<Position>> GetOpenPositionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var raw = await _accountApi.GetPositionsAsync(productCode, cancellationToken).ConfigureAwait(false);
        return BitflyerMarginMapper.MapPositions(symbol, raw);
    }

    public async Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default)
    {
        var raw = await _accountApi.GetCollateralAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerMarginMapper.MapCollateral(raw);
    }

    private async Task<RawProductCode> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return new RawProductCode(market.ProductCode);
    }
}
