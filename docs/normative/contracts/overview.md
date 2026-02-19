# Contracts Overview

## 公開安定面（重要）

本リポジトリにおける外部利用者向けの **公開安定 API** は **Contract 層のみ**である。

- **Contract 層**：外部公開（安定契約）。語彙・DTO・意味論（Shape / Semantics）は Contracts 文書群（例：`docs/normative/contracts/contracts.md`）で定義する。
- **Normalized / Raw / Wire 層**：内部実装（または高度利用向け）。外部互換性は保証しない。

実務上、取引所別の機能網羅が必要な利用者（Bot / 高度利用）は Normalized を利用できるが、
その場合は「公開安定契約外（追従前提）」として扱う。
Contracts は、取引所横断で共通に成立する最小機能に限定する。

## 利用モデル（運用指針）

- **Level A（主利用面）**: 取引所別機能網羅を優先する場合は `Normalized` を利用する。
- **Level B（横断補助面）**: 取引所横断で安定的に共通化できる最小機能のみ `Contracts` を利用する。

※ 安定保証の対象は Level B（Contracts）のみである。  
※ Level A（Normalized）は利用可能だが、互換保証外である。

## 1. Purpose

本書は Contract 層における **公開安定 API の契約文書**である。

本書（overview）は概要説明であり、拘束力（Normative）を持つ契約条文は `docs/normative/contracts/contracts.md` を正本とする。

設計規範（層構造、Call 抽象、公開範囲等）の正本は  
**TopSpec（docs/normative/topspec.md）** とする。

Contracts は取引所非依存の公開安定 API であり、
取引所固有の Raw / Normalized Request / Response / DTO を公開面に露出しない。
取引所スコープ、混線防止、例外の扱いは TopSpec の該当規定（例: 3.4.4）を正とする。

取引所実装間の「統一/例外」の運用方針は `docs/archive/references/exchange-parity-policy.md` を参照する。
Contracts API 署名の正本は `src/Contracts/Facade/Interfaces/*` とする。

本書の目的は次の 2 点に限定される。

* **取引所仕様に起因しない差異を排除すること**
* **利用者が取引所ごとの分岐を書かずに利用できる範囲を明確化すること**

実装方法（Canon）、背景説明は本書の対象外とする（Shape / Semantics の定義は contracts.md 等で行う）。

---

## 2. Scope

本書が規定するのは以下に限定される。

* 利用者が **常に同一の振る舞いを期待してよい領域**（Guaranteed）
* 取引所仕様により **差異が存在し得ることを利用者が許容する領域**（Allowed Variations）

本書は、取引所固有仕様の詳細、実装手法、最適化方針を規定しない。

### 2.1 Public / Private（署名有無）

Contracts の公開 API は Public / Private に分離する。
この Public/Private は **署名の有無**のみを表し、用途別（MarketData / Trading / Account 等）の分類ではない。
用途別の意味分類は **上位レイヤのラッパにのみ許容**し、Contracts には持ち込まない。

---

## 3. Guaranteed（差異禁止）

本節に記載された事項については、**取引所間で差異が存在してはならない**。
利用者は、本節に関して **取引所判別・分岐・例外処理を書く必要がない**。

### 3.1 取引所判別情報の非提供

* Contracts API の戻り値（`Call<TRequest, TResponse>`）および `TResponse`（ContractDTO）には、
  取引所識別情報（例：`ExchangeCode`）は含まれない。
* `BatchError` を含むエラー DTO / 結果 DTO にも、取引所識別情報は含めない。
* 利用者は「どの取引所か」を前提とした分岐を、Contracts API の戻り値から行ってはならない。
  - 分岐が必要な場合は、capability / 構築形態（Composition）によって **事前に** 分離・選択する。

### 3.2 API 呼び出し前提条件

* 同名の Contracts API は、取引所間で **呼び出し前提条件の差異を持たない**。
* 取引所固有の追加前提条件（例：accountId 等）が存在する場合、次のいずれかにより差異を解消しなければならない。

  * Composition により **自動的に前提条件を満たす**（利用者に追加入力を要求しない）
  * **別 API として明示的に分離する**（同名 API のまま成功可否が分かれる状態を禁止）

例:
Bittrade の Private API で accountId が必要な場合は、Composition で accountId を必須注入し、
Normalized/Adapter で `NotSupported` を返す通常制御を行わない。

---

### 3.3 戻り値・失敗表現

* Contracts API の戻り値は常に `Call<TRequest, TResponse>` である。
* 失敗は `Call` の失敗として表現され、例外は制御フローとして用いてはならない。
* 未対応 capability は **Facade の nullable capability により事前に判定可能**でなければならない。
* `NotSupported` を通常制御フロー（取引所判別・分岐）として利用することは禁止される（原則使用しない）。

#### 3.3.1 Async 命名

* Contracts の公開 I/F メソッド名は末尾を **`Async`** とする。
* 命名は **`Get` + `<ContractApiId>` + `Async`** を基本とする（例：`GetBalanceAsync`）。

---

### 3.4 意味論の統一

以下の概念の意味論は、取引所に依存せず常に共通である。

* Symbol / Market
* OrderKey
* Side / OrderType / TimeInForce
* Page / Limit / Cursor

取引所 API における表記差（例：`BTC_JPY` / `btcjpy` 等）は、利用者 API に露出してはならない。

---

## 4. Allowed Variations（差異許容）

本節に記載された事項については、取引所仕様により差異が存在し得る。
利用者は、**capability 等の明示的手段**を用いて分岐する。

### 4.1 機能の有無

* 取引所によって提供されない機能が存在し得る。
* 利用可否は Facade の capability（nullable）により **事前に判定可能でなければならない**。
* 利用可否判定を `NotSupported` の捕捉に依存してはならない。
* 取引所差で有無が揺れる機能は、単独 capability I/F（nullable）として分離してよい。

---

### 4.2 制約値・仕様差

以下は取引所仕様に依存する。

* 最小数量・数量精度
* 価格精度
* レート制限
* メンテナンス時間

---

## 5. Prohibitions

以下を明示的に禁止する。

* 取引所差異を理由とした API 呼び出し前の try/catch 分岐
* `NotSupported` を利用した通常制御フロー
* 取引所固有表記（文字列）を利用者 API に露出すること
* Contracts の DTO に含まれない取引所識別情報（例：`ExchangeCode`）を前提にした通常制御フロー

---

## 6. Non-Goals

本書は以下を目的としない。

* 実装方法の最適解提示
* 設計選択の理由説明
* 取引所仕様の網羅的記述
