# Contracts Overview

## 1. Purpose

本書は、本ライブラリにおける **利用者向け契約（User-facing Contract）** の正本である。

本書の目的は次の 2 点に限定される。

* **取引所仕様に起因しない差異を排除すること**
* **利用者が取引所ごとの分岐を書かずに利用できる範囲を明確化すること**

型定義（Shape / Semantics）、実装方法（Canon）、背景説明は本書の対象外とする。

---

## 2. Scope

本書が規定するのは以下に限定される。

* 利用者が **常に同一の振る舞いを期待してよい領域**（Guaranteed）
* 取引所仕様により **差異が存在し得ることを利用者が許容する領域**（Allowed Variations）

本書は、取引所固有仕様の詳細、実装手法、最適化方針を規定しない。

---

## 3. Guaranteed（差異禁止）

本節に記載された事項については、**取引所間で差異が存在してはならない**。
利用者は、本節に関して **取引所判別・分岐・例外処理を書く必要がない**。

### 3.X 取引所判別情報の非提供

* Contracts API の戻り値（`Call<TRequest, TResponse>`）および `TResponse`（ContractDTO）には、
  取引所識別情報（例：`ExchangeCode`）は含まれない。
* 利用者は「どの取引所か」を前提とした分岐を、Contracts API の戻り値から行ってはならない。
  - 分岐が必要な場合は、capability / 構築形態（Composition）によって **事前に** 分離・選択する。

### 3.1 API 呼び出し前提条件

* 同名の Contracts API は、取引所間で **呼び出し前提条件の差異を持たない**。
* 取引所固有の追加前提条件（例：accountId 等）が存在する場合、次のいずれかにより差異を解消しなければならない。

  * Composition により **自動的に前提条件を満たす**（利用者に追加入力を要求しない）
  * **別 API として明示的に分離する**（同名 API のまま成功可否が分かれる状態を禁止）

---

### 3.2 戻り値・失敗表現

* Contracts API の戻り値は常に `Call<TRequest, TResponse>` である。
* 失敗は `Call` の失敗として表現され、例外は制御フローとして用いてはならない。
* 未対応 capability は **Facade の nullable capability により事前に判定可能**でなければならない。
* `NotSupported` を通常制御フロー（取引所判別・分岐）として利用することは禁止される（原則使用しない）。

#### 3.2.1 CallAsync 命名（Call-only）

* Contracts の公開 I/F は Call-only で提供されるため、I/O を伴う公開メソッド名は末尾を **`CallAsync`** とする。
* `Async` のみ（例：`GetBalanceAsync`）は使用しない。

---

### 3.3 意味論の統一

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
