using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using BitflyerOrderRequest = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests.BitflyerOrderRequest;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Mappers;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Operations;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi
{
    private readonly IBitflyerNormalizedApi _normalized;

    public BitflyerTradingApi(
        IBitflyerNormalizedApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitCallAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default)
    {
        var request = new OrderLimitRequest(symbol, side, size, price);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var normalizedRequest = new BitflyerOrderRequest(
                Symbol: symbol,
                Side: side,
                OrderType: OrderType.Limit,
                Size: size,
                Price: price);
            var call = await _normalized
                .SendChildOrderCallAsync(normalizedRequest, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.PlaceOrder,
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<OrderLimitRequest, OrderLimitResponse>(
                request,
                startedAt,
                BitflyerOperations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.Trading.CancelOrder,
                ok => new CancelOrderResponse(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelOrderResponse>(
                request,
                startedAt,
                BitflyerOperations.Trading.CancelOrder,
                ex);
        }
    }


}
