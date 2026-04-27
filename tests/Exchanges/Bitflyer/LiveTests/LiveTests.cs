using System.Text.Json;
using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests;

public sealed class LiveTests
{
    [BitflyerPublicReadLiveFact]
    public async Task RealtimeTicker_ReadsPublicTicker()
    {
        await using var client = BitflyerRealtimeClientFactory.CreatePublicClient(new BitflyerRealtimeClientOptions
        {
            ConnectTimeout = TimeSpan.FromSeconds(10),
        });
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await foreach (var ticker in client.SubscribeTickerAsync(ProductCodes.BtcJpy, timeout.Token))
        {
            Assert.Equal(ProductCodes.BtcJpy, ticker.ProductCode);
            Assert.Equal($"lightning_ticker_{ProductCodes.BtcJpy}", ticker.Channel);
            Assert.True(ticker.Ltp > 0);
            Assert.True(ticker.ReceivedAt > DateTimeOffset.MinValue);
            return;
        }

        Assert.Fail("No realtime ticker message was received.");
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetMarkets_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        var nativeCall = await client.Public.GetMarketsAsync(new GetMarketsRequest());
        var protocolCall = await client.Protocol.Public.GetMarketsAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("product_code").GetString(), nativeItem.ProductCode);
            Assert.Equal(protocolItem.GetProperty("market_type").GetString(), ApiStringEnum<BitflyerMarketType>.Format(nativeItem.MarketType));
        }
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetTicker_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetTickerAsync(request);
        var protocolCall = await client.Protocol.Public.GetTickerAsync(ProductCodes.BtcJpy);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;
        var protocolTimestamp = ParseUtcNoOffsetTimestamp(root.GetProperty("timestamp").GetString()!);

        Assert.Equal(root.GetProperty("product_code").GetString(), native.ProductCode);
        Assert.Equal(root.GetProperty("state").GetString(), ApiStringEnum<BitflyerTradingState>.Format(native.State));
        Assert.Equal(TimeSpan.Zero, protocolTimestamp.Offset);
        Assert.Equal(TimeSpan.Zero, native.Timestamp.Offset);
        Assert.True(root.GetProperty("tick_id").GetInt64() > 0);
        Assert.True(root.GetProperty("best_bid").GetDecimal() > 0);
        Assert.True(root.GetProperty("best_ask").GetDecimal() > 0);
        Assert.True(root.GetProperty("ltp").GetDecimal() > 0);
        Assert.True(root.GetProperty("volume").GetDecimal() >= 0);
        Assert.True(root.GetProperty("volume_by_product").GetDecimal() >= 0);
        Assert.True(native.TickId > 0);
        Assert.True(native.BestBid > 0);
        Assert.True(native.BestAsk > 0);
        Assert.True(native.Ltp > 0);
        Assert.True(native.Volume >= 0);
        Assert.True(native.VolumeByProduct >= 0);
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetBoard_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetBoardRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetBoardAsync(request);
        var protocolCall = await client.Protocol.Public.GetBoardAsync(ProductCodes.BtcJpy);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.True(root.GetProperty("mid_price").GetDecimal() > 0);
        Assert.Equal(JsonValueKind.Array, root.GetProperty("bids").ValueKind);
        Assert.Equal(JsonValueKind.Array, root.GetProperty("asks").ValueKind);
        Assert.NotEmpty(native.Bids);
        Assert.NotEmpty(native.Asks);
        Assert.True(native.MidPrice > 0);
        Assert.True(native.Bids[0].Price > 0);
        Assert.True(native.Bids[0].Size >= 0);
        Assert.True(native.Asks[0].Price > 0);
        Assert.True(native.Asks[0].Size >= 0);
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetExecutions_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetExecutionsPublicRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
        };

        var nativeCall = await client.Public.GetExecutionsAsync(request);
        var protocolCall = await client.Protocol.Public.GetExecutionsAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.NotEmpty(native);

        var protocolFirst = root[0];
        var nativeFirst = native[0];

        Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
        Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("side").ValueKind);
        Assert.True(protocolFirst.GetProperty("price").GetDecimal() > 0);
        Assert.True(protocolFirst.GetProperty("size").GetDecimal() > 0);
        Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("buy_child_order_acceptance_id").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("sell_child_order_acceptance_id").GetString()));
        Assert.True(nativeFirst.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(ApiStringEnum<BitflyerOrderSide>.Format(nativeFirst.Side)));
        Assert.True(nativeFirst.Price > 0);
        Assert.True(nativeFirst.Size > 0);
        Assert.False(string.IsNullOrWhiteSpace(nativeFirst.BuyChildOrderAcceptanceId));
        Assert.False(string.IsNullOrWhiteSpace(nativeFirst.SellChildOrderAcceptanceId));
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetBoardState_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetBoardStateRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetBoardStateAsync(request);
        var protocolCall = await client.Protocol.Public.GetBoardStateAsync(request.ProductCode);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("health").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("state").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(ApiStringEnum<BitflyerHealthStatus>.Format(native.Health)));
        Assert.False(string.IsNullOrWhiteSpace(ApiStringEnum<BitflyerTradingState>.Format(native.State)));

        if (root.TryGetProperty("data", out var protocolData) &&
            protocolData.ValueKind == JsonValueKind.Object &&
            protocolData.TryGetProperty("special_quotation", out var specialQuotation))
        {
            Assert.True(specialQuotation.GetDecimal() >= 0);
        }

        if (native.Data?.SpecialQuotation is not null)
        {
            Assert.True(native.Data.SpecialQuotation.Value >= 0);
        }
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetHealth_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetHealthRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetHealthAsync(request);
        var protocolCall = await client.Protocol.Public.GetHealthAsync(request.ProductCode);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("status").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(ApiStringEnum<BitflyerHealthStatus>.Format(native.Status)));
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetFundingRate_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        var request = new GetFundingRateRequest { ProductCode = ProductCodes.FxBtcJpy };

        var nativeCall = await client.Public.GetFundingRateAsync(request);
        var protocolCall = await client.Protocol.Public.GetFundingRateAsync(request.ProductCode);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.True(root.GetProperty("current_funding_rate").ValueKind is JsonValueKind.Number);
        Assert.Equal(JsonValueKind.String, root.GetProperty("next_funding_rate_settledate").ValueKind);
        Assert.Equal(ParseUtcNoOffsetTimestamp(root.GetProperty("next_funding_rate_settledate").GetString()!), native.NextFundingRateSettleDate);
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetCorporateLeverage_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        var nativeCall = await client.Public.GetCorporateLeverageAsync(new GetCorporateLeverageRequest());
        var protocolCall = await client.Protocol.Public.GetCorporateLeverageAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.True(root.GetProperty("current_max").GetDecimal() > 0);
        Assert.Equal(JsonValueKind.String, root.GetProperty("current_startdate").ValueKind);
        Assert.True(native.CurrentMax > 0);
        Assert.Equal(ParseUtcNoOffsetTimestamp(root.GetProperty("current_startdate").GetString()!), native.CurrentStartDate);

        if (root.TryGetProperty("next_max", out var nextMax))
        {
            Assert.True(nextMax.GetDecimal() > 0);
        }

        if (native.NextMax is not null)
        {
            Assert.True(native.NextMax.Value > 0);
        }

        if (root.TryGetProperty("next_startdate", out var nextStartDate) && native.NextStartDate is not null)
        {
            Assert.Equal(ParseUtcNoOffsetTimestamp(nextStartDate.GetString()!), native.NextStartDate.Value);
        }
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetChats_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        var nativeCall = await client.Public.GetChatsAsync(new GetChatsRequest());
        var protocolCall = await client.Protocol.Public.GetChatsAsync(null);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.True(native.Count >= 0);

        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("nickname").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("message").GetString()));
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("date").ValueKind);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("date").GetString()!), native[0].Date);
        }

        if (native.Count > 0)
        {
            var nativeFirst = native[0];
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Nickname));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Message));
            Assert.True(nativeFirst.Date != default);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBalance_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetBalanceAsync(new GetBalanceRequest());
        var protocolCall = await client.Protocol.Private!.GetBalanceAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        var protocolByCurrency = root.EnumerateArray()
            .ToDictionary(
                item => item.GetProperty("currency_code").GetString()!,
                item => item);

        foreach (var item in native)
        {
            Assert.True(protocolByCurrency.TryGetValue(item.CurrencyCode, out var protocolItem));
            Assert.Equal(protocolItem.GetProperty("amount").GetDecimal(), item.Amount);
            Assert.Equal(protocolItem.GetProperty("available").GetDecimal(), item.Available);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetPositions_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetPositionsRequest { ProductCode = ProductCodes.FxBtcJpy };
        var nativeCall = await client.Private!.GetPositionsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetPositionsAsync(ProductCodes.FxBtcJpy);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("product_code").GetString(), nativeItem.ProductCode);
            Assert.Equal(protocolItem.GetProperty("side").GetString(), ApiStringEnum<BitflyerOrderSide>.Format(nativeItem.Side));
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("commission").GetDecimal(), nativeItem.Commission);
            Assert.Equal(protocolItem.GetProperty("swap_point_accumulate").GetDecimal(), nativeItem.SwapPointAccumulate);
            Assert.Equal(protocolItem.GetProperty("require_collateral").GetDecimal(), nativeItem.RequireCollateral);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolItem.GetProperty("open_date").GetString()!), nativeItem.OpenDate);
            Assert.Equal(protocolItem.GetProperty("leverage").GetDecimal(), nativeItem.Leverage);
            Assert.Equal(protocolItem.GetProperty("pnl").GetDecimal(), nativeItem.Pnl);
            Assert.Equal(protocolItem.GetProperty("sfd").GetDecimal(), nativeItem.Sfd);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateral_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetCollateralAsync(new GetCollateralRequest());
        var protocolCall = await client.Protocol.Private!.GetCollateralAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.True(root.GetProperty("collateral").GetDecimal() >= 0);
        Assert.True(root.GetProperty("require_collateral").GetDecimal() >= 0);
        Assert.True(root.GetProperty("keep_rate").GetDecimal() >= 0);
        Assert.True(native.Collateral >= 0);
        Assert.True(native.RequireCollateral >= 0);
        Assert.True(native.KeepRate >= 0);

        if (root.TryGetProperty("margin_call_due_date", out var marginCallDueDate)
            && marginCallDueDate.ValueKind == JsonValueKind.String
            && native.MarginCallDueDate is not null)
        {
            Assert.Equal(ParseUtcNoOffsetTimestamp(marginCallDueDate.GetString()!), native.MarginCallDueDate.Value);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateralAccounts_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetCollateralAccountsAsync(new GetCollateralAccountsRequest());
        var protocolCall = await client.Protocol.Private!.GetCollateralAccountsAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("currency_code").GetString(), nativeItem.CurrencyCode);
            Assert.Equal(protocolItem.GetProperty("amount").GetDecimal(), nativeItem.Amount);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetChildOrders_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetChildOrdersRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
            ChildOrderState = ChildOrderStates.Completed,
        };

        var nativeCall = await client.Private!.GetChildOrdersAsync(request);
        var protocolCall = await client.Protocol.Private!.GetChildOrdersAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ChildOrderState is { } childOrderState ? ApiStringEnum<BitflyerOrderState>.Format(childOrderState) : null,
            request.ChildOrderId,
            request.ChildOrderAcceptanceId,
            request.ParentOrderId);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("id").GetInt64(), nativeItem.Id);
            Assert.Equal(protocolItem.GetProperty("child_order_id").GetString(), nativeItem.ChildOrderId);
            Assert.Equal(protocolItem.GetProperty("product_code").GetString(), nativeItem.ProductCode);
            Assert.Equal(protocolItem.GetProperty("side").GetString(), ApiStringEnum<BitflyerOrderSide>.Format(nativeItem.Side));
            Assert.Equal(protocolItem.GetProperty("child_order_type").GetString(), ApiStringEnum<BitflyerChildOrderType>.Format(nativeItem.ChildOrderType));
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("average_price").GetDecimal(), nativeItem.AveragePrice);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("child_order_state").GetString(), ApiStringEnum<BitflyerOrderState>.Format(nativeItem.ChildOrderState));
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolItem.GetProperty("expire_date").GetString()!), nativeItem.ExpireDate);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolItem.GetProperty("child_order_date").GetString()!), nativeItem.ChildOrderDate);
            Assert.Equal(protocolItem.GetProperty("child_order_acceptance_id").GetString(), nativeItem.ChildOrderAcceptanceId);
            Assert.Equal(protocolItem.GetProperty("outstanding_size").GetDecimal(), nativeItem.OutstandingSize);
            Assert.Equal(protocolItem.GetProperty("cancel_size").GetDecimal(), nativeItem.CancelSize);
            Assert.Equal(protocolItem.GetProperty("executed_size").GetDecimal(), nativeItem.ExecutedSize);
            Assert.Equal(protocolItem.GetProperty("total_commission").GetDecimal(), nativeItem.TotalCommission);
            Assert.Equal(protocolItem.GetProperty("time_in_force").GetString(), ApiStringEnum<BitflyerTimeInForce>.Format(nativeItem.TimeInForce));
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetExecutions_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetExecutionsRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
        };

        var nativeCall = await client.Private!.GetExecutionsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetExecutionsAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ChildOrderId,
            request.ChildOrderAcceptanceId);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("id").GetInt64(), nativeItem.Id);
            Assert.Equal(protocolItem.GetProperty("child_order_id").GetString(), nativeItem.ChildOrderId);
            Assert.Equal(protocolItem.GetProperty("side").GetString(), ApiStringEnum<BitflyerOrderSide>.Format(nativeItem.Side));
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("commission").GetDecimal(), nativeItem.Commission);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolItem.GetProperty("exec_date").GetString()!), nativeItem.ExecDate);
            Assert.Equal(protocolItem.GetProperty("child_order_acceptance_id").GetString(), nativeItem.ChildOrderAcceptanceId);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateralHistory_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCollateralHistoryRequest
        {
            Count = 10,
        };

        var nativeCall = await client.Private!.GetCollateralHistoryAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCollateralHistoryAsync(request.Count, request.Before, request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var i = 0; i < native.Count; i++)
        {
            var protocolItem = root[i];
            var nativeItem = native[i];

            Assert.Equal(protocolItem.GetProperty("id").GetInt64(), nativeItem.Id);
            Assert.Equal(protocolItem.GetProperty("currency_code").GetString(), nativeItem.CurrencyCode);
            Assert.Equal(protocolItem.GetProperty("change").GetDecimal(), nativeItem.Change);
            Assert.Equal(protocolItem.GetProperty("amount").GetDecimal(), nativeItem.Amount);
            Assert.Equal(protocolItem.GetProperty("reason_code").GetString(), nativeItem.ReasonCode);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolItem.GetProperty("date").GetString()!), nativeItem.Date);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetTradingCommission_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetTradingCommissionRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        };

        var nativeCall = await client.Private!.GetTradingCommissionAsync(request);
        var protocolCall = await client.Protocol.Private!.GetTradingCommissionAsync(request.ProductCode);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Object, root.ValueKind);
        Assert.Equal(root.GetProperty("commission_rate").GetDecimal(), native.CommissionRate);
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetPermissions_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetPermissionsAsync(new GetPermissionsRequest());
        var protocolCall = await client.Protocol.Private!.GetPermissionsAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        foreach (var item in root.EnumerateArray())
        {
            Assert.Equal(JsonValueKind.String, item.ValueKind);
            Assert.False(string.IsNullOrWhiteSpace(item.GetString()));
        }

        foreach (var item in native)
        {
            Assert.False(string.IsNullOrWhiteSpace(item));
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetAddresses_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetAddressesAsync(new GetAddressesRequest());
        var protocolCall = await client.Protocol.Private!.GetAddressesAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("type").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("address").GetString()));
        }

        if (native.Count > 0)
        {
            var nativeFirst = native[0];
            Assert.False(string.IsNullOrWhiteSpace(ApiStringEnum<BitflyerAddressType>.Format(nativeFirst.Type)));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Address));
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBankAccounts_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetBankAccountsAsync(new GetBankAccountsRequest());
        var protocolCall = await client.Protocol.Private!.GetBankAccountsAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.True(protocolFirst.GetProperty("is_verified").ValueKind is JsonValueKind.True or JsonValueKind.False);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("bank_name").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("account_number").GetString()));
        }

        if (native.Count > 0)
        {
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.BankName));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.AccountNumber));
        }
    }

    [BitflyerWithdrawNegativeLiveFact]
    public async Task Withdraw_NegativeLiveContract()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var bankAccountsCall = await client.Private!.GetBankAccountsAsync(new GetBankAccountsRequest());
        Assert.True(bankAccountsCall.IsSuccess, bankAccountsCall.Error?.Message);
        Assert.NotNull(bankAccountsCall.Response);

        var verifiedBankAccounts = bankAccountsCall.Response!.Where(account => account.IsVerified).ToArray();
        Assert.NotEmpty(verifiedBankAccounts);
        var bankAccount = verifiedBankAccounts[0];

        var withdrawCall = await client.Private.WithdrawAsync(new WithdrawRequest
        {
            CurrencyCode = "JPY",
            BankAccountId = bankAccount.Id,
            Amount = 12000m,
            Code = "999999",
        });

        Assert.False(withdrawCall.IsSuccess);
        Assert.NotNull(withdrawCall.Error);
        Assert.Equal(CallErrorKinds.Http, withdrawCall.Error!.Kind);

        var protocolCall = Assert.IsType<CallResult<ProtocolRequest, ProtocolResponse>>(Assert.Single(withdrawCall.Meta.Children!));
        Assert.NotNull(protocolCall.Response);
        Assert.NotEqual(200, protocolCall.Response!.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(protocolCall.Response.BodyText));

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;

        Assert.True(root.GetProperty("status").GetInt64() < 0);
        Assert.False(root.TryGetProperty("message_id", out _));
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("error_message").GetString()));
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCoinIns_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCoinInsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetCoinInsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCoinInsAsync(request.Count, request.Before, request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.True(protocolFirst.GetProperty("amount").GetDecimal() > 0);
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("event_date").ValueKind);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("event_date").GetString()!), nativeFirst.EventDate);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCoinOuts_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCoinOutsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetCoinOutsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCoinOutsAsync(request.Count, request.Before, request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.True(protocolFirst.GetProperty("amount").GetDecimal() > 0);
            Assert.True(protocolFirst.GetProperty("fee").GetDecimal() >= 0);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.True(nativeFirst.Fee >= 0);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("event_date").GetString()!), nativeFirst.EventDate);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetDeposits_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetDepositsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetDepositsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetDepositsAsync(request.Count, request.Before, request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.True(protocolFirst.GetProperty("amount").GetDecimal() > 0);
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("event_date").ValueKind);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("event_date").GetString()!), nativeFirst.EventDate);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetWithdrawals_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetWithdrawalsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetWithdrawalsAsync(request);
        var protocolCall = await client.Protocol.Private!.GetWithdrawalsAsync(request.Count, request.Before, request.After, request.MessageId);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.True(protocolFirst.GetProperty("amount").GetDecimal() > 0);
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("event_date").ValueKind);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("event_date").GetString()!), nativeFirst.EventDate);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBalanceHistory_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetBalanceHistoryRequest { Count = 10 };
        var nativeCall = await client.Private!.GetBalanceHistoryAsync(request);
        var protocolCall = await client.Protocol.Private!.GetBalanceHistoryAsync(request.CurrencyCode, request.Count, request.Before, request.After);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("currency_code").GetString()));
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("trade_date").ValueKind);
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("event_date").ValueKind);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.Equal(ParseJstNoOffsetTimestamp(protocolFirst.GetProperty("trade_date").GetString()!), nativeFirst.TradeDate);
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("event_date").GetString()!), nativeFirst.EventDate);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetParentOrders_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetParentOrdersRequest { ProductCode = ProductCodes.BtcJpy, Count = 10 };
        var nativeCall = await client.Private!.GetParentOrdersAsync(request);
        var protocolCall = await client.Protocol.Private!.GetParentOrdersAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ParentOrderState is { } parentOrderState ? ApiStringEnum<BitflyerOrderState>.Format(parentOrderState) : null);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            var protocolFirst = root[0];
            Assert.True(protocolFirst.GetProperty("id").GetInt64() > 0);
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("parent_order_id").GetString()));
            Assert.False(string.IsNullOrWhiteSpace(protocolFirst.GetProperty("product_code").GetString()));
            Assert.Equal(JsonValueKind.String, protocolFirst.GetProperty("parent_order_date").ValueKind);
        }

        if (native.Count > 0)
        {
            var protocolFirst = root[0];
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.ParentOrderId));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.ProductCode));
            Assert.Equal(ParseUtcNoOffsetTimestamp(protocolFirst.GetProperty("parent_order_date").GetString()!), nativeFirst.ParentOrderDate);
        }
    }

    [BitflyerWriteLiveFact]
    public async Task SendParentOrder_GetParentOrder_CancelParentOrder_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var tickerCall = await client.Public.GetTickerAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        Assert.True(tickerCall.IsSuccess);
        Assert.NotNull(tickerCall.Response);

        var ticker = tickerCall.Response!;
        var limitPrice = Math.Max(1m, decimal.Floor(ticker.Ltp * 0.6m));
        var exitPrice = decimal.Ceiling(ticker.Ltp * 1.4m);
        var orderRequest = new SendParentOrderRequest
        {
            OrderMethod = ParentOrderMethods.Ifd,
            MinuteToExpire = 1,
            TimeInForce = TimeInForces.Gtc,
            Parameters =
            [
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Limit,
                    Side = OrderSides.Buy,
                    Price = limitPrice,
                    Size = 0.001m,
                },
                new SendParentOrderParameter
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ConditionType = ParentOrderConditionTypes.Limit,
                    Side = OrderSides.Sell,
                    Price = exitPrice,
                    Size = 0.001m,
                },
            ],
        };

        string? acceptanceId = null;

        try
        {
            var sendCall = await client.Private!.SendParentOrderAsync(orderRequest);
            Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
            Assert.NotNull(sendCall.Response);
            Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ParentOrderAcceptanceId));

            acceptanceId = sendCall.Response.ParentOrderAcceptanceId;

            CallResult<GetParentOrderRequest, GetParentOrderResponse>? getCall = null;
            for (var attempt = 0; attempt < 5; attempt++)
            {
                getCall = await client.Private.GetParentOrderAsync(new GetParentOrderRequest
                {
                    ParentOrderAcceptanceId = acceptanceId,
                });

                if (getCall.IsSuccess && getCall.Response is not null)
                {
                    break;
                }

                await Task.Delay(250);
            }

            Assert.NotNull(getCall);
            Assert.True(getCall!.IsSuccess, getCall.Error?.Message);
            Assert.NotNull(getCall.Response);

            var parentOrder = getCall.Response!;
            Assert.Equal(acceptanceId, parentOrder.ParentOrderAcceptanceId);
            Assert.Equal(ParentOrderMethods.Ifd, parentOrder.OrderMethod);
            Assert.Equal(2, parentOrder.Parameters.Count);
            Assert.Equal(ProductCodes.BtcJpy, parentOrder.Parameters[0].ProductCode);
            Assert.Equal(ParentOrderConditionTypes.Limit, parentOrder.Parameters[0].ConditionType);
            Assert.Equal(OrderSides.Buy, parentOrder.Parameters[0].Side);
            Assert.Equal(limitPrice, parentOrder.Parameters[0].Price);
            Assert.Equal(0.001m, parentOrder.Parameters[0].Size);
            Assert.Equal(0m, parentOrder.Parameters[0].TriggerPrice);
            Assert.Equal(0m, parentOrder.Parameters[0].Offset);
            Assert.Equal(ProductCodes.BtcJpy, parentOrder.Parameters[1].ProductCode);
            Assert.Equal(ParentOrderConditionTypes.Limit, parentOrder.Parameters[1].ConditionType);
            Assert.Equal(OrderSides.Sell, parentOrder.Parameters[1].Side);
            Assert.Equal(exitPrice, parentOrder.Parameters[1].Price);
            Assert.Equal(0.001m, parentOrder.Parameters[1].Size);
            Assert.Equal(0m, parentOrder.Parameters[1].TriggerPrice);
            Assert.Equal(0m, parentOrder.Parameters[1].Offset);
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(acceptanceId))
            {
                var cancelCall = await client.Private!.CancelParentOrderAsync(new CancelParentOrderRequest
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ParentOrderAcceptanceId = acceptanceId,
                });

                Assert.True(cancelCall.IsSuccess, cancelCall.Error?.Message);
            }
        }
    }

    [BitflyerWriteLiveFact]
    public async Task SendChildOrder_CancelChildOrder_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var tickerCall = await client.Public.GetTickerAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        Assert.True(tickerCall.IsSuccess);
        Assert.NotNull(tickerCall.Response);

        var ticker = tickerCall.Response!;
        var limitPrice = Math.Max(1m, decimal.Floor(ticker.Ltp * 0.6m));
        var orderRequest = new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Limit,
            Side = OrderSides.Buy,
            Price = limitPrice,
            Size = 0.001m,
            MinuteToExpire = 1,
            TimeInForce = TimeInForces.Gtc,
        };

        string? acceptanceId = null;

        try
        {
            var sendCall = await client.Private!.SendChildOrderAsync(orderRequest);
            Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
            Assert.NotNull(sendCall.Response);
            Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ChildOrderAcceptanceId));

            acceptanceId = sendCall.Response.ChildOrderAcceptanceId;
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(acceptanceId))
            {
                var cancelCall = await client.Private!.CancelChildOrderAsync(new CancelChildOrderRequest
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ChildOrderAcceptanceId = acceptanceId,
                });

                Assert.True(cancelCall.IsSuccess, cancelCall.Error?.Message);
            }
        }
    }

    [BitflyerCancelAllWriteLiveFact]
    public async Task SendChildOrders_CancelAllChildOrders_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClientBundle(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var activeBefore = await GetActiveChildOrdersAsync(client, ProductCodes.BtcJpy);
        if (activeBefore.Count > 0)
        {
            throw new InvalidOperationException("BTC_JPY active child orders must be empty before running CancelAllChildOrders live test.");
        }

        var tickerCall = await client.Public.GetTickerAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        Assert.True(tickerCall.IsSuccess);
        Assert.NotNull(tickerCall.Response);

        var ticker = tickerCall.Response!;
        var buyPrice = Math.Max(1m, decimal.Floor(ticker.Ltp * 0.60m));
        var sellPrice = decimal.Ceiling(ticker.Ltp * 1.40m);
        var createdAcceptanceIds = new List<string>();

        try
        {
            foreach (var (side, price) in new[]
            {
                (OrderSides.Buy, buyPrice),
                (OrderSides.Sell, sellPrice),
            })
            {
                var sendCall = await client.Private!.SendChildOrderAsync(new SendChildOrderRequest
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ChildOrderType = ChildOrderTypes.Limit,
                    Side = side,
                    Price = price,
                    Size = 0.001m,
                    MinuteToExpire = 1,
                    TimeInForce = TimeInForces.Gtc,
                });

                Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
                Assert.NotNull(sendCall.Response);
                Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ChildOrderAcceptanceId));
                createdAcceptanceIds.Add(sendCall.Response.ChildOrderAcceptanceId);
            }

            IReadOnlyList<GetChildOrders.Item>? activeOrders = null;
            for (var attempt = 0; attempt < 10; attempt++)
            {
                activeOrders = await GetActiveChildOrdersAsync(client, ProductCodes.BtcJpy);
                if (createdAcceptanceIds.All(id =>
                        activeOrders.Any(order => string.Equals(order.ChildOrderAcceptanceId, id, StringComparison.Ordinal))))
                {
                    break;
                }

                await Task.Delay(250);
            }

            Assert.NotNull(activeOrders);
            Assert.All(createdAcceptanceIds, id =>
                Assert.Contains(activeOrders!, order => string.Equals(order.ChildOrderAcceptanceId, id, StringComparison.Ordinal)));

            var cancelAllCall = await client.Private!.CancelAllChildOrdersAsync(new CancelAllChildOrdersRequest
            {
                ProductCode = ProductCodes.BtcJpy,
            });

            Assert.True(cancelAllCall.IsSuccess, cancelAllCall.Error?.Message);

            for (var attempt = 0; attempt < 10; attempt++)
            {
                activeOrders = await GetActiveChildOrdersAsync(client, ProductCodes.BtcJpy);
                if (createdAcceptanceIds.All(id =>
                        activeOrders.All(order => !string.Equals(order.ChildOrderAcceptanceId, id, StringComparison.Ordinal))))
                {
                    break;
                }

                await Task.Delay(250);
            }

            Assert.NotNull(activeOrders);
            Assert.All(createdAcceptanceIds, id =>
                Assert.DoesNotContain(activeOrders!, order => string.Equals(order.ChildOrderAcceptanceId, id, StringComparison.Ordinal)));
        }
        finally
        {
            var remainingActiveOrders = await GetActiveChildOrdersAsync(client, ProductCodes.BtcJpy);
            foreach (var acceptanceId in createdAcceptanceIds.Where(id =>
                         remainingActiveOrders.Any(order => string.Equals(order.ChildOrderAcceptanceId, id, StringComparison.Ordinal))))
            {
                var cancelCall = await client.Private!.CancelChildOrderAsync(new CancelChildOrderRequest
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ChildOrderAcceptanceId = acceptanceId,
                });

                Assert.True(cancelCall.IsSuccess, cancelCall.Error?.Message);
            }
        }
    }

    private static BitflyerClientOptions CreateOptions(BitflyerLiveTestSettings settings)
    {
        return new BitflyerClientOptions
        {
            BaseUri = settings.BaseUri,
            ApiCredentialProvider = settings.ApiCredentialProvider,
            EnableProtocolDebugLogging = settings.EnableProtocolDebugLogging,
            ProtocolDebugLogDirectory = settings.ProtocolDebugLogDirectory,
        };
    }

    private static async Task<IReadOnlyList<GetChildOrders.Item>> GetActiveChildOrdersAsync(
        BitflyerNativeBundle client,
        string productCode)
    {
        var activeOrdersCall = await client.Private!.GetChildOrdersAsync(new GetChildOrdersRequest
        {
            ProductCode = productCode,
            ChildOrderState = ChildOrderStates.Active,
        });

        Assert.True(activeOrdersCall.IsSuccess, activeOrdersCall.Error?.Message);
        Assert.NotNull(activeOrdersCall.Response);
        return activeOrdersCall.Response!;
    }

    private static DateTimeOffset ParseUtcNoOffsetTimestamp(string raw)
    {
        var parsed = DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None);
        return new DateTimeOffset(DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified), TimeSpan.Zero);
    }

    private static DateTimeOffset ParseJstNoOffsetTimestamp(string raw)
    {
        var parsed = DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None);
        return new DateTimeOffset(DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified), TimeSpan.FromHours(9))
            .ToOffset(TimeSpan.Zero);
    }
}
