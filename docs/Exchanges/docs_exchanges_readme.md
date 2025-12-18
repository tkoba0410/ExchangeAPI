# Exchanges

> このディレクトリは、各取引所実装の **Raw / Adapter 構造**を説明するための入口です。
> 5分で「どこを使えばよいか」「何を期待してはいけないか」が分かることを目的とします。

---

## 全体構造

```text
src/Exchanges/
├─ Bitflyer/
│  ├─ Raw/
│  └─ Adapter/
└─ Bittrade/
   ├─ Raw/
   └─ Adapter/
```

- **Raw**：取引所の生 API（SDK 相当）。主役。
- **Adapter**：Raw を Common 語彙に適合させる薄いラッパ。

---

## Raw とは

### 役割

- 取引所が提供する API を **忠実に・完全に** 露出する
- 署名、エンドポイント、レスポンス構造を歪めない

### 特徴

- Common（DTO / Interface）を参照しない
- 取引所固有のモデル・Enum をそのまま使う
- 公開 API は資格情報なしでも利用可能

### 使うべきケース

- 取引所固有機能を使いたい
- 完全な制御が必要
- 仕様差を明示的に扱いたい

---

## Adapter とは

### 役割

- Raw API を **Common 語彙**（DTO / Interface）へ写像する
- 複数取引所で共通化できる最小操作を提供する

### 特徴

- Raw を内部に保持する
- ErrorCategory など「失敗の意味」を共通契約に落とす
- 統合・束ね・戦略は行わない

### 使うべきケース

- 共通 DTO / Interface で処理を書きたい
- 複数取引所で同じロジックを再利用したい

---

## やらないこと（重要）

- 複数取引所を束ねる API
- Unified / MultiExchange / Registry 的機能
- クロス取引・アービトラージ
- 取引戦略・ワークフロー

これらは **利用者（アプリケーション）側の責務**です。

---

## どれを使うべきか迷ったら

> **Raw を使ってください。**

Adapter は補助的な層であり、Raw が主役です。

---

## 取引所別 README

- [`Bitflyer/README.md`](Bitflyer/README.md)
- [`Bittrade/README.md`](Bittrade/README.md)

---

## 関連ドキュメント

- `ARCHITECTURE.md`（設計憲章）
- `docs/Composition/Factory/README.md`（生成と配線）
- `docs/Common/`（共通語彙）

