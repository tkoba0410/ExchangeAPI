# Stage 1 仕様書（たたき台）

## 1. 目的
Exchange API Library の Stage 1 における最小実装範囲を定義する。

## 2. 対象範囲
- bitFlyer の Public REST のみを対象とする。
- Supported endpoints:
  - Ticker Price

## 3. Abstractions（抽象層）

### 3.1 呼称・命名方針（最初から一般化）
Stage 1 から **Abstractions 層は取引所非依存の一般的な呼称** を採用する。各取引所固有の呼称や形式は **Adapters 層に閉じ込める**。

- 公開インターフェースでは、`symbol`（例: `"BTC/JPY"`）などの一般的な概念名を用いる。
- bitFlyer 固有の `product_code`（例: `"BTC_JPY"`）は Adapters 層でのみ扱う。
- DTO のプロパティ名も一般的な名称とし、取引所固有のフィールド名（`ltp`, `best_bid` など）は Adapters 側でマッピングする。

### 3.2 IExchangeClient（インターフェース案）

```csharp
public interface IExchangeClient
{
    /// <summary>
    /// 指定したシンボルの現在のティッカー情報を取得する。
    /// シンボルは "BTC/JPY" のような "BASE/QUOTE" 形式の大文字表記とする。
    /// </summary>
    /// <param name="symbol">例: "BTC/JPY"。</param>
    /// <param name="cancellationToken">キャンセル要求。</param>
    /// <returns>ティッカー情報。</returns>
    Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
```

- Stage 1 ではエラーは例外ベースとし、ネットワークエラーや無効なシンボルは例外として通知する（詳細は今後のエラー設計で拡張）。

### 3.3 DTOs（データ構造案）

以下は bitFlyer の Ticker API (`GET /v1/getticker`) のレスポンス構造を基に、
一般化された `Ticker` DTO にマッピングする方針である。

#### bitFlyer Ticker レスポンス（元データ）
```json
{
  "product_code": "BTC_JPY",
  "state": "RUNNING",
  "timestamp": "2015-07-08T02:50:59.97",
  "tick_id": 3579,
  "best_bid": 30000,
  "best_ask": 36640,
  "best_bid_size": 0.1,
  "best_ask_size": 5,
  "total_bid_depth": 15.13,
  "total_ask_depth": 20,
  "market_bid_size": 0,
  "market_ask_size": 0,
  "ltp": 31690,
  "volume": 16819.26,
  "volume_by_product": 6819.26
}
```

#### マッピング方針
- **product_code → Symbol**（内部では `"BTC/JPY"` に変換して格納する）
- **best_bid / best_ask → BestBid / BestAsk**
- **ltp → LastTradedPrice**
- **timestamp → TimestampUtc（UTC に変換）**
- **state / tick_id / volume 系**は Stage 1 では DTO に含めない（将来拡張）。

#### Ticker DTO（Stage 1）
```csharp
public sealed class Ticker
{
    public string Symbol { get; }
    public decimal BestBid { get; }
    public decimal BestAsk { get; }
    public decimal LastTradedPrice { get; }
    public DateTime TimestampUtc { get; }

    public Ticker(
        string symbol,
        decimal bestBid,
        decimal bestAsk,
        decimal lastTradedPrice,
        DateTime timestampUtc)
    {
        Symbol = symbol;
        BestBid = bestBid;
        BestAsk = bestAsk;
        LastTradedPrice = lastTradedPrice;
        TimestampUtc = timestampUtc;
    }
}
```


```csharp
public sealed class Ticker
{
    public string Symbol { get; }
    public decimal BestBid { get; }
    public decimal BestAsk { get; }
    public decimal LastTradedPrice { get; }
    public DateTime TimestampUtc { get; }

    public Ticker(
        string symbol,
        decimal bestBid,
        decimal bestAsk,
        decimal lastTradedPrice,
        DateTime timestampUtc)
    {
        Symbol = symbol;
        BestBid = bestBid;
        BestAsk = bestAsk;
        LastTradedPrice = lastTradedPrice;
        TimestampUtc = timestampUtc;
    }
}
```

- 時刻は UTC に正規化し `TimestampUtc` として扱う。
- 数値は金額・価格を扱うため `decimal` を用いる。

### 3.4 シンボル定数（使い勝手のための定義）

（シンボルに関する補足的な仕様は 3.6 節を参照のこと。）

```csharp
public static class Symbols
{
    /// <summary>
    /// BTC/JPY（spot）。内部的には bitFlyer の "BTC_JPY" にマッピングされる。
    /// </summary>
    public const string BtcJpy = "BTC/JPY";
}
```

