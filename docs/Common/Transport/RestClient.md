# RestClient

このドキュメントは、
`Common.Transport` における **IRestClient / RestClient の役割・使い方・拡張点**を定義する。

RestClient は、
**上位レイヤが直接利用する唯一の HTTP 通信 API**である。

---

## RestClient の責務

RestClient の責務は以下に限定される。

- HTTP リクエストの組み立て
- Policy Pipeline の適用
- 成功レスポンスのデシリアライズ
- 失敗時の例外正規化（ExchangeApiException）
- Observer / Logger への通知

RestClient は、
- Retry 判断
- レート制御
- 取引所固有仕様

を **自ら判断しない**。

---

## 基本的な使用方法

```csharp
IRestClient rest = new RestClient(
    baseUri,
    httpTransport,
    policy: policyPipeline,
    signer: signer,
    observer: observer,
    logger: logger);

var result = await rest.GetAsync<MyDto>("/api/path");
```

- 上位レイヤは HTTP を意識しない
- 成功時は DTO を受け取る
- 失敗時は `ExchangeApiException` が送出される

---

## リクエスト構築

### URI 構築

- パスとクエリを安全に結合する
- 重複キーは値が同一の場合のみ許可する
- 値は URI エンコードされる

この挙動は `RestClientTests` により保証される。

---

### メソッドとボディ

- GET / POST / その他の HTTP メソッドに対応
- POST 系はリクエストボディを JSON シリアライズする

シリアライズ失敗時は `ExchangeApiException` が送出される。

---

## エラー処理

RestClient は、
下位で発生したすべての失敗を `ExchangeApiException` に正規化する。

- HTTP エラーステータス
- 通信例外（HttpRequestException 等）
- JSON 解析失敗

正規化ルールは `Common.Contracts` に従う。

---

## 拡張点

RestClient は、以下のインターフェースを差し替えることで拡張できる。

### IRequestSigner

- リクエスト送信前に署名・認証を付与する
- 取引所ごとの認証方式を吸収する

---

### IErrorPayloadParser

- エラーレスポンス本文を解析する
- 取引所固有のエラーフォーマットを解釈する

---

### IExchangeErrorClassifier

- HTTP ステータスやペイロードを元に
  `ExchangeErrorCategory` を決定する

---

### Observer / Logger

- `IRestCallObserver`
- `IRestClientLogger`

ログ・メトリクス・トレース用途で差し替える。

---

## Observer の通知タイミング

RestClient は以下の順で Observer を通知する。

1. OnRequest
2. OnResponse（成功時）
3. OnError（失敗時）

Observer は副作用のみを持ち、
処理結果に影響を与えてはならない。

---

## テスト容易性

- `IHttpTransport` を差し替えることで
  実通信なしのテストが可能
- Fake / Sequence Transport により
  Retry / Timeout / CircuitBreaker の挙動を検証できる

---

## まとめ

- RestClient は **通信 API の唯一の入口**
- HTTP / Retry / Policy の詳細を隠蔽する
- 失敗は必ず `ExchangeApiException` に正規化する
- 差し替え点を用意し、取引所差異を吸収する

