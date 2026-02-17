# Resilience Contracts（429 / Timeout / Partial Failure）

本書は、ExchangeAPI におけるレジリエンス契約の正本（Normative）である。  
公開境界（Contracts/Facade）での観測可能な挙動を固定し、実装詳細ではなく契約として定義する。  
本書の規定（MUST / MUST NOT）は実装都合より優先される。

---

## 1. 適用範囲

- 本書は `src/Transport/` の送信ポリシーと、`src/Contracts/` の公開結果型に適用される（MUST）。
- 単発 endpoint（単一リモート呼び出し）では、失敗は失敗として返却し、部分成功を返してはならない（MUST NOT）。
- 複数呼び出しを集約する API では、部分成功を公式結果型で表現しなければならない（MUST）。

---

## 2. エラー分類（Taxonomy）

### 2.1 Caller Error（入力不正）

- 呼び出し側入力の不正は `ArgumentException` 系で即時失敗とする（MUST）。
- 入力不正をリトライ対象にしてはならない（MUST NOT）。

### 2.2 Transient（一時障害）

- 次を Transient とみなす（MUST）。
  - `429`（Rate Limit）
  - `5xx`
  - `408` / `504`
  - 通信断（`HttpRequestException`）
  - Client-side timeout
- Transient は自動リトライ対象になり得る（MAY）。

### 2.3 Permanent（恒久障害）

- 次を Permanent とみなす（MUST）。
  - `4xx`（`429` 除く）
  - 認証失敗
  - 契約違反（意味解釈不能・必須データ欠落など）
- Permanent を自動リトライしてはならない（MUST NOT）。

### 2.4 Canceled（キャンセル）

- 呼び出し元 `CancellationToken` による中断は Canceled と分類する（MUST）。
- Canceled を自動リトライしてはならない（MUST NOT）。

---

## 3. 429（Rate Limit）規約

- `429` 受信時は `Retry-After` を最優先する（MUST）。
- `Retry-After` が無い、または解釈不能な場合は指数バックオフを使う（MUST）。
- 指数バックオフにはジッターを加える（MUST）。
- リトライ終了条件は「最大試行回数」か「総リトライ時間上限」の早い方とする（MUST）。
- `429` の扱いは呼び出し単位で次のどちらかを明示する（MUST）。
  - 単発 endpoint: 通常失敗として返す
  - 集約 endpoint: 部分失敗（`BatchResult`）に格納する

---

## 4. Timeout 規約

- Timeout は次の 2 種を区別する（MUST）。
  - Client-side timeout（クライアント期限超過）
  - Server-side timeout（HTTP `408` / `504`）
- どちらも原則 Transient とし、リトライ可否は冪等性とポリシー設定で判定する（MUST）。
- 呼び出し元キャンセルと client timeout は同一視してはならない（MUST NOT）。

---

## 5. リトライ可否ルール

- 自動リトライ対象（MAY）
  - `429`, `5xx`, `408`, `504`
  - `HttpRequestException`
  - client timeout
- 自動リトライ非対象（MUST NOT）
  - 呼び出し元キャンセル
  - `4xx`（`429` 除く）
  - 契約違反 / マッピング失敗 / 入力不正

---

## 6. Partial Failure（公式パターン）

複数呼び出しの集約結果は、Contracts の結果型で次を表現できなければならない（MUST）。

- 成功集合
- 失敗集合
- 全成功 / 全失敗 / 部分成功の判定

結果型は次の語彙を含む（MUST）。

- `BatchResult<TItem>`
- `BatchError`

`BatchError` は少なくとも次を持つ（MUST）。

- `EndpointId`
- `ErrorKind`
- `Message`

`BatchError` に取引所識別情報（例: `ExchangeCode` / `ExchangeId` / `ExchangeName`）を含めてはならない（MUST NOT）。
呼び出し文脈で取引所識別が必要な場合は、Composition / Application 側で付与・管理する（MUST）。

ドメイン DTO にエラー情報を混在させてはならない（MUST NOT）。
エラーはトランザクション結果オブジェクトに保持する（MUST）。

---

## 7. 観測規約（呼び出し側）

呼び出し側は次で状態判定を行う（MUST）。

- `HasErrors`
- `HasSuccesses`
- `IsSuccessOnly`
- `IsFailureOnly`
- `IsPartialSuccess`

呼び出し側が HTTP の生文字列や exchange 固有エラーコードに依存して判定してはならない（MUST NOT）。
