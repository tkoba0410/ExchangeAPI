# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Architecture Charter

> この文書は ExchangeAPI の設計方針（ぶれない芯）を固定するための「憲章」です。
> 実装・提案・思いつきが増えても、この憲章に反する変更は行いません。

---

## 1. 目的

ExchangeAPI は、複数の暗号資産取引所に対して
- **取引所ごとの raw API（SDK相当）**を主役として提供し
- 必要最小限の **共通語彙（DTO/Enum/Interface）** と
- **標準配線（通信・認証・観測の既定構成）** を提供する

ことを目的とする。

本プロジェクトは、取引所間の差分を過剰に隠蔽する「重い統合クライアント」を目標としない。

また、失敗の意味（例外・カテゴリ・メタ情報）を **Core.Contracts** に集約し、Transport/Policy/Observability が同じ契約で連携できるようにする。

---

## 2. 非目標（やらないこと）

以下は意図的にスコープ外とする。

- **複数取引所を束ねる統合クライアント**（Unified / MultiExchange / Registry 等）
- 取引所差分を完全に吸収する「万能な抽象 API」
- 取引戦略 DSL / ワークフローエンジン
- 取引の整合性（原子的クロス取引）をライブラリ側で保証すること

> 補足：クロス取引や高度な統合は、利用者（アプリケーション）側の責務とする。

---

## 3. 論理レイヤ（責務分離）

本プロジェクトは以下の 4+1 レイヤで構成する。

### 3.1 Core（技術基盤）
- HTTP/Transport
- Policy（Retry/RateLimit/Timeout/CircuitBreaker 等）
- Observability（Logger/Observer/OTel/Metrics 等）
- 失敗の「意味」を表す契約（Contracts）

**原則**：Core は取引所ドメイン（各取引所固有の仕様・型）を知らない。

### 3.2 Common（共通語彙）
- DTO / Enum / Interface（最小）
- 純粋な Extension（計算・変換のみ。IO を伴わない）

**原則**：Common は「語彙」であり、組み立てない・通信しない・設定を読まない。

### 3.3 Composition（標準配線）
- Credentials の取得（Env / File / Windows 等）
- ExchangeInfo の読み込み（例：Json）
- RestClient の既定構成（Policy/Observer を含む）

**原則**：Composition は「どう組むか」を提供する。束ねない。

**生成物の既定**：Composition が提供する生成の入口は **Raw を既定**とし、Adapter（共通I/F）は **明示的に選択**された場合にのみ生成・返却する。

### 3.4 Exchanges（取引所実装：Raw/Adapter）
各取引所は 2 層で実装する。

- **Raw**：取引所SDK相当。取引所の生 API に忠実。
- **Adapter**：Raw をラップし、Common の語彙へ適合させる（最小の共通化）。

**原則**：Raw が主役。Adapter は薄い。

---

## 4. フォルダ構成（正）

```text
src/
├─ Core/
├─ Common/
├─ Composition/
└─ Exchanges/
   ├─ Bitflyer/
   │  ├─ Raw/
   │  └─ Adapter/
   └─ Bittrade/
      ├─ Raw/
      └─ Adapter/
```

---

## 5. 依存関係ルール（絶対）

### 5.1 依存方向

- `Core` は最下層（他プロジェクト参照を持たない）
- `Common` は `Core` を参照してよい
- `Composition` は `Core` と `Common` を参照してよい
- `Exchanges/*/Raw` は `Core` を参照してよい（必要最小限）
- `Exchanges/*/Adapter` は `Raw` + `Common` + `Core` を参照してよい

### 5.2 Raw と Common の関係（固定）

- **Raw は Common を参照しない**。
- 共通語彙（DTO/Enum/Interface）への適合・写像は **Adapter の責務**である。

### 5.3 禁止

- `Core` → `Common` / `Composition` / `Exchanges` の参照
- `Raw` → `Adapter` の参照
- `Common` の中に IO（Http/File/Env/Clock/Json）を入れること
- `Unified` / `MultiExchange` 等の束ね機能を追加すること

---

## 6. Raw と Adapter の責務

### 6.1 Raw（取引所SDK相当）

Raw は以下を提供してよい。
- 取引所のエンドポイント呼び出し
- 署名（signer）
- 取引所固有のモデル（raw model）
- 取引所固有のエラー分類/パース（必要なら）

Raw は以下を **しない**。
- Common DTO への写像（Common の意味へ合わせない）
- 共通I/Fの都合に合わせて API を歪める

### 6.2 Adapter（共通化ラッパ）

Adapter は以下を行ってよい。
- Raw → Common DTO への写像（Mapper/Adapter）
- Common Interface（最小）を実装し、共通処理を書きやすくする
- エラーを `Core.Contracts.Errors` のカテゴリへ対応付ける

Adapter は以下を **しない**。
- 取引所間を束ねる
- Adapter 自体が高度な戦略（アービトラージ等）を持つ
- Raw の機能を隠す（Raw は常に利用可能）

---

## 7. 抽象 API を増やす基準（ガードレール）

Common Interface / Common DTO を増やすのは、次の条件を全て満たす場合のみ。

1. 複数取引所で **意味が一致**する
2. 取引戦略・運用で **頻出**する
3. 失敗の意味が `ErrorCategory` 等で **共通表現**できる
4. 取引所差分は Adapter の写像だけで吸収でき、I/Fが嘘にならない

条件を満たさないものは Raw に残す。

---

## 8. 公開 API の考え方

- Raw の public API は拡張し続けられることを優先する
- Adapter の public API は **最小**で固定し、破壊的変更を避ける
- まず concrete class で提供し、固まったもののみ interface 化する

---

## 9. テスト方針（最小）

- `Core`：Transport/Policy/Observability のユニットテスト
- `Common`：DTO/Extension の不変条件テスト
- `Raw`：署名・リクエスト構築・モデル変換（取引所仕様に忠実）
- `Adapter`：Raw→Common 写像と ErrorMapping のテスト
- Live/Integration は別プロジェクトとして切り出す

---

## 10. 変更を提案するときのチェックリスト

PR/提案は以下を必ず満たすこと。

- [ ] この憲章（非目標・依存ルール）に反していない
- [ ] Raw 主役が維持されている（Adapter が肥大化していない）
- [ ] Common に IO/実装が混入していない
- [ ] 束ね機能（Unified/MultiExchange）が復活していない
- [ ] 依存方向が一方向のまま

---

## 11. 付録：よくある誤解

- 「抽象 API を作れば楽になる」
  - 取引所差分が多い領域では嘘になる。共通化は最小にする。

- 「統合クライアントが最大の価値」
  - 本プロジェクトでは価値の中心は raw SDK と標準配線。
  - クロス取引の統合は利用者（アプリ側）で行う。

