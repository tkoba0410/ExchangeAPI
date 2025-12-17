# Composition / Factory

> このドキュメントは **5分で理解できる** ことを目的に、
> Composition が提供する Factory API の使い方と設計意図を説明します。

---

## 1. これは何か

`Composition.Factory` は、ExchangeAPI における **標準配線の入口**です。

* HTTP / Policy / Observability の既定構成
* 資格情報（Credentials）の注入
* ExchangeInfo の注入

をまとめて行い、

* **Raw クライアント（既定）**
* **Adapter クライアント（明示指定時のみ）**

を生成します。

---

## 2. 基本方針（最重要）

* **既定は Raw**：取引所の生 API（SDK 相当）を主役とする
* **Adapter は明示**：共通語彙で扱いたい場合のみ選択する
* **束ねない**：複数取引所をまとめる API は提供しない

---

## 3. クイックスタート（Raw 既定）

### Bitflyer（公開 API のみ）

```csharp
using Composition.Factory;

var bitflyerRaw = BitflyerFactory.CreateRaw();

// 例：公開 API を直接呼び出す
var ticker = await bitflyerRaw.GetTickerAsync("BTC_JPY");
```

* 資格情報は不要
* signer は内部で省略されます

---

### Bittrade（公開 API のみ）

```csharp
using Composition.Factory;

var bittradeRaw = BittradeFactory.CreateRaw();

var markets = await bittradeRaw.GetMarketsAsync();
```

* Bittrade も公開 API は資格情報不要

---

## 4. Adapter を使う場合（明示）

Adapter は **Common.DTO / Common.Interface** を返す薄いラッパです。

### Bitflyer Adapter

```csharp
using Composition.Factory;

var api = BitflyerFactory.CreateAdapter();

// Common.DTO を返す
var balances = await api.GetBalancesAsync();
```

* 資格情報を指定しない場合、公開系のみ動作

---

### Bittrade Adapter（注意点あり）

```csharp
using Composition.Factory;
using Common.Dtos;

var api = BittradeFactory.CreateAdapter(
    new BittradeFactoryOptions {
        AccountId = "your-account-id",
        Credentials = new ApiCredentials("accessKey", "secretKey")
    });

var balances = await api.GetBalancesAsync();
```

#### 例外条件

* `AccountId` が未指定の場合：

  * `InvalidOperationException`
* 資格情報が未指定で、認証が必要な API を呼ぶ場合：

  * `InvalidOperationException`

これは Bittrade の仕様差を **明示的に表現**するための設計です。

---

## 5. オプション指定（必要なときだけ）

```csharp
var raw = BitflyerFactory.CreateRaw(
    new BitflyerFactoryOptions {
        Credentials = new ApiCredentials("key", "secret"),
        PolicyOptions = new HttpPolicyOptions {
            Timeout = TimeSpan.FromSeconds(5)
        }
    });
```

* 指定しない場合は安全な既定値が使われます

---

## 6. 何を **しないか**（重要）

Composition / Factory は、以下を **意図的に提供しません**。

* 複数取引所を束ねるクライアント
* `Unified` / `MultiExchange` / `Registry` 的な API
* クロス取引（複数取引所をまたぐ取引）
* 戦略・ワークフロー・アービトラージロジック

これらは **利用者（アプリケーション）側の責務**です。

---

## 7. 設計上の理由（短く）

* 取引所ごとの差分は大きく、過剰な抽象化は嘘になりやすい
* raw SDK を主役にし、必要な部分だけ共通化する方が長期的に安全
* 束ね機能は、用途や失敗時の扱いがアプリ依存になる

---

## 8. どれを使うべきか？

* **raw を使うべき場合**

  * 取引所固有機能を使いたい
  * 完全な制御が必要

* **Adapter を使うべき場合**

  * 複数取引所で共通の処理を書きたい
  * DTO / Interface を揃えたい

迷ったら **Raw** を選んでください。

---

## 9. 関連ドキュメント

* `ARCHITECTURE.md`（設計憲章）
* `docs/Common/`（共通語彙）
* `docs/Exchanges/`（取引所ごとの詳細）
