using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerTradingCommissionNormalizedTests
{
    [Fact]
    public async Task NormalizeTradingCommission_ParsesRate()
    {
        var api = BitflyerTestHelpers.CreateAccountApi(
            new StubAccountApi("{\"commission_rate\":0.15}"),
            BitflyerTestHelpers.CreateResolver());

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<BitflyerTradingCommissionNormalized>.Ok>(call.Result);

        Assert.Equal("BTC_JPY", ok.Response.ProductCode);
        Assert.Equal(0.15m, ok.Response.CommissionRate);
    }

    private sealed class StubAccountApi : IBitflyerRawAccountApi
    {
        private readonly string _json;

        public StubAccountApi(string json) => _json = json;

        public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionAsync(
            GetTradingCommissionRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall(request, new RawJsonResponse(_json)));

        public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
            GetBalancesRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
            GetAccountExecutionsRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsAsync(
            GetPositionsRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralAsync(
            GetCollateralRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
            GetChildOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersAsync(
            GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
            GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    private static Call<TReq, TRes> MakeOkCall<TReq, TRes>(TReq request, TRes response)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: "StubAccountApi",
            Tags: null,
            Children: null);

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TRes>.Ok(response),
            Meta: meta);
    }
}