- 利用者は `Symbols.BtcJpy` を用いてシンボルを指定できる。
- bitFlyer Adapter は `"BTC/JPY" → "BTC_JPY"` のマッピングを内部で行う。

### 3.5 非機能要件（Abstractions / IExchangeClient）

- `IExchangeClient` の実装は **スレッドセーフであることを推奨**する。
  - 同一インスタンスを複数スレッドから同時に呼び出しても問題がない設計とする。
  - 典型的な利用形態として DI コンテナにシングルトン登録されることを想定する。
- `IExchangeClient` の生成コストは軽量であることが望ましいが、基本方針として「シングルトンで長く使う」ことを前提とする。

### 3.6 symbol の形式・正規化ルール

- `symbol` は `"BASE/QUOTE"` 形式の **大文字・スラッシュ区切り** とする（例: `"BTC/JPY"`）。
- 入力文字列の前後に存在する空白文字はトリムして扱う（`" BTC/JPY "` → `"BTC/JPY"`）。
- 形式が規定と異なる場合（例: `"BTCJPY"`, `"btc/jpy"` など）は `ArgumentException` をスローする。

### 3.7 例外ポリシー（Stage 1）

Stage 1 における `IExchangeClient` の例外ポリシーを以下のように定める。

- **ネットワーク関連エラー**（タイムアウト、DNS 解決不可、接続拒否など）
  - .NET の標準例外（`HttpRequestException` など）をそのままスローしてよい。
  - 将来の Stage で `ExchangeNetworkException` などにラップすることを許容する。

- **HTTP ステータス異常**（4xx/5xx）
  - Stage 1 では `HttpRequestException` 等で通知してよい。
  - エラー内容のログ出力・メッセージ整形は Adapter／Transport 層の責務とする。

- **API レベルのエラー応答**（エラーコードやメッセージを含む JSON）
  - Stage 1 では、パース可能な範囲でメッセージに含めた汎用例外（`ExchangeApiException`）にまとめてもよい。
  - ただし例外型の詳細設計は Stage 2 以降で拡張可能とし、互換性を前提とする。

- **無効な引数**（サポートされない `symbol` 形式など）
  - `ArgumentException` / `ArgumentNullException` を用いる。
  - 利用者が防げる入力エラーと、外部要因によるエラーを区別することを推奨する。

- **予期しない内部エラー**（バグ、パース不能なレスポンスなど）
  - 一般的な `Exception` または `ExchangeApiException` にラップしてスローする。
  - 内部実装の詳細を漏らさず、デバッグ用情報はログで補完する方針とする。

> Stage 1 では「例外ベースでシンプルに扱う」ことを優先し、
> Result 型による成功/失敗のモデリングは Stage 2 以降の拡張対象とする。

### 3.8 ExchangeApiException の仕様（Stage 1）

`ExchangeApiException` は「Exchange API レベルでの失敗」を表現するための基本例外クラスとする。
Stage 1 では最低限、次の要素を持つ。

```csharp
public class ExchangeApiException : Exception
{
    /// <summary>
    /// 対象となった取引所識別子（例: "bitflyer"）。
    /// </summary>
    public string Exchange { get; }

    /// <summary>
    /// シンボル（例: "BTC/JPY"）。指定されていない場合は null。
    /// </summary>
    public string? Symbol { get; }

    /// <summary>
    /// HTTP ステータスコード（REST の場合）。該当しない場合は null。
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// 外部 API から返却された生のエラーコード／メッセージ（パースできた範囲）。
    /// </summary>
    public string? RemoteError { get; }

    public ExchangeApiException(
        string message,
        string exchange,
        string? symbol = null,
        int? statusCode = null,
        string? remoteError = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Exchange = exchange;
        Symbol = symbol;
        StatusCode = statusCode;
        RemoteError = remoteError;
    }
}
```

- Stage 1 では 1 つの例外型に集約し、詳細なサブクラス（`InvalidSymbolException` など）は導入しない。
- `Exchange` には一意な識別子（`"bitflyer"` 等）を設定する。
- `RemoteError` には、可能であれば取引所側のエラーコードやメッセージを格納する。

### 3.9 ログ／メトリクスとの関係（方針）

Stage 1 におけるログ・メトリクスの方針は以下の通りとする。

- **Abstractions 層**
  - ログ出力やメトリクス送信の責務は持たない。
  - 例外／戻り値を通じて「観測に必要な情報（message, Exchange, Symbol 等）」を提供することに専念する。

