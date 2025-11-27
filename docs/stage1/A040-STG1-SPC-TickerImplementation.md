---

doc_id: A040-STG1-SPC-TickerImplementation
title: Stage1 実装仕様書（bitFlyer Ticker）
version: 2.0.0
status: Draft
stage: Stage1
-------------

# A040-STG1-SPC-TickerImplementation

Stage1 実装仕様書（bitFlyer Public REST / Ticker）

本書は、Exchange API Library における **Stage1（bitFlyer Public REST `GET /v1/getticker`）** の
具体的な実装仕様（Specification）を定義する。A010（OVR）と A020（REQ）、A030（ARC）で定めた
目的・要求・構造に基づき、IExchangeClient / bitFlyer Adapter / Infrastructure の振る舞いを
詳細に規定する。

---

## 1. 目的（Purpose）

* `IExchangeClient.GetTickerAsync("BTC/JPY")` の呼び出しが、安定して Ticker 情報を返すようにする。
* Abstractions / Adapter / Infrastructure の役割と境界を明示し、実装のぶれをなくす。
* 将来の Stage2（認証 / WebSocket / 複数取引所 / Transport/Protocol 強化）に備えて、
  Stage1 の Ticker 実装を正確に記録しておく。

---

## 2. スコープ（Scope）

本仕様書が対象とするのは、次のコンポーネントに関する **Ticker 取得処理**である。

* Boundary / Abstractions

  * `IExchangeClient`
  * `Ticker` DTO
  * `Symbols`
* Adapter（bitFlyer）

  * `BitflyerExchangeClient`
  * `IBitflyerPublicApi` / その実装
  * `BitflyerTickerRaw`
* Infrastructure（REST/HTTP）

  * `IRestClient` / `RestClient`
  * `IHttpTransport` / `HttpTransport`

その他の API（認証 REST / WebSocket 等）は本書の対象外とし、Stage2 で別途規定する。

---

## 3. インターフェース概要

### 3.1 `IExchangeClient`（Abstractions）

```csharp
public interface IExchangeClient
{
    Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);
}
```

* `symbol`

  * 形式: `"BASE/QUOTE"`（大文字、スラッシュ区切り）
  * Stage1 で必須対応とするのは `"BTC/JPY"` のみ。
* 戻り値

  * `Ticker` DTO（本書 4 章で仕様を定義）。

### 3.2 `Ticker` DTO（Abstractions）

```csharp
public sealed record Ticker(
    string Symbol,
    decimal BestBid,
    decimal BestAsk,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp);
```

* `Symbol`

  * 要求と同一の文字列（例: `"BTC/JPY"`）。
* `BestBid` / `BestAsk`

  * それぞれベストビッド/ベストアスク価格。
* `LastTradedPrice`

  * 最終約定価格。
* `Timestamp`

  * UTC に正規化された日時（`DateTimeKind.Utc` 推奨）。

Volume や `tick_id` などの取引所固有情報は Stage1 では `Ticker` に含めない。
必要に応じて Raw モデルから参照する。

### 3.3 `IBitflyerPublicApi`（Adapter 内部 API）

```csharp
public interface IBitflyerPublicApi
{
    Task<BitflyerTickerRaw> GetTickerRawAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
```

* `productCode`

  * 形式: `"BTC_JPY"`（bitFlyer 仕様に準拠）。
* 戻り値

  * `BitflyerTickerRaw`（bitFlyer の `GET /v1/getticker` レスポンスの写像）。

### 3.4 REST / HTTP インターフェース（Infrastructure）

```csharp
public interface IRestClient
{
    Task<TResponse> GetAsync<TResponse>(
        string path,
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default);
}

public interface IHttpTransport
{
    Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);
}
```

* `IRestClient` は HTTP Method / Path / Query を受け取り、JSON デシリアライズした `TResponse` を返す。
* `IHttpTransport` は `HttpClient` をラップし、実際の HTTP 通信を行う。

---

## 4. Ticker DTO 仕様（詳細）

`Ticker` DTO は Abstractions に属し、次の性質を持つ。

