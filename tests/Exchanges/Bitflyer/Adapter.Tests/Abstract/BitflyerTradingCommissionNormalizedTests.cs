using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerTradingCommissionNormalizedTests
{
    [Fact]
    public async Task NormalizeTradingCommission_ParsesRate()
    {
        var privateApi = new FakeBitflyerPrivateApi(
            response: Array.Empty<RawPrivateDtos.GetBalanceItem>(),
            tradingCommissionJson: "{\"commission_rate\":0.15}");
        var api = BitflyerTestHelpers.CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            BitflyerTestHelpers.CreateResolver(),
            privateApi: privateApi);

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<GetTradingCommissionResponse>.Ok>(call.Result);

        Assert.Null(ok.Response.ProductCode);
        Assert.Equal(0.15m, ok.Response.CommissionRate);
    }

    [Fact]
    public async Task NormalizeTradingCommission_InvalidDecimal_ReturnsMappingError()
    {
        var privateApi = new FakeBitflyerPrivateApi(
            response: Array.Empty<RawPrivateDtos.GetBalanceItem>(),
            tradingCommissionJson: "{\"commission_rate\":\"not-a-decimal\"}");
        var api = BitflyerTestHelpers.CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            BitflyerTestHelpers.CreateResolver(),
            privateApi: privateApi);

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var err = Assert.IsType<CallResult<GetTradingCommissionResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Mapping, err.Error.Kind);
        Assert.Contains("TradingCommissionResponse.commission_rate", err.Error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NormalizeTradingCommission_EmptyDecimal_AllowsNull()
    {
        var privateApi = new FakeBitflyerPrivateApi(
            response: Array.Empty<RawPrivateDtos.GetBalanceItem>(),
            tradingCommissionJson: "{\"commission_rate\":\"\"}");
        var api = BitflyerTestHelpers.CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            BitflyerTestHelpers.CreateResolver(),
            privateApi: privateApi);

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<GetTradingCommissionResponse>.Ok>(call.Result);

        Assert.Null(ok.Response.CommissionRate);
    }

    [Fact]
    public async Task NormalizeTradingCommission_ProductCodeInPayload_IsPreserved()
    {
        var privateApi = new FakeBitflyerPrivateApi(
            response: Array.Empty<RawPrivateDtos.GetBalanceItem>(),
            tradingCommissionJson: "{\"product_code\":\"BTC_JPY\",\"commission_rate\":0.15}");
        var api = BitflyerTestHelpers.CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            BitflyerTestHelpers.CreateResolver(),
            privateApi: privateApi);

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<GetTradingCommissionResponse>.Ok>(call.Result);

        Assert.Equal(ProductCode.ParseOrThrow("BTC_JPY"), ok.Response.ProductCode);
        Assert.Equal(0.15m, ok.Response.CommissionRate);
    }
}
