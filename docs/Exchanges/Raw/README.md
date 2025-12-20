# Exchanges / Raw レイヤ

本ドキュメントは **ExchangeAPI における Raw レイヤの設計思想・責務・使い方** を説明します。

Raw レイヤは、各取引所が提供する **公式 API をそのまま扱うための最下位の取引所 API 層**です。

> Raw API は公式 API の鏡像である。意味を足さない。揃えない。

---

## Raw レイヤの位置づけ

```
Application
   ↑
Adapter API（任意）
   ↑
Raw API（本レイヤ）
   ↑
Core / HTTP / Policy
```

- Raw API は **常に第一選択**
- Adapter は Raw API を利用する上位レイヤ
- Raw API は Adapter の都合を考慮しない

---

## Raw API が提供するもの

Raw API は、公式 API ドキュメントに記載されている内容を **できるだけ忠実にコードへ写像**します。

- エンドポイント単位の API
- 取引所固有の概念・制約・仕様
- Public / Private API の区別
- REST / WebSocket API（存在する場合）

### Raw API が提供しないもの

- 複数取引所を跨ぐ共通操作
- 意味の補完・推測・自動変換
- 取引戦略・業務ロジック

---

## 設計原則

### 1. 公式 API への忠実性

- 公式 API に存在する操作のみを提供する
- エンドポイントと **1対1対応**するメソッド設計
- パラメータ・レスポンスの意味を変えない

### 2. 最小限の整形のみ行う

- 命名は C# の慣習に合わせて調整する
- 型付けは行うが、意味は加工しない
- 取引所固有の癖はそのまま露出する

### 3. Adapter を意識しない

- 共通 DTO / Interface は使わない
- Adapter での利用しやすさを理由に API を歪めない

---

## 構成

各取引所の Raw API は以下の構成を持ちます。

```
Exchanges/
  Bitflyer/
    Raw/
      XxxRawApi.cs
      Requests/
      Responses/
```

- `XxxRawApi` : 取引所 Raw API クライアント
- `Requests` / `Responses` : Raw 専用 DTO

---

## 生成と利用

Raw API は **Composition / Factory** を通して生成します。

- 認証情報
- HTTP クライアント
- Retry / RateLimit / Observability

はすべて外部から注入されます。

Raw API 自身は **通信手段や認証情報の管理を行いません**。

---

## 非同期・例外・エラー

- Raw API は **すべて非同期 API**
- エラーは `Common.Contracts` で定義された契約に従う
- HTTP / 通信例外を握りつぶさない

---

## 命名規約

Raw API のメソッド名・クラス名・DTO 名の命名規約は、
以下のドキュメントに分離して定義します。

- **Raw API 命名規約** → `Naming.md`

本 README では「何を提供する層か」に集中します。

---

## Raw API を使うべき場面

- 取引所固有機能を最大限使いたい
- API の挙動を正確に把握・制御したい
- Adapter の抽象が不要、または邪魔な場合

---

## まとめ

- Raw レイヤは **取引所公式 API の直接表現**
- 意味の統合・抽象化は行わない
- Adapter は任意の上位翻訳層

> Raw first. Faithful mapping. No unification.