1. **取引所非依存であること**

   * 特定取引所に固有のフィールド名や状態を直接持たない。
2. **Stage1 では 5 フィールドのみ**

   * `Symbol`, `BestBid`, `BestAsk`, `LastTradedPrice`, `Timestamp`。
3. **値の整合性**

   * `Symbol` は引数 `symbol` と同一であること。
   * `Timestamp` は bitFlyer の `timestamp` を UTC に正規化した値とする。

将来 Stage2 で複数取引所や追加情報が必要になった場合、
必要に応じて DTO の拡張を検討するが、Stage1 では最小構成とする。

---

## 5. bitFlyer Raw モデル仕様

### 5.1 HTTP エンドポイント

* メソッド: `GET`
* パス: `/v1/getticker`
* ベース URL: `https://api.bitflyer.com`（本番環境）
* クエリパラメータ:

  * `product_code=BTC_JPY`

### 5.2 要求ヘッダ（推奨）

* `User-Agent`: ライブラリ名 + バージョン（例: `ExchangeApi/1.0`）
* `Accept`: `application/json`

### 5.3 レスポンス JSON の主要フィールド

`BitflyerTickerRaw` は少なくとも次のフィールドを含む。

* `product_code`: string
* `timestamp`: DateTimeOffset
* `tick_id`: long
* `best_bid`: decimal
* `best_ask`: decimal
* `best_bid_size`: decimal
* `best_ask_size`: decimal
* `total_bid_depth`: decimal
* `total_ask_depth`: decimal
* `ltp`: decimal（last traded price）
* `volume`: decimal
* `volume_by_product`: decimal

公式仕様に新フィールドが追加された場合、後方互換性のある範囲で Raw モデルにプロパティを追加してよい。

### 5.4 `BitflyerTickerRaw` クラス例

```csharp
public sealed class BitflyerTickerRaw
{
    public string ProductCode { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; };
    public long TickId { get; init; };
    public decimal BestBid { get; init; };
    public decimal BestAsk { get; init; };
    public decimal BestBidSize { get; init; };
    public decimal BestAskSize { get; init; };
    public decimal TotalBidDepth { get; init; };
    public decimal TotalAskDepth { get; init; };
    public decimal LastTradedPrice { get; init; };
    public decimal Volume { get; init; };
    public decimal VolumeByProduct { get; init; };
}
```

* `Timestamp` は JSON の `timestamp` を DateTimeOffset にパースした値とする（UTC）。
* プロパティ名は C# の慣習に合わせて PascalCase を用いる。

---

## 6. Raw → Ticker マッピング仕様

`BitflyerExchangeClient` は `BitflyerTickerRaw` から `Ticker` へのマッピングを行う。

### 6.1 マッピング規則

* `Ticker.Symbol`

  * 入力 `symbol` をそのまま設定する（例: `"BTC/JPY"`）。
* `Ticker.BestBid`

  * `BitflyerTickerRaw.BestBid` を設定する。
* `Ticker.BestAsk`

  * `BitflyerTickerRaw.BestAsk` を設定する。
* `Ticker.LastTradedPrice`

  * `BitflyerTickerRaw.LastTradedPrice` を設定する（`ltp`）。
* `Ticker.Timestamp`

  * `BitflyerTickerRaw.Timestamp` を UTC 正規化して設定する。

### 6.2 単位・丸め

* 価格・数量は JSON の値をそのまま `decimal` で保持し、Stage1 では丸めや小数点処理を行わない。
* 単位は bitFlyer の仕様に従う（BTC/JPY）。

### 6.3 失敗時の挙動

マッピング中に予期せぬ null や不正値が検出された場合：

* Stage1 では **防御的な null チェックよりも、例外として検出する方針**とする。
* 具体的には、必須フィールドが欠落している場合は `ExchangeApiException` をスローしてよい。

---

## 7. HTTP / 例外ハンドリング仕様

### 7.1 `GetTickerAsync` の正常フロー