- **Adapters 層 / Transport 層**
  - 通信エラーや API エラーが発生した場合は、適切なログレベル（Warning / Error）で記録することを推奨する。
  - ログには少なくとも以下を含める：
    - Exchange（例: bitflyer）
    - Symbol（可能であれば）
    - HTTP メソッド／エンドポイント
    - ステータスコード
    - RemoteError（API 応答メッセージ）

- **メトリクス**
  - Stage 1 ではメトリクスの仕組み自体は必須としない（実装依存）。
  - 将来の Stage では、リクエスト数／エラー数／レイテンシなどをメトリクスとして収集できるよう、
    Transport 層に計測フックを設けることを想定する（Timer, Counter 等）。

- **責務分離の原則**
  - Abstractions は「何が起きたかを表現する」役割、
  - Adapters / Transport は「それをどのように記録・監視するか」を担う役割とし、
    ログ・メトリクスの具体実装（ILogger, OpenTelemetry 等）には依存しない。

---



### IExchangeClient（インターフェース）
- `GetTickerAsync(string symbol, CancellationToken ct)`：指定したシンボルの現在価格を取得する。

### DTOs（データ構造）
- Ticker: Symbol, Price.

## 4. 対象外（Stage 1 では扱わないもの）
- 認証付き REST
- WebSocket
- Rate limiting / CircuitBreaker 等の高度な制御
- 複数取引所対応

## 5. Adapters 層の方針（bitFlyer）

Stage 1 において、bitFlyer 向け Adapters 層は **API レスポンスの全情報を「拾う」ことを原則とする。**

### 5.1 Raw モデル（取引所固有モデル）

- bitFlyer の `GET /v1/getticker` については、レスポンス JSON を余すことなく保持する Raw モデルを定義する：

```csharp
public sealed class BitflyerTickerRaw
{
    public string ProductCode { get; set; } = default!;
    public string State { get; set; } = default!;
    public DateTime Timestamp { get; set; };
    public long TickId { get; set; };
    public decimal BestBid { get; set; };
    public decimal BestAsk { get; set; };
    public decimal BestBidSize { get; set; };
    public decimal BestAskSize { get; set; };
    public decimal TotalBidDepth { get; set; };
    public decimal TotalAskDepth { get; set; };
    public decimal MarketBidSize { get; set; };
    public decimal MarketAskSize { get; set; };
    public decimal LastTradedPrice { get; set; };
    public decimal Volume { get; set; };
    public decimal VolumeByProduct { get; set; };
}
```

- プロパティ名・型は bitFlyer の API 仕様にできる限り忠実に対応させる。
- Stage 1 では、この Raw モデルは Adapters 層の内部でのみ使用し、Abstractions 層には公開しない。

### 5.2 Raw → 一般化 DTO へのマッピング

- `BitflyerTickerRaw` から一般化された `Ticker` DTO へとマッピングする：
  - `ProductCode ("BTC_JPY")` → `Symbol ("BTC/JPY")`
  - `BestBid` / `BestAsk` → `Ticker.BestBid` / `Ticker.BestAsk`
  - `LastTradedPrice` → `Ticker.LastTradedPrice`
  - `Timestamp` → `Ticker.TimestampUtc`（UTC へ正規化）
- Volume や Size 系の値は Stage 1 では `Ticker` には載せないが、Raw モデル側で保持しておくことで、Stage 2 以降に DTO を拡張しやすくする。

### 5.3 「すべてを拾う」方針の位置づけ

- Abstractions 層：
  - ライブラリ利用者にとって汎用的・安定的な最小限の情報のみ公開する。
- Adapters 層：
  - 取引所固有の情報を **できる限り欠損なく持つ**（Raw モデル）ことで、将来の機能拡張や詳細なモニタリングに対応可能にする。
  - Raw モデルは Adapter 層の内部詳細であり、Abstractions には直接露出しない。

> これにより、「使い勝手の良い一般化された Abstractions」と
> 「取引所固有情報を失わず保持する Adapters」の両立を図る。

### 5.4 Adapter Raw API の公開方針

上級利用者向けに、Adapter 層の生データ（Raw モデル）も利用可能とする。ただし、
Abstractions 層の抽象化を崩さないよう、以下の方針で公開する。

- Raw API は **取引所別の専用インターフェース** として定義する。
- Abstractions のインターフェースには Raw 型を一切登場させない。

#### IBitflyerPublicApi（例）

