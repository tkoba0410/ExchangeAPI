# 06 Factory / Composition Template (Public/Private + Bundle)

目的：
- Core(Rest/Http) を組み立てる
- Raw → Wire → Adapter を束ねる
- `ExchangeClient` 実体を返す（client は Raw/Wire を implements しない）

## 例：Public / Private 生成

```csharp
public static class BitflyerClientFactory
{
    public static IExchangeClient CreatePublic(BitflyerClientOptions options)
    {
        var rest = BuildRestClient(options);

        // Raw
        var rawMarket = new BitflyerRawMarketDataApi(rest);
        IBitflyerRawApi raw = new BitflyerRawApi(rawMarket);

        // Wire
        var wireMarket = new BitflyerWireMarketDataApi(raw.MarketData);
        IBitflyerWireApi wire = new BitflyerWireApi(wireMarket);

        // Adapter(Common)
        IMarketDataApi marketData = new BitflyerMarketDataApi(wire.MarketData);
        IExchangeInfoApi exchangeInfo = new BitflyerExchangeInfoApi(/* ... */);

        ITradingApi trading = new NotSupportedTradingApi(ExchangeCode.Bitflyer);
        IAccountApi account = new NotSupportedAccountApi(ExchangeCode.Bitflyer);

        return new BitflyerExchangeClient(
            trading, account, marketData, exchangeInfo,
            rawBundle: raw,
            wireBundle: wire
        );
    }

    public static IExchangeClient CreatePrivate(BitflyerCredentials creds, BitflyerClientOptions options)
    {
        var rest = BuildRestClient(options, creds);

        // Raw (Public+Private を bundle)
        var rawMarket = new BitflyerRawMarketDataApi(rest);
        var rawTrading = new BitflyerRawTradingApi(rest);
        IBitflyerRawApi raw = new BitflyerRawApi(rawMarket /*, rawTrading ...*/);

        // Wire
        var wireMarket = new BitflyerWireMarketDataApi(raw.MarketData);
        var wireTrading = new BitflyerWireTradingApi(/* rawTrading ... */);
        IBitflyerWireApi wire = new BitflyerWireApi(wireMarket /*, wireTrading ...*/);

        // Adapter(Common)
        IMarketDataApi marketData = new BitflyerMarketDataApi(wire.MarketData);
        ITradingApi trading = new BitflyerTradingApi(/* wireTrading + mapper + errorMapper */);
        IAccountApi account = new BitflyerAccountApi(/* ... */);
        IExchangeInfoApi exchangeInfo = new BitflyerExchangeInfoApi(/* ... */);

        return new BitflyerExchangeClient(
            trading, account, marketData, exchangeInfo,
            rawBundle: raw,
            wireBundle: wire
        );
    }
}
```

## ポイント

- Public は Trading/Account を NotSupported で良い（利用者が迷わない）
- Private は認証入り rest を使う
- client は Raw/Wire を implements せず、bundle を内部保持し `Raw<T>()/Wire<T>()` で取り出す
