# Normalized API 命名規則（共通）

## 目的

本プロジェクトにおける **Normalized API** は、各取引所の公式 API 仕様を
**意味変換・統合を行わずにそのまま写像した正本（single source of truth）** とする。

そのため、Normalized API の命名規則は、

* 人間の解釈による揺れを排し
* 取引所ごとの差異を尊重し
* 将来の取引所追加時にも一貫して適用できる

ことを目的として定める。

本書は、**全取引所共通で守るべき最小ルールのみ**を定義し、
取引所固有の癖や例外は各取引所別文書に委ねる。

---

## 適用範囲

* 対象：`ExchangeApi.Exchanges.<Ex>.Normalized` 配下の公開 API
* 非対象：Raw / Wire / Adapter 内部 API、Facade 抽象 API

---

## 基本原則

### 原則1：1 endpoint = 1 method

* 公式 API の **1 HTTP endpoint は、必ず 1 つの Normalized メソッドとして定義する**。
* 複数 endpoint をまとめた共通メソッドを作ってはならない。
* 利便性のための統合・省略・意味寄せは禁止する。

---

### 原則2：命名は公式 API パス由来とする

* Normalized API のメソッド名は、**公式 API の HTTP パスに基づいて決定する**。
* パスに含まれる情報を、共通化・抽象化の都合で削除してはならない。

具体的なパス → メソッド名の変換規則は、各取引所別文書で定義する。

---

### 原則3：メソッド名の構成

Normalized API のメソッド名は、以下の構成をとる。

```
<Method> + <PathDerivedName> + CallAsync
```

* `<Method>` : 操作を表す動詞（後述）
* `<PathDerivedName>` : 公式 API パス由来の文字列（取引所別規則に従う）
* `CallAsync` : 非同期 Call API を示す固定サフィックス

---

### 原則4：動詞（Method prefix）セット

使用可能な動詞は、以下に限定する。

* `Get`
* `Post`
* `Send`
* `Cancel`
* `Delete`

補足：

* HTTP メソッドと必ずしも 1:1 である必要はない。
* POST であっても意味が「キャンセル」の場合は `Cancel` を用いてよい。
* `Fetch` / `Load` / `List` / `Query` 等の動詞は使用禁止とする。

---

### 原則5：CallAsync 固定

* Normalized API は **Call-only** とする。
* すべての公開メソッドは `CallAsync` で終わらなければならない。
* 同期版 API、`Async` を省略した命名は禁止する。

---

### 原則6：Path Parameter の表記

* 公式 API パス中の path parameter（例：`{order-id}`）は、

  * 中括弧を除去し
  * 記号区切りを分解し
  * PascalCase 化する

例：

```
{order-id} → OrderId
{withdraw_id} → WithdrawId
```

---

## 禁止事項

* 取引所間で命名を揃える目的での省略・改名
* 意味の異なる endpoint を同一メソッド名にまとめること
* Facade / 抽象 API と同一の命名を Normalized に流用すること

---

## 取引所別文書との関係

* 各取引所は、本書の原則を満たす範囲で **独自のパス変換規則**を定義する。
* 取引所別文書には、以下を必ず含める。

  1. 当該取引所の命名変換ルール
  2. 公式 API endpoint 一覧と対応する Normalized メソッド名

共通ルールと取引所別ルールが矛盾する場合、
**取引所別ルールを優先する**（ただし原則違反は不可）。

---

## 本書の位置づけ

本書は、

* 命名の揺れを防ぐための **拘束力のある設計ルール**であり
* 利便性や共通化を目的としたガイドラインではない。

利便性・横断利用・意味統一は、Facade 層で扱う。
