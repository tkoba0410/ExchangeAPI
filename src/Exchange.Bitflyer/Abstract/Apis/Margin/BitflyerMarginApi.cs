using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Errors;

namespace Exchange.Bitflyer.Abstract;

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
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetBalances");
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

    public async Task<IReadOnlyList<AccountExecution>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        try
        {
            var raw = await _privateApi
                .GetExecutionsAsync(productCode, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return BitflyerAccountMapper.MapAccountExecutions(productCode, raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetAccountExecutions");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getexecutions API.",
                exchangeId: _exchangeId,
                operation: "GetAccountExecutions",
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

            return BitflyerMarginMapper.MapPositions(raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetOpenPositions");
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

            return BitflyerMarginMapper.MapCollateral(raw);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetCollateral");
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
