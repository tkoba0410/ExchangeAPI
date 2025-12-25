# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Exchanges レイヤ

本ドキュメントは **ExchangeAPI における Exchanges レイヤ全体の役割と構成** を説明します。

Exchanges レイヤは、各暗号資産取引所の API を **そのまま扱える Raw API** と、
必要最小限の共通語彙へ翻訳する **Adapter API** を提供します。

> Raw が主役。Adapter は翻訳。

---

## このレイヤの位置づけ

```
Application
   ↑
Adapter API（任意）
   ↑
Raw API（主役）
   ↑
Core / HTTP / Policy
```

- **Raw API** : 公式 API の構造・意味・制約をそのまま表現する
- **Adapter API** : Raw API を利用し、意味だけを共通語彙に翻訳する
- **Application** : Raw / Adapter を用途に応じて直接利用する

Exchanges レイヤは **統合クライアントを提供しません**。
複数取引所を束ねる責務は、アプリケーション側に委ねます。

---

## Raw / Adapter の考え方

### Raw API（第一選択）

Raw API は「公式 API の SDK 相当」です。

- 公式 API と **1対1 で対応**するエンドポイント
- 取引所固有の仕様・制約・癖を **そのまま表現**
- 意味の補完・統合・推測はしない

> Raw API は **事実の写像**である。

### Adapter API（任意）

Adapter API は Raw API の上に構築されます。

- 取引所間の **名称差・構造差の翻訳**
- 共通 DTO / Interface（`Common`）へのマッピング
- 複数 Raw API 呼び出しをまとめた **意味的操作**

> Adapter は **翻訳層**であり、抽象化層ではない。

---

## どれを使うべきか

| 要件 | 推奨 |
|---|---|
| 取引所固有機能を使いたい | Raw API |
| 完全な制御が必要 | Raw API |
| 複数取引所で同じ処理を書きたい | Adapter API |
| 共通 DTO / Interface が欲しい | Adapter API |

---

## ディレクトリ構成

```
Exchanges/
  Bitflyer/
    Raw/
    Adapter/
  Bittrade/
    Raw/
    Adapter/
```

- 各取引所は **独立した実装単位**
- Raw / Adapter は明確に分離

---

## 生成方法（Composition / Factory）

Raw API / Adapter API の生成は **Composition / Factory** を通して行います。

- 認証情報（API Key / Secret）
- ExchangeInfo
- HTTP / Retry / RateLimit

これらはすべて Composition 側で組み立てられ、
Exchanges レイヤは **API 呼び出しの責務のみに集中**します。

---

## 実装上の共通原則

- Raw / Adapter は **状態を持たない**
- HTTP クライアントを自前で生成しない
- エラー契約は `Common.Contracts` に従う
- テストは Raw / Adapter で責務分離する

---

## 関連ドキュメント

### Raw API

- Raw レイヤ概要 → `Raw/README.md`
- Raw API 命名規約 → `Raw/Naming.md`

### Adapter API

- Adapter レイヤ概要 → `Adapter/README.md`

---

## まとめ

- Exchanges レイヤは **取引所 API そのもの**を扱う
- Raw API が常に主役
- Adapter API は必要なときだけ使う

> Raw first. Minimal translation. No unification.