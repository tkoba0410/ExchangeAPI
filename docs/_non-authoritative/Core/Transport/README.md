# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Common.Transport

`Common.Transport` は、
**ExchangeAPI における HTTP 通信・再試行・レート制御・可観測性を含む通信基盤**を提供する。

このレイヤは、
- 上位レイヤ（Factory / Exchange 実装）から HTTP 実装を隠蔽し
- 通信失敗を `Common.Contracts` に従って正規化し
- 安定した再試行・制御・観測を提供する

ことを目的とする。

---

## このレイヤの責務

Common.Transport の責務は以下に集約される。

- HTTP リクエスト/レスポンスの送受信
- Retry / RateLimit / CircuitBreaker / Timeout の適用
- エラーの正規化（`ExchangeApiException`）
- ログ・メトリクス・トレースの発行

**取引所ドメインの知識は持たない。**

---

## レイヤ構成（概要）

```
IRestClient
   |
   v
[ Policy Pipeline ]
   |
   v
IHttpTransport
   |
   v
HttpClient
```

- `IRestClient` : 上位レイヤが利用する唯一の通信 API
- Policy Pipeline : 再試行・制御・遮断
- `IHttpTransport` : 実際の HTTP 送信

---

## 主な構成要素

### Protocol

- `IRestClient` / `RestClient`
- `IRequestSigner`
- `IErrorPayloadParser`
- `IExchangeErrorClassifier`

リクエスト構築・署名・レスポンス解釈を担当する。

---

### Policy

- `IHttpPolicy`
- `HttpPolicyPipeline`
- Retry / RateLimit / CircuitBreaker / Timeout
- `HttpPolicyFactory` / `HttpPolicyOptions`

通信の安定性とシステム保護を担う。

判断基準は `Common.Contracts` に従う。

---

### Transport

- `IHttpTransport`
- `HttpTransport`

HttpClient を用いた実送信を担当する。

---

### Observability

- `IRestClientLogger`
- `IRestCallObserver`
- `RestCallContext`
- OpenTelemetry / Metrics 実装

ログ・メトリクス・トレースを提供する。

---

### Time

- `IExchangeClock`
- `SystemClock`

テスト可能な時刻取得を提供する。

---

## Common.Contracts との関係

Common.Transport は、
`Common.Contracts` に定義された以下の契約を **必ず遵守**する。

- `ExchangeApiException`
- `ExchangeErrorCategory`
- ErrorMapping / RetryDecision

Transport 実装は、
**独自のエラー解釈や Retry 判断を行ってはならない。**

---

## 含まれないもの

- 取引所固有の API パス
- ドメイン DTO（Order / Balance 等）
- 取引所ごとの認証方式の詳細

これらは上位（Exchange.*）の責務である。

---

## 次に読むべき文書

- `Architecture.md` : レイヤ構造と依存関係
- `RestClient.md` : RestClient の使い方と拡張点
- `Policies.md` : Policy の構成と設定
- `Observability.md` : ログ・メトリクス
- `HttpTransport.md` : HttpClient の扱い

---

## まとめ

- Common.Transport は **通信基盤の中核**
- エラー・再試行・観測を一貫して提供する
- ドメイン知識は持たず、契約に従う

