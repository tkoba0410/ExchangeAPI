using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
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

    [BitflyerPrivateReadLiveFact]
    public async Task GetPermissions_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetPermissionsCallAsync(new GetPermissionsRequest());
        var protocolCall = await client.Protocol.Private!.GetPermissionsCallAsync();

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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetAddressesCallAsync(new GetAddressesRequest());
        var protocolCall = await client.Protocol.Private!.GetAddressesCallAsync();

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
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Type));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.Address));
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBankAccounts_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetBankAccountsCallAsync(new GetBankAccountsRequest());
        var protocolCall = await client.Protocol.Private!.GetBankAccountsCallAsync();

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
        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var bankAccountsCall = await client.Private!.GetBankAccountsCallAsync(new GetBankAccountsRequest());
        Assert.True(bankAccountsCall.IsSuccess, bankAccountsCall.Error?.Message);
        Assert.NotNull(bankAccountsCall.Response);

        var verifiedBankAccounts = bankAccountsCall.Response!.Where(account => account.IsVerified).ToArray();
        Assert.NotEmpty(verifiedBankAccounts);
        var bankAccount = verifiedBankAccounts[0];

        var withdrawCall = await client.Private.WithdrawCallAsync(new WithdrawRequest
        {
            CurrencyCode = "JPY",
            BankAccountId = bankAccount.Id,
            Amount = 12000m,
            Code = "999999",
        });

        Assert.False(withdrawCall.IsSuccess);
        Assert.NotNull(withdrawCall.Error);
        Assert.Equal(CallErrorKinds.Http, withdrawCall.Error!.Kind);

        var protocolCall = Assert.IsType<Call<ProtocolRequest, ProtocolResponse>>(Assert.Single(withdrawCall.Meta.Children!));
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

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCoinInsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetCoinInsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCoinInsCallAsync(request.Count, request.Before, request.After);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.True(nativeFirst.EventDate != default);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetCoinOuts_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetCoinOutsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetCoinOutsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetCoinOutsCallAsync(request.Count, request.Before, request.After);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.True(nativeFirst.Fee >= 0);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetDeposits_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetDepositsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetDepositsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetDepositsCallAsync(request.Count, request.Before, request.After);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.True(nativeFirst.EventDate != default);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetWithdrawals_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetWithdrawalsRequest { Count = 10 };
        var nativeCall = await client.Private!.GetWithdrawalsCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetWithdrawalsCallAsync(request.Count, request.Before, request.After, request.MessageId);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.Amount > 0);
            Assert.True(nativeFirst.EventDate != default);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBalanceHistory_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetBalanceHistoryRequest { Count = 10 };
        var nativeCall = await client.Private!.GetBalanceHistoryCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetBalanceHistoryCallAsync(request.CurrencyCode, request.Count, request.Before, request.After);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.CurrencyCode));
            Assert.True(nativeFirst.TradeDate != default);
            Assert.True(nativeFirst.EventDate != default);
        }
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetParentOrders_ReadContract()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var request = new GetParentOrdersRequest { ProductCode = ProductCodes.BtcJpy, Count = 10 };
        var nativeCall = await client.Private!.GetParentOrdersCallAsync(request);
        var protocolCall = await client.Protocol.Private!.GetParentOrdersCallAsync(
            request.ProductCode,
            request.Count,
            request.Before,
            request.After,
            request.ParentOrderState);

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
            var nativeFirst = native[0];
            Assert.True(nativeFirst.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.ParentOrderId));
            Assert.False(string.IsNullOrWhiteSpace(nativeFirst.ProductCode));
            Assert.True(nativeFirst.ParentOrderDate != default);
        }
    }

    [BitflyerWriteLiveFact]
    public async Task SendParentOrder_GetParentOrder_CancelParentOrder_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var tickerCall = await client.Public.GetTickerCallAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
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
            var sendCall = await client.Private!.SendParentOrderCallAsync(orderRequest);
            Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
            Assert.NotNull(sendCall.Response);
            Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ParentOrderAcceptanceId));

            acceptanceId = sendCall.Response.ParentOrderAcceptanceId;

            Call<GetParentOrderRequest, GetParentOrderResponse>? getCall = null;
            for (var attempt = 0; attempt < 5; attempt++)
            {
                getCall = await client.Private.GetParentOrderCallAsync(new GetParentOrderRequest
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
                var cancelCall = await client.Private!.CancelParentOrderCallAsync(new CancelParentOrderRequest
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

    [BitflyerCancelAllWriteLiveFact]
    public async Task SendChildOrders_CancelAllChildOrders_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var activeBefore = await GetActiveChildOrdersAsync(client, ProductCodes.BtcJpy);
        if (activeBefore.Count > 0)
        {
            throw new InvalidOperationException("BTC_JPY active child orders must be empty before running CancelAllChildOrders live test.");
        }

        var tickerCall = await client.Public.GetTickerCallAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
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
                var sendCall = await client.Private!.SendChildOrderCallAsync(new SendChildOrderRequest
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

            var cancelAllCall = await client.Private!.CancelAllChildOrdersCallAsync(new CancelAllChildOrdersRequest
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

    private static async Task<IReadOnlyList<GetChildOrders.Item>> GetActiveChildOrdersAsync(
        BitflyerNativeBundle client,
        string productCode)
    {
        var activeOrdersCall = await client.Private!.GetChildOrdersCallAsync(new GetChildOrdersRequest
        {
            ProductCode = productCode,
            ChildOrderState = ChildOrderStates.Active,
        });

        Assert.True(activeOrdersCall.IsSuccess, activeOrdersCall.Error?.Message);
        Assert.NotNull(activeOrdersCall.Response);
        return activeOrdersCall.Response!;
    }
}
