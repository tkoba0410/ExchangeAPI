using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Enums;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;

/// <summary>
/// Bittrade の口座 API 実装（Balances/Executions）。
/// </summary>
internal sealed class BittradeAccountApi : IAccountApi
{
    private readonly IRestClient _restClient;
    private readonly string _accountId;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeAccountApi(IRestClient restClient, string accountId)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.Account.GetBalances";
        try
        {
            var resp = await _restClient.GetAsync<BalancesResponse>(
                $"v1/account/accounts/{_accountId}/balance",
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!string.Equals(resp.Status, "ok", StringComparison.OrdinalIgnoreCase) || resp.Data is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade balance response invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            return BittradeMapper.MapBalances(resp.Data);
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
