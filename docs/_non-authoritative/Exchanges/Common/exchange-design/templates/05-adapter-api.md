# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# 05 Adapter (Common) Template (DesignContract準拠)

本書は、Raw / Wire(Normalized) を Common API に抽象化する Adapter 層の正本テンプレである。

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

Operation は定数として集中管理する（取引所ごとに Operations.cs を持つ）。

```csharp
internal static class Operations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bitflyer.MarketData.GetTicker";
    }
}
```

### 1.3 Parsing ルール

- Adapter での parsing は原則禁止
- parsing が必要な場合は Wire（Normalized）で行う（Try-style + context 付き例外）

---

## 2. Mapper（Wire → Common DTO）

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

## 4. Trading / Account Adapter の注意点（必須）

- Common Request → Raw Request 変換は Adapter の責務
- request body は型付き DTO（Dictionary 直書きは禁止）
- `Price/Size` は entry point 以外で string に戻さない（Parse/OrThrow 方針）

---

## 5. 禁止事項

- Raw DTO / Wire DTO を外に返す
- 取引所固有の enum / 文字列を Common に漏らす
- 例外を握りつぶす / 再分類する
