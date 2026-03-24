using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
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
using ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests;

public sealed class LiveTests
{
    [BitflyerPublicReadLiveFact]
    public async Task GetMarkets_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        var nativeCall = await client.Public.GetMarketsCallAsync(new GetMarketsRequest());
        var protocolCall = await client.Protocol.Public.GetMarketsCallAsync();

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
            Assert.Equal(protocolItem.GetProperty("market_type").GetString(), nativeItem.MarketType);
        }
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetTicker_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetTickerCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetTickerCallAsync(ProductCodes.BtcJpy);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(root.GetProperty("product_code").GetString(), native.ProductCode);
        Assert.Equal(root.GetProperty("state").GetString(), native.State);
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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetBoardRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetBoardCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetBoardCallAsync(ProductCodes.BtcJpy);

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetExecutionsPublicRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
        };

        var nativeCall = await client.Public.GetExecutionsCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetExecutionsCallAsync(
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
        Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Side));
        Assert.True(nativeFirst.Price > 0);
        Assert.True(nativeFirst.Size > 0);
        Assert.False(string.IsNullOrWhiteSpace(nativeFirst.BuyChildOrderAcceptanceId));
        Assert.False(string.IsNullOrWhiteSpace(nativeFirst.SellChildOrderAcceptanceId));
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetBoardState_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetBoardStateRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetBoardStateCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetBoardStateCallAsync(request.ProductCode);

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
        Assert.False(string.IsNullOrWhiteSpace(native.Health));
        Assert.False(string.IsNullOrWhiteSpace(native.State));

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetHealthRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetHealthCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetHealthCallAsync(request.ProductCode);

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
        Assert.False(string.IsNullOrWhiteSpace(native.Status));
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetFundingRate_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetFundingRateRequest { ProductCode = ProductCodes.FxBtcJpy };

        var nativeCall = await client.Public.GetFundingRateCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetFundingRateCallAsync(request.ProductCode);

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
        Assert.True(native.NextFundingRateSettleDate != default);
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetCorporateLeverage_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        var nativeCall = await client.Public.GetCorporateLeverageCallAsync(new GetCorporateLeverageRequest());
        var protocolCall = await client.Protocol.Public.GetCorporateLeverageCallAsync();

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
        Assert.True(native.CurrentStartDate != default);

        if (root.TryGetProperty("next_max", out var nextMax))
        {
            Assert.True(nextMax.GetDecimal() > 0);
        }

        if (native.NextMax is not null)
        {
            Assert.True(native.NextMax.Value > 0);
        }
    }

    [BitflyerPublicReadLiveFact]
    public async Task GetChats_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        var nativeCall = await client.Public.GetChatsCallAsync(new GetChatsRequest());
        var protocolCall = await client.Protocol.Public.GetChatsCallAsync(null);

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetBalanceCallAsync(new GetBalanceRequest());
        var protocolCall = await client.Protocol.Private!.GetBalanceCallAsync();

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetPositionsRequest { ProductCode = ProductCodes.FxBtcJpy };
        var nativeCall = await client.Private!.GetPositionsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetPositionsCallAsync(ProductCodes.FxBtcJpy);

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
            Assert.Equal(protocolItem.GetProperty("side").GetString(), nativeItem.Side);
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("commission").GetDecimal(), nativeItem.Commission);
            Assert.Equal(protocolItem.GetProperty("swap_point_accumulate").GetDecimal(), nativeItem.SwapPointAccumulate);
            Assert.Equal(protocolItem.GetProperty("require_collateral").GetDecimal(), nativeItem.RequireCollateral);
            Assert.Equal(protocolItem.GetProperty("open_date").GetDateTimeOffset(), nativeItem.OpenDate);
            Assert.Equal(protocolItem.GetProperty("leverage").GetDecimal(), nativeItem.Leverage);
            Assert.Equal(protocolItem.GetProperty("pnl").GetDecimal(), nativeItem.Pnl);
            Assert.Equal(protocolItem.GetProperty("sfd").GetDecimal(), nativeItem.Sfd);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateral_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetCollateralCallAsync(new GetCollateralRequest());
        var protocolCall = await client.Protocol.Private!.GetCollateralCallAsync();

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
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateralAccounts_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetCollateralAccountsCallAsync(new GetCollateralAccountsRequest());
        var protocolCall = await client.Protocol.Private!.GetCollateralAccountsCallAsync();

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetChildOrdersRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
            ChildOrderState = ChildOrderStates.Completed,
        };

        var nativeCall = await client.Private!.GetChildOrdersCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetChildOrdersCallAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ChildOrderState,
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
            Assert.Equal(protocolItem.GetProperty("side").GetString(), nativeItem.Side);
            Assert.Equal(protocolItem.GetProperty("child_order_type").GetString(), nativeItem.ChildOrderType);
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("average_price").GetDecimal(), nativeItem.AveragePrice);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("child_order_state").GetString(), nativeItem.ChildOrderState);
            Assert.Equal(protocolItem.GetProperty("expire_date").GetDateTimeOffset(), nativeItem.ExpireDate);
            Assert.Equal(protocolItem.GetProperty("child_order_date").GetDateTimeOffset(), nativeItem.ChildOrderDate);
            Assert.Equal(protocolItem.GetProperty("child_order_acceptance_id").GetString(), nativeItem.ChildOrderAcceptanceId);
            Assert.Equal(protocolItem.GetProperty("outstanding_size").GetDecimal(), nativeItem.OutstandingSize);
            Assert.Equal(protocolItem.GetProperty("cancel_size").GetDecimal(), nativeItem.CancelSize);
            Assert.Equal(protocolItem.GetProperty("executed_size").GetDecimal(), nativeItem.ExecutedSize);
            Assert.Equal(protocolItem.GetProperty("total_commission").GetDecimal(), nativeItem.TotalCommission);
            Assert.Equal(protocolItem.GetProperty("time_in_force").GetString(), nativeItem.TimeInForce);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetExecutions_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetExecutionsRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            Count = 10,
        };

        var nativeCall = await client.Private!.GetExecutionsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetExecutionsCallAsync(
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
            Assert.Equal(protocolItem.GetProperty("side").GetString(), nativeItem.Side);
            Assert.Equal(protocolItem.GetProperty("price").GetDecimal(), nativeItem.Price);
            Assert.Equal(protocolItem.GetProperty("size").GetDecimal(), nativeItem.Size);
            Assert.Equal(protocolItem.GetProperty("commission").GetDecimal(), nativeItem.Commission);
            Assert.Equal(protocolItem.GetProperty("exec_date").GetDateTimeOffset(), nativeItem.ExecDate);
            Assert.Equal(protocolItem.GetProperty("child_order_acceptance_id").GetString(), nativeItem.ChildOrderAcceptanceId);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCollateralHistory_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCollateralHistoryRequest
        {
            Count = 10,
        };

        var nativeCall = await client.Private!.GetCollateralHistoryCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCollateralHistoryCallAsync(request.Count, request.Before, request.After);

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
            Assert.Equal(protocolItem.GetProperty("date").GetDateTimeOffset(), nativeItem.Date);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetTradingCommission_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetTradingCommissionRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        };

        var nativeCall = await client.Private!.GetTradingCommissionCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetTradingCommissionCallAsync(request.ProductCode);

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

    [BitflyerWriteLiveFact]
    public async Task SendChildOrder_CancelChildOrder_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var tickerCall = await client.Public.GetTickerCallAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
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
            var sendCall = await client.Private!.SendChildOrderCallAsync(orderRequest);
            Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
            Assert.NotNull(sendCall.Response);
            Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ChildOrderAcceptanceId));

            acceptanceId = sendCall.Response.ChildOrderAcceptanceId;
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(acceptanceId))
            {
                var cancelCall = await client.Private!.CancelChildOrderCallAsync(new CancelChildOrderRequest
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
            Credentials = settings.Credentials,
            EnableProtocolDebugLogging = settings.EnableProtocolDebugLogging,
            ProtocolDebugLogDirectory = settings.ProtocolDebugLogDirectory,
        };
    }
}
