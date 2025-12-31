using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Enums;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;

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

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Account.GetBalances";
        try
        {
            var normalized = await _account.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
            return BittradeMapper.MapBalances(normalized);
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (BittradeNormalizedException ex)
        {
            throw new ExchangeApiException(
                message: ex.Message,
                exchange: Exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default)
    {
        throw new ExchangeFeatureNotSupportedException(Exchange, "AccountExecutions");
    }
}
