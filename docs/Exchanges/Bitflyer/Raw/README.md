# bitFlyer Raw API

本ディレクトリは、**bitFlyer Lightning REST API** をそのまま扱うための **Raw API ドキュメント群**です。

Raw API は公式 API の **鏡像（faithful mapping）** として設計されており、
取引所固有の構造・語彙・制約を **抽象化せずに公開**します。

> Raw first. Faithful mapping. No abstraction.

---

## このディレクトリの読み方

bitFlyer Raw API を理解・利用する際は、以下の順で読むことを推奨します。

1. **API 実装対応表**（まず全体像を把握）  
   → [`ApiMap.md`](ApiMap.md)

2. **命名・設計規則**（なぜこの名前・形なのか）  
   → [`../../Raw/Naming.md`](../../Raw/Naming.md)

3. **利用例・最短ルート**（実際に呼ぶ）  
   → `Quickstart.md`（予定）

---

## Raw API の位置づけ

```
Application
   ↑
Adapter API（任意）
   ↑
bitFlyer Raw API（本ディレクトリ）
   ↑
Core / HTTP / Policy
```

- Raw API は **常に第一選択**です
- Adapter は Raw API を利用した翻訳層であり、必須ではありません
- Raw API は Adapter の都合を考慮しません

---

## 何が含まれるか

このディレクトリには、以下の情報を用途別に分割して配置します。

- **ApiMap.md**  
  公式 REST API の Endpoint と Raw / 抽象層での対応関係一覧

- **Quickstart.md**（予定）  
  bitFlyer Raw API を最短で利用するための例

- **Requests.md**（予定）  
  Request DTO（Body / Query 集約）の一覧と用途

- **Errors.md** / **Constraints.md**（必要に応じて追加）  
  bitFlyer 固有のエラーや制約

---

## Raw API が提供しないもの

- 複数取引所を横断する統合 API
- 意味の補完・推測・共通化
- 業務ロジック・取引戦略

それらは **Application** または **Adapter** の責務です。

---

## 補足

- 抽象層で未露出の API も、Raw 層では **意図的に保持**されます
- 抽象層への昇格は、ユースケースが明確になった段階で検討します

---

> Raw API は例外を恐れない。ただし例外は文書に残す。

