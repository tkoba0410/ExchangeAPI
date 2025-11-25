# A040-STG2-ARCB bitFlyer Raw API → 抽象層マッピング（get balance）

## 1. 本文書の目的
Stage2 では、bitFlyer Private API の最初のエンドポイントとして
**`/v1/me/getbalance` を抽象インターフェースまでマッピングする仕様**を確立する。

本ドキュメントは、Raw API（bitFlyer 固有形式）からドメインモデル（抽象層）への
**対応表・値変換仕様・例外扱い**を明確にし、後続の Private API 実装のテンプレートとすることを目的とする。

---

## 2. API 定義（bitFlyer Raw）
### 2.1 エンドポイント
- **HTTP Method**: GET
- **Path**: `/v1/me/getbalance`
- **Auth**: 必須（API Key / API Secret / 署名）
- **Query Parameters**: なし

### 2.2 レスポンス（bitFlyer 固有形式）
レスポンスは通貨ごとの配列で返される。

例：
```json
[
  {
    "currency_code": "JPY",
    "amount": 120000.0,
    "available": 100000.0
  },
  {
    "currency_code": "BTC",
    "amount": 0.5,
    "available": 0.4
  }
]
```

### 2.3 DTO（Raw 層で扱う型）
```csharp
public sealed class BalanceResponse
{
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Available { get; set; }
}
```

---

## 3. 抽象層ドメイン（Abstractions）
Stage2 では以下の `Balance` を利用する。

```csharp
public sealed record Balance(
    string Currency,
    decimal Amount,
    decimal Available
);
```

抽象インターフェース：
```csharp
Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct = default);
```

---

## 4. API マッピング表（Raw → Domain）

| Raw フィールド             | Domain フィールド | 変換仕様 | 注意点 |
|---------------------------|-------------------|----------|--------|
| `currency_code` (string)  | `Currency`        | 文字列をそのまま渡す | bitFlyer 固有のコード体系に依存（例: "JPY", "BTC"）。抽象層では解釈しない。 |
| `amount` (decimal)        | `Amount`          | そのままマップ        | 小数精度はそのまま保持する。 |
| `available` (decimal)     | `Available`       | そのままマップ        | 取引所ごとの仕様差異（ロック残高等）は抽象層では扱わない。 |

→ Stage2 時点では「忠実なマッピング」のみ行い、意味的解釈や補正は行わない。

---

## 5. 変換ロジック（Mapper）

```csharp
public static class BitflyerDtoMapper
{
    public static Balance ToBalance(BalanceResponse dto)
        => new(
            dto.CurrencyCode,
            dto.Amount,
            dto.Available
        );
}
```

**責務の原則**：
- 変換は純粋関数であること（例外・ロジックを持たない）。
- bitFlyer の値を可能な限りそのままドメインへ渡す。
- 追加加工（例: 通貨コードの正規化）は Stage3 以降に検討する。

---

## 6. IExchangeClient 実装における流れ

```csharp
public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct)
{
    var dtoList = await _raw.GetBalanceAsync(ct);
    return dtoList.Select(BitflyerDtoMapper.ToBalance).ToList();
}
```

### フロー図
```
BitflyerExchangeClient
   ↓ calls
IBitflyerRawApiClient.GetBalanceAsync
   ↓ calls
RestClient.GetAsync("/v1/me/getbalance")
   ↓
HTTP GET + 署名
   ↓
JSON → BalanceResponse[]
   ↓
Mapper
   ↓
Balance[]（ドメインモデル）
```

---

## 7. 例外扱い
Stage2 ではエラー処理レベルは **E1（最小限）** とし、以下の扱いとする：

| 種別 | 例外化 | 備考 |
|------|--------|------|
| HTTP 400 / 404 / 500 系 | `ExchangeApiException` | RestClient が統一処理として扱う |
| 署名エラー（403） | `ExchangeApiException` | API Key / Secret 不正時 |
| タイムアウト | `ExchangeApiException` | HttpClient 依存 |

bitFlyer 固有のエラーコードの解釈は **Stage3 以降に検討**する。

---

## 8. 今後の拡張に向けた検討
- `CurrencyCode` は他取引所や仮想通貨名の揺れがあるため、将来的に列挙型や正規化対応が必要になる可能性あり。
- リスト系 GET（childorders、executions など）ではクエリ・ページングが発生するため、
  `IRestClient` のインターフェースを拡張する可能性がある。
- `Balance` 以外の口座系 (`Collateral`, `Position`) 追加時にも同じマッピング方式を利用する。

---

## 9. Stage2 完了条件（マッピング観点）
- Raw → Domain の対応が本書通りに実装されている。
- DTO → Domain 変換が正しく行われている。
- `/v1/me/getbalance` を通した実データ取得で、JPY と BTC の値が問題なく `Balance` モデルにマッピングされる。

---

本ドキュメントは、Stage3 以降で追加される Private GET（collateral, positions）が
同じ設計パターンで進められるようにするための基礎仕様となる。

