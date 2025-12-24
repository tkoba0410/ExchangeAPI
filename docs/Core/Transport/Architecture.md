# Architecture

このドキュメントは、
`Common.Transport` の **構造・責務境界・依存方向**を定義する。

---

## レイヤの全体像

`Common.Transport` は、次の層で構成される。

```
[ Upper Layers ]
  Factory / Exchange.*
        |
        v
    IRestClient
        |
        v
  HttpPolicyPipeline
        |
        v
   IHttpTransport
        |
        v
     HttpClient
```

- 上位レイヤは `IRestClient` のみを利用する
- HTTP 実装詳細は `IHttpTransport` 以下に閉じ込める

---

## 主要コンポーネントと責務

### Protocol（リクエスト/レスポンスの意味づけ）

- `IRestClient` / `RestClient`
- `IRequestSigner`
- `IErrorPayloadParser`
- `IExchangeErrorClassifier`

責務：
- リクエストの構築
- 認証/署名
- レスポンス（成功/失敗）の解釈
- エラーの正規化（Category の決定）

※ 取引所固有の仕様はここに直接埋め込まず、
上位から差し替え可能な拡張点として提供する。

---

### Policy（安定性とシステム保護）

- `IHttpPolicy`
- `HttpPolicyPipeline`
- Retry / RateLimit / CircuitBreaker / Timeout
- `HttpPolicyFactory` / `HttpPolicyOptions`
- `IPolicyObserver`

責務：
- 再試行（Retry）
- レート制御（RateLimit）
- 障害遮断（CircuitBreaker）
- タイムアウト（Timeout）
- 観測（PolicyObserver）

Policy は **判断基準を持たない**。
判断基準は `Common.Contracts`（RetryDecision）に従う。

---

### Transport（送信の実体）

- `IHttpTransport`
- `HttpTransport`

責務：
- HttpClient による送信
- 送受信の最小抽象化

Transport は「HTTP を送る」だけであり、
ドメインや Retry 戦略を持たない。

---

### Observability（ログ/メトリクス/トレース）

- `IRestClientLogger`
- `IRestCallObserver`
- `RestCallContext`
- `RestCallOpenTelemetryObserver`
- `RestCallMetricsObserver`

責務：
- 失敗/成功のログ出力
- 呼び出し単位の計測とトレース

ログと観測は **副作用として独立**し、
送信結果や Retry 戦略に影響を与えない。

---

### Time（テスト容易性のための時刻）

- `IExchangeClock`
- `SystemClock`

責務：
- テスト可能な時刻取得

---

## 依存方向（重要）

依存は常に **上位 → 下位** でなければならない。

- Protocol → Policy → Transport
- Observability は横断だが「結果を変えない」

禁止：
- Transport が Policy を参照する
- Policy が Exchange.*（取引所実装）を参照する

---

## Common.Contracts との関係

`Common.Transport` は、
`Common.Contracts` を **契約として参照**する。

- `ExchangeApiException` による失敗の伝達
- `ExchangeErrorCategory` による意味分類
- `ErrorMapping` に従う正規化
- `RetryDecision` に従う Policy 挙動

Transport は契約を満たし、
上位は契約に依存する。

---

## 拡張点（プラグイン/取引所差し替えのため）

上位レイヤ（Exchange.*）は、以下を差し替えて取引所差異を吸収できる。

- `IRequestSigner`（署名方式）
- `IErrorPayloadParser`（エラー本文解析）
- `IExchangeErrorClassifier`（Category 判定）
- Observers / Loggers（観測方法）

これにより、Transport 自体はドメインから独立したまま維持できる。

---

## まとめ

- 上位は `IRestClient` のみに依存する
- Policy は安定性提供、判断基準は Contracts に委譲
- Transport は送信のみ
- 観測は副作用で結果に影響しない
- 差し替え点を用意し、取引所差異を吸収する

