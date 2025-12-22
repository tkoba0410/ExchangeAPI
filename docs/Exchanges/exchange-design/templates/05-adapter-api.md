# 05 Adapter (Common) Template (DesignContract準拠)

本書は、Raw / Wire(Normalized) を Common API に抽象化する Adapter 層の正本テンプレである。

---

## 0. Adapter 層の位置づけ（再確認）

Adapter 層は：
- **Common API（ITradingApi / IMarketDataApi / IAccountApi / IExchangeInfoApi）を実装**
- Raw / Wire の差分を吸収し、Common DTO を返す
- 例外を **必ず Enrich** して上位に伝える

---

## 1. Adapter が守るべき絶対ルール

### 1.1 例外 Enrich（必須）

Adapter 公開メソッドでは、`ExchangeCode` と `Operation` を必ず例外に付与する。

```csharp
catch (ExchangeApiException ex)
{
    throw ex.Enrich(
        exchange: ExchangeCode.Bitflyer,
        operation: Operations.MarketData.GetTicker
    );
}
```

### 1.2 Operation 命名規則（固定）

形式：

```text
<Exchange>.<Area>.<Method>
```

Operation は定数として集中管理する。

```csharp
internal static class Operations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bitflyer.MarketData.GetTicker";
    }
}
```

### 1.3 Parsing ルール（再掲）

- Adapter での parsing は原則禁止
- parsing が必要な場合は Wire（Normalized）で行う（Try-style + context 付き例外）

---

## 2. Common DTO 変換テンプレ

### 2.1 Common DTO 例（既存前提）

```csharp
public sealed record Ticker(
    Price BestBid,
    Price BestAsk,
    Price Last,
    Size Volume,
    DateTimeOffset Timestamp
);
```

### 2.2 Wire → Common Mapper（専用クラス）

```csharp
internal static class BitflyerMapper
{
    public static Ticker ToCommon(WireTicker wire)
        => new(
            BestBid: Price.FromDecimal(wire.BestBid),
            BestAsk: Price.FromDecimal(wire.BestAsk),
            Last: Price.FromDecimal(wire.Ltp),
            Volume: Size.FromDecimal(wire.Volume),
            Timestamp: wire.Timestamp
        );
}
```

---

## 3. Adapter 実装テンプレ（MarketData 例）

```csharp
internal sealed class BitflyerMarketDataApi : IMarketDataApi
{
    private readonly IBitflyerWireMarketDataApi _wire;

    public BitflyerMarketDataApi(IBitflyerWireMarketDataApi wire)
        => _wire = wire;

    public async Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken ct = default)
    {
        try
        {
            var wireTicker = await _wire.GetTickerAsync(symbol.Value, ct);
            return BitflyerMapper.ToCommon(wireTicker);
        }
        catch (ExchangeApiException ex)
        {
            throw ex.Enrich(
                exchange: ExchangeCode.Bitflyer,
                operation: Operations.MarketData.GetTicker
            );
        }
    }
}
```

---

## 4. Trading / Account Adapter の注意点

- Common Request → Raw Request 変換は Adapter の責務
- request body は型付き DTO（Dictionary 直書きは禁止）

---

## 5. Adapter がやってはいけないこと（禁止）

- Raw DTO / Wire DTO を外に返す
- 取引所固有の enum / 文字列を Common に漏らす
- 例外を握りつぶす / 再分類する
