using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Extensions;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;

namespace ExchangeApi.Common.Services;

public sealed class ExchangeInfoMarketResolver : IExchangeMarketResolver
{
    private readonly IExchangeInfoApi _exchangeInfo;
    private ExchangeInfo? _cache;

    public ExchangeInfoMarketResolver(IExchangeInfoApi exchangeInfo)
        => _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));

    public async Task<ExchangeMarketInfo> ResolveAsync(Symbol symbol, CancellationToken ct = default)
    {
        if (symbol.IsEmpty) throw new SymbolNotSupportedException(symbol.ToString());

        _cache ??= await _exchangeInfo.GetExchangeInfoAsync(ct).ConfigureAwait(false);

        var market = _cache.FindMarket(symbol.Value);
        if (market is null) throw new SymbolNotSupportedException(symbol.Value);

        return market;
    }
}