```csharp
public interface IBitflyerPublicApi
{
    /// <summary>
    /// bitFlyer の Public Ticker API (GET /v1/getticker) をそのまま呼び出し、
    /// 生のレスポンスを表現する Raw モデルとして返す。
    /// </summary>
    /// <param name="productCode">bitFlyer の product_code（例: "BTC_JPY"）。</param>
    /// <param name="cancellationToken">キャンセル要求。</param>
    /// <returns>bitFlyer Ticker の生データ。</returns>
    Task<BitflyerTickerRaw> GetTickerRawAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
```

#### IExchangeClient との関係

```csharp
public class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerPublicApi _rawApi;

    public BitflyerExchangeClient(IBitflyerPublicApi rawApi)
    {
        _rawApi = rawApi;
    }

    public async Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        var productCode = ConvertSymbolToProductCode(symbol); // "BTC/JPY" → "BTC_JPY"
        var raw = await _rawApi.GetTickerRawAsync(productCode, cancellationToken);
        return MapToTicker(symbol, raw);
    }
}
```

- `IExchangeClient` はあくまで一般化された DTO (`Ticker`) のみを返す。
- Raw モデルを扱いたい場合は、利用者が `IBitflyerPublicApi` を直接利用することで、
  取引所固有の詳細情報を取得できるようにする。

### 5.5 メソッド名への API 呼称の取り込み方針

- メソッド名には、取引所 API のエンドポイント名の「意味的な部分」を取り込む。
  - 例: `GET /v1/getticker` → `GetTickerRawAsync`
  - 例: `GET /v1/getboard` → `GetBoardRawAsync`
- ただし、以下はメソッド名には含めない：
  - HTTP メソッド（GET/POST など）
  - バージョン番号（`/v1/` など）
  - スラッシュ区切りのパス構造（`/v1/○○/△△` の階層そのもの）
- C# の命名規則に従い、メソッド名は PascalCase + `Async` サフィックスとする。
- API 名に含まれる語は、可能な範囲で C# スタイルに変換して利用する：
  - `getticker` → `GetTicker`
  - `getboard` → `GetBoard`

> これにより、取引所 API の呼称との対応関係を保ちつつ、
> C#/.NET として自然な命名規則を維持する。

---

これにより、Abstractions を汚さずに Adapter 層の生データを利用可能とする設計とする。

### 5.6 Raw モデルの互換性ポリシー（Versioning）

- `BitflyerTickerRaw` を含む Raw モデルは、bitFlyer API 仕様のスナップショットとして扱う。
- 後方互換性の考え方：
  - プロパティの **追加** は後方互換とみなす（既存利用コードはコンパイル・実行ともに継続可能）。
  - プロパティ名の変更・削除、型の変更は **互換性破壊変更（Breaking Change）** とみなす。
- Raw モデルの Breaking Change を行う場合は、メジャーバージョンアップや別クラス名（例: `BitflyerTickerRawV2`）で提供することを検討する。

> 「取引所固有情報を失わず保持する Adapters」の両立を図る。

## 6. 実装方針

- Authenticated REST
- WebSocket
- Rate limiting
- Multi-exchange

## 5. 実装方針
- Simple Adapter for bitFlyer.
- Direct HTTP inside adapter.
- Extract common parts later.

## 6. テスト方針
- Basic console test.
- Error cases minimal.


## 7. ファイル構成・開発環境（案）
Stage 1 における最小構成は、Abstractions と bitFlyer Adapter を **2 つのプロジェクト** に分ける。

```
project-root/
  src/
    ExchangeApi.Abstractions/
      ExchangeApi.Abstractions.csproj
      IExchangeClient.cs
      Models/
        Ticker.cs
      Symbols.cs

    ExchangeApi.Bitflyer/
      ExchangeApi.Bitflyer.csproj
      IBitflyerPublicApi.cs
      Models/
        BitflyerTickerRaw.cs
      BitflyerExchangeClient.cs

  tests/
    ExchangeApi.Abstractions.Tests/
      ExchangeApi.Abstractions.Tests.csproj
    ExchangeApi.Bitflyer.Tests/
      ExchangeApi.Bitflyer.Tests.csproj

  docs/
    0210-REQ-STG1-AbstractionsMVP.md
    0120-OVR-INTR-ProjectOverview.md

  README.md
```

## 7.1 開発環境・ターゲットフレームワーク

- .NET ランタイム／SDK: **.NET 10 (LTS)** を前提とする。
- C# 言語バージョン: C# 14（`LangVersion` は明示指定せず SDK 既定に従う）。
- プロジェクトの `TargetFramework` は `net10.0` を用いる。

- IDE 例：
  - Visual Studio 2025 以降
  - Rider 最新版
  - Visual Studio Code + C# 拡張