1. 呼び出し元が `GetTickerAsync("BTC/JPY", ct)` を呼ぶ。
2. `BitflyerExchangeClient` が `symbol` を検証する。

   * null/空白 → `ArgumentException`。
   * 未対応シンボル → `SymbolNotSupportedException`。
3. `symbol"BTC/JPY"` を `"BTC_JPY"` に変換する。
4. `IBitflyerPublicApi.GetTickerRawAsync("BTC_JPY", ct)` を呼ぶ。
5. `IBitflyerPublicApi` 実装が `IRestClient.GetAsync<BitflyerTickerRaw>` を呼び出す。
6. `IRestClient` が内部で `IHttpTransport.SendAsync` を通じて HTTP リクエストを送信する。
7. 正常な JSON レスポンスが返り、`BitflyerTickerRaw` にデシリアライズされる。
8. `BitflyerExchangeClient` が Raw → Ticker にマッピングして返す。

### 7.2 HTTP レベルエラー

* ネットワークエラー / タイムアウト / DNS エラーなど

  * `ExchangeApiException` として通知する。
* HTTP ステータスコードが 2xx 以外の場合

  * ステータスコードとレスポンス内容を含む情報を `ExchangeApiException` に格納してスローすることが望ましい。

### 7.3 JSON パースエラー

* Content-Type が `application/json` 以外、または JSON デシリアライズに失敗した場合

  * `ExchangeApiException` として通知する。

### 7.4 キャンセル

* `CancellationToken` がキャンセルされた場合

  * 基本的には `TaskCanceledException` か `OperationCanceledException` が呼び出し側に伝播される。
  * `ExchangeApiException` でラップせず、そのまま伝播してよい。

### 7.5 入力エラー

* `symbol` が null または空白

  * `ArgumentException`。
* `symbol` が `"BTC/JPY"` 以外（Stage1 時点）

  * `SymbolNotSupportedException`（`ExchangeApiException` 派生）をスローする。

---

## 8. スレッドセーフティ / ライフタイム

* `IExchangeClient` の実装（`BitflyerExchangeClient`）は、
  複数スレッドからの同時呼び出しに耐えられる実装とする（SHOULD）。
* `HttpClient` インスタンスは DI によって管理され、使い捨てしないことが望ましい（SHOULD）。
* `BitflyerExchangeClient` 自体はステートレス（設定情報を不変として持つ）とし、
  多数の同時呼び出しで共有しても問題ない構造とする。

---

## 9. 使用例（参考）

```csharp
var client = serviceProvider.GetRequiredService<IExchangeClient>();

var ticker = await client.GetTickerAsync(Symbols.BtcJpy, cancellationToken);

Console.WriteLine($"Symbol: {ticker.Symbol}");
Console.WriteLine($"BestBid: {ticker.BestBid}");
Console.WriteLine($"BestAsk: {ticker.BestAsk}");
Console.WriteLine($"LastTradedPrice: {ticker.LastTradedPrice}");
Console.WriteLine($"Timestamp: {ticker.Timestamp:O}");
```

* 上記コードが問題なく動作し、Ticker 情報が取得できることが Stage1 のゴールのひとつである。

---

## 10. Stage1 SPEC DoD（Definition of Done）

本仕様書に関する Stage1 の完了条件は次の通り。

1. `IExchangeClient.GetTickerAsync("BTC/JPY")` が、
   実 API または HTTP モックに対するテストで正常に動作する。
2. Raw モデル `BitflyerTickerRaw` が bitFlyer のレスポンス仕様と整合している。
3. Raw → Ticker マッピングが本書 6 章の規則に従っている。
4. HTTP / 例外ハンドリングが本書 7 章の規則に従っている。
5. A010（OVR）/ A020（REQ）/ A030（ARC）と矛盾がない。

---

## 11. 改訂履歴

| 版     | 日付         | 内容                                                                                        |
| ----- | ---------- | ----------------------------------------------------------------------------------------- |
| 2.0.0 | 2025-11-XX | Stage1 実装構造と新設計方針に合わせて全面改訂。IExchangeClient / bitFlyer Adapter / Infrastructure 間の具体仕様を定義。 |
