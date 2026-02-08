using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;

internal sealed class NormalizedPrivateApi
{
    private readonly IBittradeRawApi _trading;
    private readonly IBittradeMarketResolver _markets;
    private readonly FreeText _accountId;

    public NormalizedPrivateApi(
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

    public async Task<Call<NormalizedRequests.GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetAccountsBalanceByAccountIdCallAsync(new RawPrivateRequests.GetAccountsBalanceByAccountIdRequest(new AccountId(_accountId.Value)), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetAccountsBalanceByAccountIdRequest(new AccountId(_accountId.Value));

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetAccountsBalanceByAccountId),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    return MapResult<GetAccountsBalanceByAccountIdResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade balance response invalid."));
                }

                if (!Normalizer.TryNormalizeBalances(ok.Data, out var balances, out var normalizeError))
                {
                    return MapResult<GetAccountsBalanceByAccountIdResponse>.Fail(normalizeError!);
                }

                return MapResult<GetAccountsBalanceByAccountIdResponse>.Ok(
                    new GetAccountsBalanceByAccountIdResponse(balances!));
            });
    }

    public async Task<Call<NormalizedRequests.GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetAccountsCallAsync(new RawPrivateRequests.GetAccountsRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetAccountsRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetAccounts),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<GetAccountsResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade accounts response invalid."));
                }

                if (!Normalizer.TryNormalizeAccounts(ok.Data, out var accounts, out var normalizeError))
                {
                    return MapResult<GetAccountsResponse>.Fail(normalizeError!);
                }

                return MapResult<GetAccountsResponse>.Ok(
                    new GetAccountsResponse(accounts!));
            });
    }

    public async Task<Call<NormalizedRequests.GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        NormalizedRequests.GetDepositWithdrawRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetDepositWithdrawCallAsync(new RawPrivateRequests.GetDepositWithdrawRequest(
                new FreeText(request.Type.ToString()),
                request.Currency.HasValue ? new FreeText(CurrencyCodeConverter.ToCurrencyString(request.Currency.Value)) : null,
                request.From,
                request.Size,
                request.Direct), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetDepositWithdraw),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<GetDepositWithdrawResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade deposit/withdraw response invalid."));
                }

                if (!Normalizer.TryNormalizeDepositWithdraws(ok.Data, out var entries, out var normalizeError))
                {
                    return MapResult<GetDepositWithdrawResponse>.Fail(normalizeError!);
                }

                return MapResult<GetDepositWithdrawResponse>.Ok(
                    new GetDepositWithdrawResponse(entries!));
            });
    }

    public async Task<Call<NormalizedRequests.GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetWithdrawVirtualAddressesCallAsync(new RawPrivateRequests.GetWithdrawVirtualAddressesRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetWithdrawVirtualAddressesRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetWithdrawVirtualAddresses),
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<GetWithdrawVirtualAddressesResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade withdraw addresses response invalid."));
                }

                if (!Normalizer.TryNormalizeWithdrawVirtualAddresses(ok.Data, out var addresses, out var normalizeError))
                {
                    return MapResult<GetWithdrawVirtualAddressesResponse>.Fail(normalizeError!);
                }

                return MapResult<GetWithdrawVirtualAddressesResponse>.Ok(
                    new GetWithdrawVirtualAddressesResponse(addresses!));
            });
    }

    public async Task<Call<NormalizedRequests.GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _trading
            .GetRetailAccountBalanceCallAsync(new RawPrivateRequests.GetRetailAccountBalanceRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetRetailAccountBalanceRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetRetailAccountBalance),
            ok =>
            {
                if (ok.Success is not true)
                {
                    return MapResult<GetRetailAccountBalanceResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail balance response invalid."));
                }

                if (!Normalizer.TryNormalizeRetailBalances(ok.Data, out var balances, out var normalizeError))
                {
                    return MapResult<GetRetailAccountBalanceResponse>.Fail(normalizeError!);
                }

                return MapResult<GetRetailAccountBalanceResponse>.Ok(
                    new GetRetailAccountBalanceResponse(balances!));
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        NormalizedRequests.PostOrdersPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var callRequest = request;
        var marketCall = await _markets.ResolveCallAsync(request.Request.Symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.PostOrdersPlaceRequest, PostOrdersPlaceResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.PostOrdersPlace),
                marketError!);
        }

        if (!TradingMapper.TryToRaw(new AccountId(_accountId.Value), new Symbol(apiSymbol!), request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostOrdersPlaceRequest, PostOrdersPlaceResponse>(
                callRequest,
                Component(EndpointIds.PostOrdersPlace),
                mapError!);
        }

        var rawCall = await _trading
            .PostOrdersPlaceCallAsync(new RawPrivateRequests.PostOrdersPlaceRequest(rawRequest!), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(EndpointIds.PostOrdersPlace),
            raw => MapResult<PostOrdersPlaceResponse>.Ok(
                new PostOrdersPlaceResponse(TradingMapper.ToOrderResult(raw))));
    }

    public async Task<Call<NormalizedRequests.GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetOrdersRequest();
        var rawCall = await _trading
            .GetOrdersCallAsync(new RawPrivateRequests.GetOrdersRequest(), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetOrders),
            raw =>
            {
                if (!string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    return MapResult<GetOrdersResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade orders response invalid."));
                }

                if (!TradingMapper.TryToOrderSummaries(raw.Data, out var summaries, out var mapError))
                {
                    return MapResult<GetOrdersResponse>.Fail(mapError!);
                }

                return MapResult<GetOrdersResponse>.Ok(
                    new GetOrdersResponse(summaries!));
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>(
                callRequest,
                Component(EndpointIds.PostOrdersSubmitCancelByOrderId),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            return CreateNotSupported<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>(
                callRequest,
                component: "Bittrade.Trading",
                feature: "CancelOrder",
                reason: $"orderKey.Kind={orderKey.Kind}",
                meta: CallMeta.CreateInternal("Normalized", "Bittrade.Trading"));
        }

        var rawCall = await _trading
            .PostOrdersSubmitCancelByOrderIdCallAsync(new RawPrivateRequests.PostOrdersSubmitCancelByOrderIdRequest(new OrderId(orderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(EndpointIds.PostOrdersSubmitCancelByOrderId),
            _ => MapResult<PostOrdersSubmitCancelByOrderIdResponse>.Ok(
                new PostOrdersSubmitCancelByOrderIdResponse(new CancelResult(true))));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        NormalizedRequests.PostOrdersBatchCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostOrdersBatchCancelCallAsync(new RawPrivateRequests.PostOrdersBatchCancelRequest(
                new RawPrivateRequests.RawPostOrdersBatchCancelRequest(request.OrderIds.ToList())), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostOrdersBatchCancel),
            raw => MapResult<PostOrdersBatchCancelResponse>.Ok(
                new PostOrdersBatchCancelResponse(
                    new CancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase)))));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
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
                return CreateCallError<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>(
                    marketCall,
                    request,
                    Component(EndpointIds.PostOrdersBatchCancelOpenOrders),
                    marketError!);
            }
        }

        var rawRequest = new RawPrivateRequests.RawPostOrdersBatchCancelOpenOrdersRequest(
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
            .PostOrdersBatchCancelOpenOrdersCallAsync(new RawPrivateRequests.PostOrdersBatchCancelOpenOrdersRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostOrdersBatchCancelOpenOrders),
            raw => MapResult<PostOrdersBatchCancelOpenOrdersResponse>.Ok(
                new PostOrdersBatchCancelOpenOrdersResponse(
                    new CancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase)))));
    }

    public async Task<Call<NormalizedRequests.GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetOpenOrdersRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.GetOpenOrdersRequest, GetOpenOrdersResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.GetOpenOrders),
                marketError!);
        }

        var rawCall = await _trading
            .GetOpenOrdersCallAsync(new RawPrivateRequests.GetOpenOrdersRequest(new Symbol(apiSymbol!), new AccountId(_accountId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(EndpointIds.GetOpenOrders),
            raw =>
            {
                if (!TradingMapper.TryToOpenOrders(symbol, raw, out var orders, out var mapError))
                {
                    return MapResult<GetOpenOrdersResponse>.Fail(mapError!);
                }

                return MapResult<GetOpenOrdersResponse>.Ok(new GetOpenOrdersResponse(orders!));
            });
    }

    public async Task<Call<NormalizedRequests.GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetOrdersByOrderIdRequest(symbol, orderKey);
        if (symbol.IsEmpty)
        {
            return CreateImmediateError<NormalizedRequests.GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>(
                callRequest,
                Component(EndpointIds.GetOrdersByOrderId),
                new CallError(CallErrorKind.Semantic, "Symbol is required."));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            return CreateNotSupported<NormalizedRequests.GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>(
                callRequest,
                component: "Bittrade.Trading",
                feature: "GetOrder",
                reason: $"orderKey.Kind={orderKey.Kind}",
                meta: CallMeta.CreateInternal("Normalized", "Bittrade.Trading"));
        }
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (marketCall.Result is CallResult<MarketInfo>.Err marketError)
        {
            return CreateCallError<NormalizedRequests.GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.GetOrdersByOrderId),
                marketError.Error);
        }

        var market = ((CallResult<MarketInfo>.Ok)marketCall.Result).Response;
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var rawCall = await _trading
            .GetOrdersByOrderIdCallAsync(new RawPrivateRequests.GetOrdersByOrderIdRequest(new OrderId(orderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            Component(EndpointIds.GetOrdersByOrderId),
            raw =>
            {
                if (!TradingMapper.TryToOrderStatus(market.ProductCode, raw, key, out var status, out var mapError))
                {
                    return MapResult<GetOrdersByOrderIdResponse>.Fail(mapError!);
                }

                return MapResult<GetOrdersByOrderIdResponse>.Ok(new GetOrdersByOrderIdResponse(status!));
            });
    }

    public async Task<Call<NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetOrdersMatchResultsByOrderIdCallAsync(new RawPrivateRequests.GetOrdersMatchResultsByOrderIdRequest(new OrderId(request.OrderKey.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetOrdersMatchResultsByOrderId),
            raw =>
            {
                if (!TradingMapper.TryToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>(), out var executions, out var mapError))
                {
                    return MapResult<GetOrdersMatchResultsByOrderIdResponse>.Fail(mapError!);
                }

                return MapResult<GetOrdersMatchResultsByOrderIdResponse>.Ok(
                    new GetOrdersMatchResultsByOrderIdResponse(executions!));
            });
    }

    public async Task<Call<NormalizedRequests.GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetMatchResultsRequest(symbol, limit);
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.GetMatchResultsRequest, GetMatchResultsResponse>(
                marketCall,
                callRequest,
                Component(EndpointIds.GetMatchResults),
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
            Component(EndpointIds.GetMatchResults),
            raw =>
            {
                if (!TradingMapper.TryToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>(), out var executions, out var mapError))
                {
                    return MapResult<GetMatchResultsResponse>.Fail(mapError!);
                }

                return MapResult<GetMatchResultsResponse>.Ok(new GetMatchResultsResponse(executions!));
            });
    }

    public async Task<Call<NormalizedRequests.PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        NormalizedRequests.PostWithdrawApiCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = new RawPrivateRequests.RawPostWithdrawApiCreateRequest(
            request.Address,
            new FreeText(request.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            request.Currency,
            request.Fee.HasValue ? new FreeText(request.Fee.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)) : null,
            request.AddressTag);

        var rawCall = await _trading
            .PostWithdrawApiCreateCallAsync(new RawPrivateRequests.PostWithdrawApiCreateRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostWithdrawApiCreate),
            raw => MapResult<PostWithdrawApiCreateResponse>.Ok(
                new PostWithdrawApiCreateResponse(TradingMapper.ToWithdrawResult(raw))));
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        NormalizedRequests.PostRetailOrderPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (!TradingMapper.TryToRawRetailOrder(request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>(
                request,
                Component(EndpointIds.PostRetailOrderPlace),
                mapError!);
        }

        var rawCall = await _trading
            .PostRetailOrderPlaceCallAsync(new RawPrivateRequests.PostRetailOrderPlaceRequest(rawRequest!), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostRetailOrderPlace),
            raw => MapResult<PostRetailOrderPlaceResponse>.Ok(
                new PostRetailOrderPlaceResponse(TradingMapper.ToRetailOrderResult(raw))));
    }

    public async Task<Call<NormalizedRequests.GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        NormalizedRequests.GetRetailOrderListRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetRetailOrderListCallAsync(new RawPrivateRequests.GetRetailOrderListRequest(
                Direct: request.Direct,
                Status: request.Status,
                StartTime: request.StartTime,
                EndTime: request.EndTime), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetRetailOrderList),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<GetRetailOrderListResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order list response invalid."));
                }

                if (!TradingMapper.TryToRetailOrders(raw.Data, out var orders, out var mapError))
                {
                    return MapResult<GetRetailOrderListResponse>.Fail(mapError!);
                }

                return MapResult<GetRetailOrderListResponse>.Ok(
                    new GetRetailOrderListResponse(orders!));
            });
    }

    public async Task<Call<NormalizedRequests.GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
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
            Component(EndpointIds.GetRetailOrderDetailByOrderId),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<GetRetailOrderDetailByOrderIdResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order detail response invalid."));
                }

                if (!TradingMapper.TryToRetailOrder(raw.Data, out var order, out var mapError))
                {
                    return MapResult<GetRetailOrderDetailByOrderIdResponse>.Fail(mapError!);
                }

                return MapResult<GetRetailOrderDetailByOrderIdResponse>.Ok(
                    new GetRetailOrderDetailByOrderIdResponse(
                        Found: order is not null,
                        Item: order));
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        NormalizedRequests.PostRetailOrderHistoryRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        string? symbolText = null;
        if (request.Symbol is not null)
        {
            if (!ExchangeSymbol.TryParse(request.Symbol.Value.Value, out var parsedSymbol))
            {
                return CreateImmediateError<NormalizedRequests.PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>(
                    request,
                    Component(EndpointIds.PostRetailOrderHistory),
                    new CallError(CallErrorKind.Mapping, "Bittrade symbol is invalid."));
            }

            symbolText = parsedSymbol.Value;
        }
        var body = new RawPrivateRequests.RawPostRetailOrderHistoryRequest(
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
            Component(EndpointIds.PostRetailOrderHistory),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<PostRetailOrderHistoryResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order history response invalid."));
                }

                if (!TradingMapper.TryToRetailOrders(raw.Data, out var orders, out var mapError))
                {
                    return MapResult<PostRetailOrderHistoryResponse>.Fail(mapError!);
                }

                return MapResult<PostRetailOrderHistoryResponse>.Ok(
                    new PostRetailOrderHistoryResponse(orders!));
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        NormalizedRequests.PostRetailOrderDetailRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var body = new RawPrivateRequests.RawPostRetailOrderDetailRequest(request.OrderId);
        var rawCall = await _trading
            .PostRetailOrderDetailCallAsync(new RawPrivateRequests.PostRetailOrderDetailRequest(body), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostRetailOrderDetail),
            raw =>
            {
                if (raw.Success is not true)
                {
                    return MapResult<PostRetailOrderDetailResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade retail order detail response invalid."));
                }

                if (!TradingMapper.TryToRetailOrder(raw.Data, out var order, out var mapError))
                {
                    return MapResult<PostRetailOrderDetailResponse>.Fail(mapError!);
                }

                return MapResult<PostRetailOrderDetailResponse>.Ok(
                    new PostRetailOrderDetailResponse(
                        Found: order is not null,
                        Item: order));
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        NormalizedRequests.PostRetailOrderCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        if (!TradingMapper.TryToRawRetailOrder(request.Request, out var rawRequest, out var mapError))
        {
            return CreateImmediateError<NormalizedRequests.PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>(
                request,
                Component(EndpointIds.PostRetailOrderCreate),
                mapError!);
        }

        var rawCall = await _trading
            .PostRetailOrderCreateCallAsync(new RawPrivateRequests.PostRetailOrderCreateRequest(
                new RawPrivateRequests.RawPostRetailOrderCreateRequest(rawRequest!.Symbol, rawRequest.Type, rawRequest.Price, rawRequest.Amount, rawRequest.CashAmount)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostRetailOrderCreate),
            raw => MapResult<PostRetailOrderCreateResponse>.Ok(
                new PostRetailOrderCreateResponse(TradingMapper.ToRetailOrderResult(raw))));
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        NormalizedRequests.PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostRetailOrderCancelByOrderIdCallAsync(new RawPrivateRequests.PostRetailOrderCancelByOrderIdRequest(request.OrderId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostRetailOrderCancelByOrderId),
            raw => MapResult<PostRetailOrderCancelByOrderIdResponse>.Ok(
                new PostRetailOrderCancelByOrderIdResponse(TradingMapper.ToRetailOrderResult(raw))));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByAddressIdCreateCallAsync(new RawPrivateRequests.PostWithdrawVirtualByAddressIdCreateRequest(new AddressId(request.AddressId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostWithdrawVirtualByAddressIdCreate),
            raw => MapResult<PostWithdrawVirtualByAddressIdCreateResponse>.Ok(
                new PostWithdrawVirtualByAddressIdCreateResponse(TradingMapper.ToWithdrawResult(raw))));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdPlaceCallAsync(new RawPrivateRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest(new WithdrawId(request.WithdrawId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostWithdrawVirtualByWithdrawIdPlace),
            raw => MapResult<PostWithdrawVirtualByWithdrawIdPlaceResponse>.Ok(
                new PostWithdrawVirtualByWithdrawIdPlaceResponse(TradingMapper.ToWithdrawResult(raw))));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdCancelCallAsync(new RawPrivateRequests.PostWithdrawVirtualByWithdrawIdCancelRequest(new WithdrawId(request.WithdrawId.Value)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.PostWithdrawVirtualByWithdrawIdCancel),
            raw => MapResult<PostWithdrawVirtualByWithdrawIdCancelResponse>.Ok(
                new PostWithdrawVirtualByWithdrawIdCancelResponse(TradingMapper.ToWithdrawResult(raw))));
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
        Call<ResolveBittradeMarketRequest, MarketInfo> marketCall,
        out string? apiSymbol,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<MarketInfo>.Err err)
        {
            apiSymbol = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<MarketInfo>.Ok ok)
        {
            if (ok.Response.ProductCode.IsEmpty)
            {
                apiSymbol = null;
                error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
                return false;
            }

            if (!ExchangeSymbol.TryParse(ok.Response.ProductCode.Value, out var symbol))
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
        Call<ResolveBittradeMarketRequest, MarketInfo> marketCall,
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
