using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

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

        public Task<Call<RawPrivateModels.GetTradingCommissionRequest, RawPrivateModels.RawJsonResponse>> GetTradingCommissionCallAsync(
            RawPrivateModels.GetTradingCommissionRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall(request, new RawPrivateModels.RawJsonResponse(_json)));

        public Task<Call<RawPrivateModels.GetBalancesRequest, IReadOnlyList<RawPrivateModels.BalanceResponse>>> GetBalanceCallAsync(
            RawPrivateModels.GetBalancesRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetAccountExecutionsRequest, IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
            RawPrivateModels.GetAccountExecutionsRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetPositionsRequest, IReadOnlyList<RawPrivateModels.PositionResponse>>> GetPositionsCallAsync(
            RawPrivateModels.GetPositionsRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetCollateralRequest, RawPrivateModels.CollateralResponse>> GetCollateralCallAsync(
            RawPrivateModels.GetCollateralRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.ChildOrderResponse>>> GetChildOrdersCallAsync(
            RawPrivateModels.GetChildOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.ParentOrderResponse>>> GetParentOrdersCallAsync(
            RawPrivateModels.GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.ParentOrderDetailResponse>> GetParentOrderCallAsync(
            RawPrivateModels.GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    private static Call<TReq, TRes> MakeOkCall<TReq, TRes>(TReq request, TRes response)
    {
        var meta = CallMeta.CreateInternal("Raw", "StubAccountApi");

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TRes>.Ok(response),
            Meta: meta);
    }
}
