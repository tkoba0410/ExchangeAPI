# ExchangeAPI Documentation

> ExchangeAPI は、複数の暗号資産取引所 API を扱うための **技術基盤＋取引所 SDK 集合**です。
> 本ドキュメントは **1分で全体像が分かり、次に読むべき場所へ迷わず進める** ことを目的とします。

---

## はじめに（1分で分かる全体像）

### 何ができるのか

- 各取引所の **raw API（SDK 相当）** を主役として利用できる
- HTTP / Retry / RateLimit / Observability などの **共通技術基盤**を再利用できる
- 必要な場合のみ、最小限の **共通語彙（Adapter）** を使える

### 何をしないのか

- 複数取引所を束ねる統合クライアント
- クロス取引・アービトラージ・戦略実装

> Raw が主役。共通化は最小。束ねない。

---

## 最初に読む（導線）

1. **クイックスタート**  
   → 実際に API を呼ぶ最短ルート  
   [`quickstart.md`](quickstart.md)

2. **エントリーガイド**  
   → 利用シーン別の考え方・選び方  
   [`entry-guide.md`](entry-guide.md)

3. **設計思想（Architecture）**  
   → なぜこの構造なのか  
   [`Core/README.md`](Core/README.md)

---

## レイヤ別ドキュメント

### Core（技術基盤）

HTTP / Policy / Observability / Error 契約など、
**全取引所で共通の技術基盤**を提供します。

- [`Core/README.md`](Core/README.md)

---

### Common（共通語彙）

DTO / Enum / Interface など、
**意味だけを共有する最小セット**です。

- [`Common/README.md`](Common/README.md)
- [`Common/Contracts/README.md`](Common/Contracts/README.md)

---

### Composition（標準配線）

Credentials / ExchangeInfo / RestClient を組み立て、
**Raw または Adapter を生成する入口**です。

- [`Composition/README.md`](Composition/README.md)
- [`Composition/Factory/README.md`](Composition/Factory/README.md)

---

### Exchanges（取引所実装）

各取引所は **Raw / Adapter** の 2 層で構成されます。

- [`Exchanges/README.md`](Exchanges/README.md)

#### 対応取引所

- Bitflyer  
  - Raw API / Adapter API
- Bittrade  
  - Raw API / Adapter API

---

## どれを使えばよいか？（早見表）

| 目的 | 推奨 |
|---|---|
| 取引所固有機能を使いたい | Raw |
| 完全な制御が必要 | Raw |
| 共通 DTO / Interface で処理したい | Adapter |
| 複数取引所をまとめたい | 対象外（アプリ側で実装） |

---

## テストについて

- Raw / Adapter / Core / Common それぞれで **責務ごとにテストを分離**しています
- Live / Integration テストは別プロジェクトで管理しています

---

## このドキュメントの位置づけ

- 本 README は **docs 全体の入口**です
- 各 README は独立して読めるように設計されています
- 設計の正本はリポジトリ直下の `ARCHITECTURE.md` です

---

## 次に読む

- Raw API を直接使いたい → [`Exchanges/README.md`](Exchanges/README.md)
- Factory の使い方を知りたい → [`Composition/Factory/README.md`](Composition/Factory/README.md)
- エラーや Retry の考え方 → [`Common/Contracts/README.md`](Common/Contracts/README.md)

---

> **Raw first. Minimal abstraction. No unification.**

