# Exchanges / Adapter レイヤ

本ドキュメントは **ExchangeAPI における Adapter レイヤ全体の設計思想・責務・使い方** を説明します。

Adapter レイヤは、各取引所の Raw API を利用し、
**取引所ごとの差異を最小限の共通語彙へ翻訳するための層**です。

> Adapter は翻訳層であり、抽象化層ではない。

---

## Adapter レイヤの位置づけ

```
Application
   ↑
Adapter API（本レイヤ）
   ↑
Raw API（第一選択）
   ↑
Core / HTTP / Policy
```

- Adapter は **必須ではない**
- Raw API を直接使える場合は、常に Raw API を優先する
- Adapter は Application と Raw API の間に位置する

---

## Adapter がやること

Adapter レイヤは以下を責務とします。

- 取引所ごとの **名称差・構造差の翻訳**
- 共通 DTO / Interface（`Common`）へのマッピング
- 複数の Raw API 呼び出しをまとめた **意味的操作**

例：

- 取引所ごとに異なる注文・残高レスポンスを共通 DTO に変換
- Public / Private API の差異を吸収

---

## Adapter がやらないこと

- 取引所機能の拡張・補完
- 意味の推測・自動補完
- ビジネスロジック・戦略判断
- 取引所間の自動統合・横断操作

---

## Raw API との関係

- Adapter は **必ず Raw API を利用**する
- Adapter が HTTP を直接呼ぶことはない
- Raw API の制約・癖は Adapter で隠しすぎない

Raw API は **事実の写像**、
Adapter は **意味の翻訳**です。

---

## Adapter を使うべきケース

| 要件 | 推奨 |
|---|---|
| 複数取引所で同じ処理を書きたい | Adapter |
| 共通 DTO / Interface が欲しい | Adapter |
| 単一取引所・完全制御 | Raw API |
| 取引所固有機能を最大限使いたい | Raw API |

---

## 構成

各取引所の Adapter は以下の構成を持ちます。

```
Exchanges/
  Bitflyer/
    Adapter/
      XxxAdapter.cs
  Bittrade/
    Adapter/
      XxxAdapter.cs
```

- Adapter は取引所ごとに独立
- Raw API とは別ディレクトリで管理

---

## 生成と利用

Adapter は **Composition / Factory** を通して生成します。

- Raw API
- 共通 DTO / Interface
- 必要な設定情報

はすべて外部から注入されます。

Adapter 自身は **接続・認証・HTTP 管理を行いません**。

---

## 設計原則

- Adapter は **状態を持たない**
- Adapter は Raw API を **委譲して利用**する
- Raw API の命名・構造を尊重する
- Adapter 固有の例外を作らない

---

## 命名規約・設計規約

Adapter API の命名規約・設計規約は別ドキュメントで管理します。

- Adapter API 命名規約 → （今後追加予定）

本 README では Adapter レイヤの **役割と責務** のみを扱います。

---

## まとめ

- Adapter は **任意の翻訳レイヤ**
- Raw API が常に主役
- Adapter は意味を揃えるが、機能は盛らない

> Raw first. Minimal translation. No unification.

