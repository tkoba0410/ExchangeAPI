using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;

internal sealed class BittradeNormalizedPrivateApi
{
    private readonly IBittradeRawApi _trading;
    private readonly IBittradeMarketResolver _markets;
    private readonly FreeText _accountId;

    public BittradeNormalizedPrivateApi(
        IBittradeRawApi trading,
        IBittradeMarketResolver markets,
        FreeText accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        _accountId = accountId;
    }

    public async Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetAccountsBalanceByAccountIdCallAsync(new RawPrivateRequests.GetAccountBalanceRequest(new AccountId(_accountId.Value)), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBalancesRequest(_accountId);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetAccountsBalanceByAccountId),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    return MapResult<IReadOnlyList<BittradeBalanceEntryNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade balance response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeBalances(ok.Data, out var balances, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeBalanceEntryNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeBalanceEntryNormalized>>.Ok(balances!);
            });
    }

    public async Task<Call<NormalizedRequests.GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetAccountsCallAsync(new RawPrivateRequests.GetAccountsRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetAccountsRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetAccounts),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<IReadOnlyList<BittradeAccountNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade accounts response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeAccounts(ok.Data, out var accounts, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeAccountNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeAccountNormalized>>.Ok(accounts!);
            });
    }

    public async Task<Call<NormalizedRequests.GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        NormalizedRequests.GetDepositWithdrawRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetDepositWithdrawCallAsync(new RawPrivateRequests.GetDepositWithdrawsRequest(
                request.Type,
                request.Currency,
                request.From,
                request.Size,
                request.Direct), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetDepositWithdraw),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<IReadOnlyList<BittradeDepositWithdrawNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade deposit/withdraw response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeDepositWithdraws(ok.Data, out var entries, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeDepositWithdrawNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeDepositWithdrawNormalized>>.Ok(entries!);
            });
    }

    public async Task<Call<NormalizedRequests.GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetWithdrawVirtualAddressesCallAsync(new RawPrivateRequests.GetWithdrawVirtualAddressesRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetWithdrawVirtualAddressesRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetWithdrawVirtualAddresses),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade withdraw addresses response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeWithdrawVirtualAddresses(ok.Data, out var addresses, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>.Ok(addresses!);
            });
    }

    public async Task<Call<NormalizedRequests.GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetRetailAccountBalanceCallAsync(new RawPrivateRequests.GetRetailAccountBalanceRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetRetailAccountBalanceRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetRetailAccountBalance),
            ok =>
            {
                if (ok.Success is not true)
                {
                    return MapResult<IReadOnlyList<BittradeRetailBalanceEntryNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail balance response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeRetailBalances(ok.Data, out var balances, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeRetailBalanceEntryNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeRetailBalanceEntryNormalized>>.Ok(balances!);
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        NormalizedRequests.PostOrdersPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var callRequest = request;
        var marketCall = await _markets.ResolveCallAsync(request.Request.Symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>(
                marketCall,
                callRequest,
                Component(BittradeEndpointIds.PostOrdersPlace),
                marketError!);
        }

        if (!BittradeTradingMapper.TryToRaw(new AccountId(_accountId.Value), new Symbol(apiSymbol!), request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>(
                callRequest,
                Component(BittradeEndpointIds.PostOrdersPlace),
                mapError!);
        }

        var rawCall = await _trading
            .PostOrdersPlaceCallAsync(new RawPrivateRequests.CreateOrderRequest(rawRequest!), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BittradeEndpointIds.PostOrdersPlace),
            raw => MapResult<BittradeOrderResult>.Ok(BittradeTradingMapper.ToOrderResult(raw)));
    }

    public async Task<Call<NormalizedRequests.GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetOrdersRequest();
        var rawCall = await _trading
            .GetOrdersCallAsync(new RawPrivateRequests.GetOrdersRequest(), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetOrders),
            raw =>
            {
                if (!string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<IReadOnlyList<BittradeOrderSummaryNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade orders response invalid."));
                }

                if (!BittradeTradingMapper.TryToOrderSummaries(raw.Data, out var summaries, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeOrderSummaryNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeOrderSummaryNormalized>>.Ok(summaries!);
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>(
                callRequest,
                Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            return CreateNotSupported<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>(
                callRequest,
                component: "Bittrade.Trading",
                feature: "CancelOrder",
                reason: $"orderKey.Kind={orderKey.Kind}",
                meta: CallMeta.CreateInternal("Normalized", "Bittrade.Trading"));
        }

        var rawCall = await _trading
            .PostOrdersSubmitCancelByOrderIdCallAsync(new RawPrivateRequests.CancelOrderRequest(new OrderId(orderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId),
            _ => MapResult<BittradeCancelResult>.Ok(new BittradeCancelResult(true)));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        NormalizedRequests.PostOrdersBatchCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostOrdersBatchCancelCallAsync(new RawPrivateRequests.CancelOrdersRequest(
                new RawPrivateRequests.RawCancelOrdersRequest(request.OrderIds.ToList())), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancel),
            raw => MapResult<BittradeCancelResult>.Ok(
                new BittradeCancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase))));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        string? apiSymbol = null;
        if (request.Symbol is { IsEmpty: false })
        {
            var marketCall = await _markets.ResolveCallAsync(request.Symbol.Value, ct).ConfigureAwait(false);
            if (!TryGetApiSymbol(marketCall, out apiSymbol, out var marketError))
            {
                return CreateCallError<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>(
                    marketCall,
                    request,
                    Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders),
                    marketError!);
            }
        }

        var rawRequest = new RawPrivateRequests.RawCancelOpenOrdersRequest(
            AccountId: new AccountId(_accountId.Value),
            Symbol: apiSymbol is null ? null : new Symbol(apiSymbol),
            Side: request.Side is null
                ? null
                : new FreeText(request.Side.Value == Side.Buy ? "buy" : "sell"),
            Size: request.Size.HasValue
                ? new FreeText(request.Size.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                : null,
            Price: request.Price.HasValue
                ? new FreeText(request.Price.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                : null,
            CreatedAt: request.CreatedAt);

        var rawCall = await _trading
            .PostOrdersBatchCancelOpenOrdersCallAsync(new RawPrivateRequests.CancelOpenOrdersRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders),
            raw => MapResult<BittradeCancelResult>.Ok(
                new BittradeCancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase))));
    }

    public async Task<Call<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetOpenOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>(
                marketCall,
                callRequest,
                Component(BittradeEndpointIds.GetOpenOrders),
                marketError!);
        }

        var rawCall = await _trading
            .GetOpenOrdersCallAsync(new RawPrivateRequests.GetOpenOrdersRequest(new Symbol(apiSymbol!), new AccountId(_accountId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BittradeEndpointIds.GetOpenOrders),
            raw =>
            {
                if (!BittradeTradingMapper.TryToOpenOrders(symbol, raw, out var orders, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeOpenOrder>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeOpenOrder>>.Ok(orders!);
            });
    }

    public async Task<Call<NormalizedRequests.GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetOrderRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<NormalizedRequests.GetOrderRequest, BittradeOrderStatus>(
                callRequest,
                Component(BittradeEndpointIds.GetOrdersByOrderId),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            return CreateNotSupported<NormalizedRequests.GetOrderRequest, BittradeOrderStatus>(
                callRequest,
                component: "Bittrade.Trading",
                feature: "GetOrder",
                reason: $"orderKey.Kind={orderKey.Kind}",
                meta: CallMeta.CreateInternal("Normalized", "Bittrade.Trading"));
        }
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (marketCall.Result is CallResult<BittradeMarketInfo>.Err marketError)
        {
            return CreateCallError<NormalizedRequests.GetOrderRequest, BittradeOrderStatus>(
                marketCall,
                callRequest,
                Component(BittradeEndpointIds.GetOrdersByOrderId),
                marketError.Error);
        }

        var market = ((CallResult<BittradeMarketInfo>.Ok)marketCall.Result).Response;
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var rawCall = await _trading
            .GetOrdersByOrderIdCallAsync(new RawPrivateRequests.GetOrderRequest(new OrderId(orderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BittradeEndpointIds.GetOrdersByOrderId),
            raw =>
            {
                if (!BittradeTradingMapper.TryToOrderStatus(market.ProductCode, raw, key, out var status, out var mapError))
                {
                    return MapResult<BittradeOrderStatus>.Fail(mapError!);
                }

                return MapResult<BittradeOrderStatus>.Ok(status!);
            });
    }

    public async Task<Call<NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetOrdersMatchResultsByOrderIdCallAsync(new RawPrivateRequests.GetOrderMatchResultsRequest(new OrderId(request.OrderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetOrdersMatchResultsByOrderId),
            raw =>
            {
                if (!BittradeTradingMapper.TryToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>(), out var executions, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Ok(executions!);
            });
    }

    public async Task<Call<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetAccountExecutionsRequest(symbol, limit);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                marketCall,
                callRequest,
                Component(BittradeEndpointIds.GetMatchResults),
                marketError!);
        }

        var requestedLimit = limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        var rawCall = await _trading
            .GetMatchResultsCallAsync(new RawPrivateRequests.GetMatchResultsRequest(Symbol: new Symbol(apiSymbol!), Size: appliedLimit), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(BittradeEndpointIds.GetMatchResults),
            raw =>
            {
                if (!BittradeTradingMapper.TryToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>(), out var executions, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Ok(executions!);
            });
    }

    public async Task<Call<NormalizedRequests.PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        NormalizedRequests.PostWithdrawApiCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = new RawPrivateRequests.RawCreateWithdrawRequest(
            request.Address,
            new FreeText(request.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            request.Currency,
            request.Fee.HasValue ? new FreeText(request.Fee.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)) : null,
            request.AddressTag);

        var rawCall = await _trading
            .PostWithdrawApiCreateCallAsync(new RawPrivateRequests.CreateWithdrawRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostWithdrawApiCreate),
            raw => MapResult<BittradeWithdrawResult>.Ok(BittradeTradingMapper.ToWithdrawResult(raw)));
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        NormalizedRequests.PostRetailOrderPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (!BittradeTradingMapper.TryToRawRetailOrder(request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostRetailOrderPlaceRequest, BittradeRetailOrderResult>(
                request,
                Component(BittradeEndpointIds.PostRetailOrderPlace),
                mapError!);
        }

        var rawCall = await _trading
            .PostRetailOrderPlaceCallAsync(new RawPrivateRequests.CreateRetailOrderRequest(rawRequest!), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostRetailOrderPlace),
            raw => MapResult<BittradeRetailOrderResult>.Ok(BittradeTradingMapper.ToRetailOrderResult(raw)));
    }

    public async Task<Call<NormalizedRequests.GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        NormalizedRequests.GetRetailOrderListRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetRetailOrderListCallAsync(new RawPrivateRequests.GetRetailOrdersRequest(
                Direct: request.Direct,
                Status: request.Status,
                StartTime: request.StartTime,
                EndTime: request.EndTime), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetRetailOrderList),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order list response invalid."));
                }

                if (!BittradeTradingMapper.TryToRetailOrders(raw.Data, out var orders, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Ok(orders!);
            });
    }

    public async Task<Call<NormalizedRequests.GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        NormalizedRequests.GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetRetailOrderDetailByOrderIdCallAsync(new RawPrivateRequests.GetRetailOrderDetailByOrderIdRequest(request.OrderId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetRetailOrderDetailByOrderId),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<BittradeRetailOrderEntryNormalized?>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order detail response invalid."));
                }

                if (!BittradeTradingMapper.TryToRetailOrder(raw.Data, out var order, out var mapError))
                {
                    return MapResult<BittradeRetailOrderEntryNormalized?>.Fail(mapError!);
                }

                return MapResult<BittradeRetailOrderEntryNormalized?>.Ok(order);
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        NormalizedRequests.PostRetailOrderHistoryRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        string? symbolText = null;
        if (request.Symbol is not null)
        {
            if (!BittradeSymbol.TryParse(request.Symbol.Value.Value, out var parsedSymbol))
            {
                return CreateImmediateError<NormalizedRequests.PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>(
                    request,
                    Component(BittradeEndpointIds.PostRetailOrderHistory),
                    new CallError(CallErrorKind.Mapping, "Bittrade symbol is invalid."));
            }

            symbolText = parsedSymbol.Value;
        }
        var body = new RawPrivateRequests.RawRetailOrderHistoryRequest(
            Symbol: symbolText is null ? null : new Symbol(symbolText),
            Direct: request.Direct,
            Status: request.Status,
            StartTime: request.StartTime?.ToUnixTimeMilliseconds(),
            EndTime: request.EndTime?.ToUnixTimeMilliseconds(),
            Size: request.Size);
        var rawCall = await _trading
            .PostRetailOrderHistoryCallAsync(new RawPrivateRequests.PostRetailOrderHistoryRequest(body), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostRetailOrderHistory),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order history response invalid."));
                }

                if (!BittradeTradingMapper.TryToRetailOrders(raw.Data, out var orders, out var mapError))
                {
                    return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Fail(mapError!);
                }

                return MapResult<IReadOnlyList<BittradeRetailOrderEntryNormalized>>.Ok(orders!);
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        NormalizedRequests.PostRetailOrderDetailRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var body = new RawPrivateRequests.RawRetailOrderDetailRequest(request.OrderId);
        var rawCall = await _trading
            .PostRetailOrderDetailCallAsync(new RawPrivateRequests.PostRetailOrderDetailRequest(body), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostRetailOrderDetail),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<BittradeRetailOrderEntryNormalized?>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order detail response invalid."));
                }

                if (!BittradeTradingMapper.TryToRetailOrder(raw.Data, out var order, out var mapError))
                {
                    return MapResult<BittradeRetailOrderEntryNormalized?>.Fail(mapError!);
                }

                return MapResult<BittradeRetailOrderEntryNormalized?>.Ok(order);
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        NormalizedRequests.PostRetailOrderCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (!BittradeTradingMapper.TryToRawRetailOrder(request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostRetailOrderCreateRequest, BittradeRetailOrderResult>(
                request,
                Component(BittradeEndpointIds.PostRetailOrderCreate),
                mapError!);
        }

        var rawCall = await _trading
            .PostRetailOrderCreateCallAsync(new RawPrivateRequests.CreateRetailOrderRequest(rawRequest!), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostRetailOrderCreate),
            raw => MapResult<BittradeRetailOrderResult>.Ok(BittradeTradingMapper.ToRetailOrderResult(raw)));
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        NormalizedRequests.PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostRetailOrderCancelByOrderIdCallAsync(new RawPrivateRequests.CancelRetailOrderRequest(request.OrderId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostRetailOrderCancelByOrderId),
            raw => MapResult<BittradeRetailOrderResult>.Ok(BittradeTradingMapper.ToRetailOrderResult(raw)));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByAddressIdCreateCallAsync(new RawPrivateRequests.CreateWithdrawVirtualByAddressIdRequest(new AddressId(request.AddressId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByAddressIdCreate),
            raw => MapResult<BittradeWithdrawResult>.Ok(BittradeTradingMapper.ToWithdrawResult(raw)));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdPlaceCallAsync(new RawPrivateRequests.PlaceWithdrawVirtualRequest(new WithdrawId(request.WithdrawId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace),
            raw => MapResult<BittradeWithdrawResult>.Ok(BittradeTradingMapper.ToWithdrawResult(raw)));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdCancelCallAsync(new RawPrivateRequests.CancelWithdrawRequest(new WithdrawId(request.WithdrawId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel),
            raw => MapResult<BittradeWithdrawResult>.Ok(BittradeTradingMapper.ToWithdrawResult(raw)));
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, MapResult<TOk>> mapper)
    {
        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: rawCall.Meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: rawCall.Meta)
        };
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, MapResult<TOk>> mapper)
    {
        try
        {
            var result = mapper(raw);
            if (result.Error is not null)
            {
                return new Call<TReq, TOk>(
                    Id: CallId.New(),
                    StartedAt: rawCall.StartedAt,
                    Duration: rawCall.Duration,
                    Request: request,
                    Result: new CallResult<TOk>.Err(result.Error),
                    Meta: rawCall.Meta);
            }

            var mapped = result.Value!;
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: rawCall.Meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map normalized response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: rawCall.Meta);
        }
    }

    private static bool TryGetApiSymbol(
        Call<ResolveBittradeMarketRequest, BittradeMarketInfo> marketCall,
        out string? apiSymbol,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<BittradeMarketInfo>.Err err)
        {
            apiSymbol = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<BittradeMarketInfo>.Ok ok)
        {
            if (ok.Response.ProductCode.IsEmpty)
            {
                apiSymbol = null;
                error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
                return false;
            }

            if (!BittradeSymbol.TryParse(ok.Response.ProductCode.Value, out var symbol))
            {
                apiSymbol = null;
                error = new CallError(CallErrorKind.Semantic, $"Market resolution returned invalid product code: {ok.Response.ProductCode.Value}.");
                return false;
            }

            apiSymbol = symbol.Value;
            error = null;
            return true;
        }

        apiSymbol = null;
        error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
        return false;
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ResolveBittradeMarketRequest, BittradeMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: marketCall.Meta);
    }

    private static Call<TReq, TOk> CreateNotSupported<TReq, TOk>(
        TReq request,
        string component,
        string feature,
        string? reason,
        CallMeta meta)
    {
        var message = reason is null
            ? $"NotSupported:{feature}"
            : $"NotSupported:{feature}. reason={reason}";
        var error = new CallError(CallErrorKind.Semantic, message);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

    private static Call<TReq, TOk> CreateImmediateError<TReq, TOk>(
        TReq request,
        string component,
        CallError error)
    {
        var now = DateTimeOffset.UtcNow;
        var meta = CallMeta.CreateInternal("Normalized", component);
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

    private readonly record struct MapResult<TOk>(TOk? Value, CallError? Error)
    {
        public static MapResult<TOk> Ok(TOk value) => new(value, null);
        public static MapResult<TOk> Fail(CallError error) => new(default, error);
    }

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
