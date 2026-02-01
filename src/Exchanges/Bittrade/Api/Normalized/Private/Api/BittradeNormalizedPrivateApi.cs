using System;
using System.Collections.Generic;
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
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.NotSupported;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using RawPrivateDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;

internal sealed class BittradeNormalizedPrivateApi
{
    private readonly IBittradeRawApi _trading;
    private readonly IBittradeMarketResolver _markets;
    private readonly string? _accountId;

    public BittradeNormalizedPrivateApi(
        IBittradeRawApi trading,
        IBittradeMarketResolver markets,
        string? accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        _accountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }

    public async Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_accountId))
        {
            return BittradeNotSupportedNormalizedCalls.Create<
                NormalizedRequests.GetBalancesRequest,
                IReadOnlyList<BittradeBalanceEntryNormalized>>(
                new NormalizedRequests.GetBalancesRequest(string.Empty),
                "AccountIdRequired");
        }

        var rawCall = await _trading
            .GetAccountsBalanceByAccountIdCallAsync(new RawPrivateRequests.GetAccountBalanceRequest(_accountId), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBalancesRequest(_accountId!);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetAccountsBalanceByAccountId",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    throw new InvalidOperationException("Bittrade balance response invalid.");
                }

                return BittradeNormalizer.NormalizeBalances(ok.Data);
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
            "Bittrade.GetAccounts",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade accounts response invalid.");
                }

                return BittradeNormalizer.NormalizeAccounts(ok.Data);
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
            "Bittrade.GetDepositWithdraw",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade deposit/withdraw response invalid.");
                }

                return BittradeNormalizer.NormalizeDepositWithdraws(ok.Data);
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
            "Bittrade.GetWithdrawVirtualAddresses",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade withdraw addresses response invalid.");
                }

                return BittradeNormalizer.NormalizeWithdrawVirtualAddresses(ok.Data);
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
            "Bittrade.GetRetailAccountBalance",
            ok =>
            {
                if (ok.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail balance response invalid.");
                }

                return BittradeNormalizer.NormalizeRetailBalances(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        NormalizedRequests.PostOrdersPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (!HasAccountId)
        {
            return AccountIdMissing<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>(request);
        }

        var callRequest = request;
        var marketCall = await _markets.ResolveCallAsync(request.Request.Symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.PostOrdersPlaceRequest, BittradeOrderResult>(
                marketCall,
                callRequest,
                "Bittrade.PostOrdersPlace",
                marketError!);
        }

        var rawRequest = BittradeTradingMapper.ToRaw(_accountId!, apiSymbol!, request.Request);
        var rawCall = await _trading
            .PostOrdersPlaceCallAsync(new RawPrivateRequests.CreateOrderRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bittrade.PostOrdersPlace", BittradeTradingMapper.ToOrderResult);
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
            "Bittrade.GetOrders",
            raw =>
            {
                if (!string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Bittrade orders response invalid.");
                }

                return BittradeTradingMapper.ToOrderSummaries(raw.Data);
            });
    }

    public async Task<Call<NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey);
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
            .PostOrdersSubmitCancelByOrderIdCallAsync(new RawPrivateRequests.CancelOrderRequest(orderKey.Value), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bittrade.PostOrdersSubmitCancelByOrderId", _ => new BittradeCancelResult(true));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        NormalizedRequests.PostOrdersBatchCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostOrdersBatchCancelCallAsync(new RawPrivateRequests.CancelOrdersRequest(
                new RawPrivateRequests.RawCancelOrdersRequest(request.OrderIds)), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, request, "Bittrade.PostOrdersBatchCancel", raw =>
            new BittradeCancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<Call<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (!HasAccountId)
        {
            return AccountIdMissing<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>(request);
        }

        string? apiSymbol = null;
        if (request.Symbol is { IsEmpty: false })
        {
            var marketCall = await _markets.ResolveCallAsync(request.Symbol.Value, ct).ConfigureAwait(false);
            if (!TryGetApiSymbol(marketCall, out apiSymbol, out var marketError))
            {
                return CreateCallError<NormalizedRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>(
                    marketCall,
                    request,
                    "Bittrade.PostOrdersBatchCancelOpenOrders",
                    marketError!);
            }
        }

        var rawRequest = new RawPrivateRequests.RawCancelOpenOrdersRequest(
            AccountId: _accountId!,
            Symbol: apiSymbol,
            Side: request.Side is null
                ? null
                : request.Side.Value == Side.Buy
                    ? "buy"
                    : "sell",
            Size: request.Size?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Price: request.Price?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            CreatedAt: request.CreatedAt);

        var rawCall = await _trading
            .PostOrdersBatchCancelOpenOrdersCallAsync(new RawPrivateRequests.CancelOpenOrdersRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, request, "Bittrade.PostOrdersBatchCancelOpenOrders", raw =>
            new BittradeCancelResult(string.Equals(raw.Status, "ok", StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<Call<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var callRequest = new NormalizedRequests.GetOpenOrdersRequest(symbol);
        if (!HasAccountId)
        {
            return AccountIdMissing<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>(callRequest);
        }
        var marketCall = await _markets.ResolveCallAsync(symbol, ct).ConfigureAwait(false);
        if (!TryGetApiSymbol(marketCall, out var apiSymbol, out var marketError))
        {
            return CreateCallError<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>(
                marketCall,
                callRequest,
                "Bittrade.GetOpenOrders",
                marketError!);
        }

        var rawCall = await _trading
            .GetOpenOrdersCallAsync(new RawPrivateRequests.GetOpenOrdersRequest(apiSymbol!, _accountId!), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bittrade.GetOpenOrders", raw => BittradeTradingMapper.ToOpenOrders(symbol, raw));
    }

    public async Task<Call<NormalizedRequests.GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var callRequest = new NormalizedRequests.GetOrderRequest(symbol, orderKey);
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
                "Bittrade.GetOrdersByOrderId",
                marketError.Error);
        }

        var market = ((CallResult<BittradeMarketInfo>.Ok)marketCall.Result).Response;
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var rawCall = await _trading
            .GetOrdersByOrderIdCallAsync(new RawPrivateRequests.GetOrderRequest(orderKey.Value), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, callRequest, "Bittrade.GetOrdersByOrderId", raw => BittradeTradingMapper.ToOrderStatus(market.ProductCode, raw, key));
    }

    public async Task<Call<NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        NormalizedRequests.GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .GetOrdersMatchResultsByOrderIdCallAsync(new RawPrivateRequests.GetOrderMatchResultsRequest(request.OrderKey.Value), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetOrdersMatchResultsByOrderId",
            raw => BittradeTradingMapper.ToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>()));
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
                "Bittrade.GetMatchResults",
                marketError!);
        }

        var requestedLimit = limit ?? 1000;
        var appliedLimit = Math.Min(requestedLimit, 1000);
        var rawCall = await _trading
            .GetMatchResultsCallAsync(new RawPrivateRequests.GetMatchResultsRequest(Symbol: apiSymbol, Size: appliedLimit), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            callRequest,
            "Bittrade.GetMatchResults",
            raw => BittradeTradingMapper.ToExecutions(raw.Data ?? Array.Empty<RawPrivateDtos.RawMatchResultEntry>()));
    }

    public async Task<Call<NormalizedRequests.PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        NormalizedRequests.PostWithdrawApiCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = new RawPrivateRequests.RawCreateWithdrawRequest(
            request.Address,
            request.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            request.Currency,
            request.Fee?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            request.AddressTag);

        var rawCall = await _trading
            .PostWithdrawApiCreateCallAsync(new RawPrivateRequests.CreateWithdrawRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, request, "Bittrade.PostWithdrawApiCreate", BittradeTradingMapper.ToWithdrawResult);
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        NormalizedRequests.PostRetailOrderPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BittradeTradingMapper.ToRawRetailOrder(request.Request);
        var rawCall = await _trading
            .PostRetailOrderPlaceCallAsync(new RawPrivateRequests.CreateRetailOrderRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(rawCall, request, "Bittrade.PostRetailOrderPlace", BittradeTradingMapper.ToRetailOrderResult);
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
            "Bittrade.GetRetailOrderList",
            raw =>
            {
                if (raw.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail order list response invalid.");
                }

                return BittradeTradingMapper.ToRetailOrders(raw.Data);
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
            "Bittrade.GetRetailOrderDetailByOrderId",
            raw =>
            {
                if (raw.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail order detail response invalid.");
                }

                return BittradeTradingMapper.ToRetailOrder(raw.Data);
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        NormalizedRequests.PostRetailOrderHistoryRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var symbolText = request.Symbol is null ? null : BittradeSymbol.Normalize(request.Symbol.Value.Value);
        var body = new RawPrivateRequests.RawRetailOrderHistoryRequest(
            Symbol: symbolText,
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
            "Bittrade.PostRetailOrderHistory",
            raw =>
            {
                if (raw.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail order history response invalid.");
                }

                return BittradeTradingMapper.ToRetailOrders(raw.Data);
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
            "Bittrade.PostRetailOrderDetail",
            raw =>
            {
                if (raw.Success is not true)
                {
                    throw new InvalidOperationException("Bittrade retail order detail response invalid.");
                }

                return BittradeTradingMapper.ToRetailOrder(raw.Data);
            });
    }

    public async Task<Call<NormalizedRequests.PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        NormalizedRequests.PostRetailOrderCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BittradeTradingMapper.ToRawRetailOrder(request.Request);
        var rawCall = await _trading
            .PostRetailOrderCreateCallAsync(new RawPrivateRequests.CreateRetailOrderRequest(rawRequest), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.PostRetailOrderCreate",
            BittradeTradingMapper.ToRetailOrderResult);
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
            "Bittrade.PostRetailOrderCancelByOrderId",
            BittradeTradingMapper.ToRetailOrderResult);
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        NormalizedRequests.PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByAddressIdCreateCallAsync(new RawPrivateRequests.CreateWithdrawVirtualByAddressIdRequest(request.AddressId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.PostWithdrawVirtualByAddressIdCreate",
            BittradeTradingMapper.ToWithdrawResult);
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdPlaceCallAsync(new RawPrivateRequests.PlaceWithdrawVirtualRequest(request.WithdrawId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.PostWithdrawVirtualByWithdrawIdPlace",
            BittradeTradingMapper.ToWithdrawResult);
    }

    public async Task<Call<NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        NormalizedRequests.PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawCall = await _trading
            .PostWithdrawVirtualByWithdrawIdCancelCallAsync(new RawPrivateRequests.CancelWithdrawRequest(request.WithdrawId), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.PostWithdrawVirtualByWithdrawIdCancel",
            BittradeTradingMapper.ToWithdrawResult);
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
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
        Func<TRaw, TOk> mapper)
    {
        try
        {
            var mapped = mapper(raw);
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
            if (string.IsNullOrWhiteSpace(ok.Response.ProductCode))
            {
                apiSymbol = null;
                error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
                return false;
            }

            if (!BittradeSymbol.TryParse(ok.Response.ProductCode, out var symbol))
            {
                apiSymbol = null;
                error = new CallError(CallErrorKind.Semantic, $"Market resolution returned invalid product code: {ok.Response.ProductCode}.");
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

    private bool HasAccountId => !string.IsNullOrWhiteSpace(_accountId);

    private static Call<TReq, TOk> AccountIdMissing<TReq, TOk>(TReq request) =>
        BittradeNotSupportedNormalizedCalls.Create<TReq, TOk>(request, "AccountIdRequired");

}
