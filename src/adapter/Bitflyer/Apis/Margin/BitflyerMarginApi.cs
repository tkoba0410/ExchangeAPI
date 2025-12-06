using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Adapters;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Apis.Margin;

public sealed class BitflyerMarginApi : IMarginAccountApi
{
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly string _exchangeId;

    public BitflyerMarginApi(IBitflyerPrivateApi privateApi, string exchangeId = "bitFlyer")
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _exchangeId = exchangeId;
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rawBalances = await _privateApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);

            return rawBalances
                .Select(b => new Balance(
                    b.CurrencyCode,
                    b.Amount,
                    b.Available))
                .ToArray();
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerMappers.EnrichBitflyerException(ex, _exchangeId, "GetBalances");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchangeId: _exchangeId,
                operation: "GetBalances",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<Position>> GetOpenPositionsAsync(string productCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _privateApi
                .GetPositionsAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            return raw
                .Select(p => new Position(
                    ProductCode: p.ProductCode,
                    Side: BitflyerMappers.MapSide(p.Side),
                    Size: p.Size,
                    Price: p.Price,
                    OpenDate: p.OpenDate,
                    Pnl: p.Pnl))
                .ToArray();
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerMappers.EnrichBitflyerException(ex, _exchangeId, "GetOpenPositions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getpositions API.",
                exchangeId: _exchangeId,
                operation: "GetOpenPositions",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _privateApi
                .GetCollateralAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Collateral(
                Amount: raw.Collateral,
                OpenPositionPnl: raw.OpenPositionPnl,
                RequireCollateral: raw.RequireCollateral,
                KeepRate: raw.KeepRate);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerMappers.EnrichBitflyerException(ex, _exchangeId, "GetCollateral");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getcollateral API.",
                exchangeId: _exchangeId,
                operation: "GetCollateral",
                statusCode: null,
                innerException: ex);
        }
    }
}
