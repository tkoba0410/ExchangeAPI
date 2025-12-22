# 08 Tests (Fast Suite) Template

目的：
- 設計契約（境界・例外・JSON形）を最小テストで固定する
- integration は opt-in とし、fast suite は常に回る

## 最小で入れるテスト（推奨）

### A. 例外 Enrich が入ること
- Adapter の公開メソッドで投げた例外に
  - ExchangeCode
  - Operation
  が入っていること

### B. parsing 失敗メッセージに “文脈” があること
- Wire の Try-parse 失敗が
  - どのAPI
  - どのフィールド
  - 入力値
  を含むこと

### C. Request DTO の JSON 形が壊れないこと
- `CreateOrderRequest` のキー
- enum のシリアライズ表現

### D. Raw DTO の strict enum（Closed set）
- 未知値で fail fast（必要箇所のみ）

## 実装例（擬似）

```csharp
[Fact]
public async Task MarketData_GetTicker_EnrichesException()
{
    // arrange: wire が ExchangeApiException を投げるスタブ
    // act
    // assert: ex.ExchangeCode == Bitflyer, ex.Operation == Operations.MarketData.GetTicker
}
```

## CI 運用（推奨）

- Fast suite：常に
- Live suite：環境変数 opt-in（APIキー等）
