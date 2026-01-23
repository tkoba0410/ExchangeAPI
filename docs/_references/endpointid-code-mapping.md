# EndpointId Code Mapping（Reference）

## 1. 目的

本書は、**EndpointId をコードへ反映する際の実装上の対応関係**を整理するための補助資料である。

* 層構造・責務・公開範囲・Call 抽象などの設計規範は **TopSpec（docs/topspec.md）** を正本とする。
* 本書は規範（Normative）ではなく、**実装指針（Reference）**に位置付けられる。

---

## 2. EndpointId とコードの関係

EndpointId は、取引所ごとの API Endpoint を一意に識別する識別子である。
コード上では、EndpointId を以下の目的で利用する。

* API メソッド名の基底
* Call メタデータへの埋め込み
* inventory（一覧）との対応付け

EndpointId 自体は、Request / Response の構造や振る舞いを規定しない。

---

## 3. 各層 API と EndpointId

### 3.1 共通原則

* 各層（Wire / Raw / Normalized / Contract）は API 呼び出し点を持つ。
* 各層の API メソッドは、**同一の EndpointId に対応**する。
* 上位層は直下層を呼び、変換のみを行う（I/O は Wire のみ）。

※ 層責務の正本定義は TopSpec を参照すること。

---

### 3.2 API メソッド名

API メソッド名は、EndpointId を基底として派生させる。

例：

* EndpointId: `GetTicker`

  * Wire: `GetTickerCallAsync`
  * Raw: `GetTickerCallAsync`
  * Normalized: `GetTickerCallAsync`
  * Contract: `GetTickerCallAsync`

※ Method（GET/POST 等）を含めるか否か、単語境界の粒度などは
取引所ごとの補助資料に委ねる。

---

## 4. Call と EndpointId

### 4.1 Call メタデータ

各層の API 呼び出しは `Call<TRequest, TResponse>` を返す。

Call には、対応する EndpointId をメタデータとして保持してよい（MAY）。

* 例：

  * `CallMeta.EndpointId = EndpointId.GetTicker`

これにより、ログ・トレース・テスト時の識別が容易になる。

---

## 5. inventory との関係

* inventory（`docs/inventory/endpoints-<exchange>.md`）は、
  EndpointId と公式 API Endpoint（Method / Path / Scope）の対応関係を列挙する一覧である。
* inventory は規範ではなく、**対応関係の可視化**を目的とする。
* inventory に記載のない EndpointId は、本リポジトリでは未定義として扱う。

---

## 6. 禁止事項

本書は以下を規定しない。

* 層構造や責務の再定義
* 公開 API の範囲
* Request / Response の意味論
* Capability の提供可否

これらはすべて TopSpec および Contract 文書の責務である。

---

## 7. 位置付けの再確認

* 本書は **Reference** であり、Normative ではない。
* 設計判断に迷いが生じた場合は、必ず TopSpec を参照すること。
