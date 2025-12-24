# Errors

`Common.Contracts.Errors` は、
**HTTP・Transport・Policy 層で発生するエラーを、上位レイヤへ安全に伝達するための共通契約**を定義する。

この層の目的は次の通り：

- 上位レイヤが **HTTP や通信例外を直接扱わなくてよい**ようにする
- Retry / CircuitBreaker / Factory が **共通の判断軸**を持てるようにする
- 取引所固有のエラー仕様を **ここに持ち込まない**

---

## ExchangeApiException

### 役割

`ExchangeApiException` は、
**Common 基盤における唯一の「外部公開エラー型」**である。

Transport / RestClient / Policy 層は、
例外をそのままスローしてはならず、原則として `ExchangeApiException` に正規化する。

---

### 保持する情報

`ExchangeApiException` は、最低限以下の情報を保持する。

| 項目 | 説明 |
|---|---|
| Message | 呼び出し元がログ・診断に利用できるメッセージ |
| StatusCode | HTTP ステータスコード（不明な場合は null） |
| ErrorCategory | エラーの意味分類（Retry 判断に使用） |
| InnerException | 元となった例外（HttpRequestException / JsonException 等） |

---

### 基本方針

- **HTTP の存在は隠蔽する**  
  上位レイヤは `HttpResponseMessage` を扱わない
- **Retry 判断は ErrorCategory に基づく**  
  StatusCode を直接見て判断しない
- **詳細情報は InnerException に残す**  
  利用者が必要なら辿れる

---

## ExchangeErrorCategory

`ExchangeErrorCategory` は、
**エラーの意味的分類**を表す enum である。

Retry / CircuitBreaker / Policy は、この値のみを見て判断を行う。

---

### 分類一覧と意味

| Category | 意味 | Retry |
|---|---|---|
| Request | リクエスト不正（パラメータ・形式） | ❌ しない |
| RateLimit | レート制限 | ⭕ する |
| Server | サーバー内部エラー | ⭕ する |
| Network | 通信障害・一時障害 | ⭕ する |
| Unknown | 上記に分類できないもの | ⭕（デフォルト） |

※ `Request` のみが **明示的に Retry 不可**である。

---

### 設計意図

- Retry の可否を **StatusCode ではなく意味で決める**
- 取引所ごとの HTTP 差異を吸収する
- 将来、取引所以外の API にも再利用可能

---

## エラー正規化ルール（概要）

Transport / RestClient 層は、以下の方針でエラーを正規化する。

### HTTP エラーレスポンス

- HTTP ステータスコードを `StatusCode` に設定
- 内容に関わらず `ExchangeApiException` を生成
- ステータスに応じて `ErrorCategory` を決定

### HttpRequestException

- `InnerException` に保持
- `StatusCode` があれば設定
- `ErrorCategory` は `Network` または `Server`

### JSON 解析失敗

- `InnerException` に `JsonException` を設定
- `ErrorCategory` は `Server` または `Unknown`

---

## Retry / Policy との関係

Retry / CircuitBreaker / Timeout の各 Policy は、

- `ExchangeApiException` の **ErrorCategory**
- および設定値（最大回数・遅延）

のみを用いて動作する。

Policy は以下を **前提としてよい**：

- Request カテゴリは Retry されない
- RateLimit / Network / Server は Retry 対象
- HTTP 実装差異は既に吸収されている

---

## この層が扱わないもの

以下は **この契約の責務外**である：

- 取引所固有のエラーコード
- Order / Balance / Trade 等のドメイン DTO
- API パスや JSON フォーマット
- Factory による取引所選択ロジック

それらは `Exchange.Common` 側で定義する。

---

## まとめ

- `ExchangeApiException` は **Common 基盤の唯一の公開エラー型**
- Retry 判断は **ErrorCategory に集約**
- HTTP / 通信の詳細は **下位層に閉じ込める**
- 取引所ドメインは **この層に侵入しない**