project-root/
  src/
    Abstractions/
      IExchangeClient.cs
      Models/
        Ticker.cs
    Adapters/
      bitFlyer/
        bitFlyerPublicApiClient.cs
  tests/
    AbstractionsTests/
    AdapterTests/
  docs/
    0210-REQ-STG1-AbstractionsMVP.md
    0120-OVR-INTR-ProjectOverview.md
  README.md
```



## 8. 参考資料（Reference）
Stage 1 の仕様策定および実装にあたり参照する外部資料を以下にまとめる。

### 8.1 bitFlyer API 公式ドキュメント
- REST API（Public）
  - Ticker: https://api.bitflyer.com/v1/getticker
  - Board: https://api.bitflyer.com/v1/getboard
- API 全体仕様
  - https://lightning.bitflyer.com/docs?lang=ja

### 8.2 C# / .NET 関連
- HttpClient（公式ドキュメント）
- Task / async-await パターン
- System.Text.Json など JSON シリアライゼーション

### 8.3 プロジェクト内文書
- `docs/0120-OVR-INTR-ProjectOverview.md`（正典）
- `docs/0800-STD-DOC0-DocumentPolicy.md`

### 8.4 設計に影響する一般的な規格・ガイドライン
- RFC2119（MUST/SHOULD/MAY）
- REST API ベストプラクティス
- エラーハンドリング指針

## 9. Stage 1 完了条件（Definition of Done）

Stage 1 が「完了」とみなせる条件を以下に定義する。

### 9.1 機能面の完了条件

- Abstractions 層：
  - `IExchangeClient` が定義されていること。
    - 少なくとも `Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);` を含む。
  - `Ticker` DTO が本仕様書の定義どおり実装されていること。
  - `Symbols.BtcJpy` が定義されていること。

- bitFlyer Adapter 層：
  - `BitflyerTickerRaw` が bitFlyer の `GET /v1/getticker` のレスポンスを余さず表現していること。
  - `IBitflyerPublicApi.GetTickerRawAsync(string productCode, CancellationToken)` が実装され、実際に bitFlyer Public API からデータを取得できること。
  - `BitflyerExchangeClient` が `IExchangeClient` を実装し、
    - `symbol`（例: "BTC/JPY"）を `product_code`（例: "BTC_JPY"）へ変換すること。
    - `BitflyerTickerRaw` から `Ticker` へのマッピングを行うこと。

### 9.2 例外・エラー処理の完了条件

- 本仕様書で定義した例外ポリシーが実装されていること：
  - ネットワークエラーは .NET 標準例外（`HttpRequestException` 等）で通知されること。
  - API レベルのエラーについて `ExchangeApiException` が利用されていること（必要な箇所）。
  - 無効な `symbol` に対して `ArgumentException` 等が用いられていること。

### 9.3 ログ・メトリクス方針の反映

- Abstractions 層がログ・メトリクスに直接依存していないこと。
- Adapter 実装内で、少なくともエラー発生時にログ出力用のフック（`ILogger` 等）を導入可能な構造になっていること。
  - 実際のロガー実装はアプリケーション側に委ねる（インターフェースや DI で差し込める形であればよい）。

### 9.4 テスト・動作確認

- `ExchangeApi.Bitflyer.Tests` プロジェクト内に、以下を満たすテストが存在すること：
  - `GetTickerAsync(Symbols.BtcJpy)` を呼び出し、正常に `Ticker` が取得できること（統合テスト、または HTTP モックを用いたテスト）。
  - 無効な `symbol` を指定した場合に、適切な例外が発生すること。
- 最低限、ローカル環境で bitFlyer API に対して 1 回は実通信テストを行い、動作確認が記録されていること（メモや README でもよい）。

### 9.5 ドキュメント

- 本 Stage 1 仕様書（本ドキュメント）がリポジトリの `docs/` 配下に配置されていること。
- README に、Stage 1 で提供される機能概要と簡単な使用例（`GetTickerAsync("BTC/JPY")` の実行例）が記載されていること。

### 9.6 使用例（想定される基本的な利用コード）

以下のようなコードで `Ticker` を取得し、価格を表示できることを目安とする。

```csharp
var rawApi = /* IBitflyerPublicApi 実装の生成 */;
var client = new BitflyerExchangeClient(rawApi);

var ticker = await client.GetTickerAsync(Symbols.BtcJpy, cancellationToken);

Console.WriteLine($"{ticker.Symbol}: {ticker.LastTradedPrice}");
```

- 上記のコード断片が、ライブラリ利用者にとって自然であり、かつ本仕様書の設計方針と整合していること。

