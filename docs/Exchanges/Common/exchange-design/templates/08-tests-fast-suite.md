# 08 テスト（Fast Suite）テンプレート

目的：
- 設計契約（境界・例外・JSON形）を最小限のテストで固定する
- integration はオプトインとし、fast suite は常時実行

## 最低限含めるべきテスト（推奨）

### A. 例外の Enrich が含まれること
- Adapter の公開メソッドで投げられる例外に
  - ExchangeCode
  - Operation
  が含まれていること

### B. パース失敗メッセージに「文脈」があること
- Wire の Try-parse 失敗が
  - どの API
  - どのフィールド
  - 入力値
  を含むこと

### C. Request DTO の JSON 形が壊れていないこと
- `CreateOrderRequest` のキーが維持されていること
- enum のシリアライズ表現が変わらないこと

### D. Raw DTO の strict enum（Closed set）
- 未知値で fail fast すること（必要箇所に限定）

## 実装例（擬似コード）

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

- Fast suite：常時実行
- Live suite：環境変数でオプトイン（APIキー等）
