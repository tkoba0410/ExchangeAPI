# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Exchange.Common

`Exchange.Common` は、
**取引所ドメインに共通する概念・契約・型定義を集約する領域**である。

このレイヤは、
- 特定の取引所（Bitflyer / Bittrade 等）に依存しない
- しかし「取引所」というドメイン知識を前提とする

という点で、`Common`（技術基盤）とは明確に区別される。

---

## このレイヤの責務

Exchange.Common の責務は、
**取引所 API を扱う上で共通となる「ドメイン語彙」を定義すること**である。

具体的には：

- 取引所共通の DTO / Value Object
- 取引所 API に固有だが共通化できるエラー表現
- Adapter / Factory / Client 間で共有されるドメイン契約

---

## Common との違い

| 観点 | Common | Exchange.Common |
|---|---|---|
| 対象 | 技術基盤 | 取引所ドメイン |
| 依存 | ドメイン非依存 | Exchange 前提 |
| 例 | Transport / Policy / Logging | Order / Balance / ErrorCode |

- **Common** は「どう通信するか」
- **Exchange.Common** は「何を扱うか」

を定義する。

---

## 含まれるもの（例）

※ 実際の型はコードを正とする。

- 共通 DTO
  - Order
  - Balance
  - Ticker
- 取引所共通エラー表現
- 共通列挙型（Side / OrderType 等）

これらは、
どの取引所実装からも利用される前提で設計される。

---

## 含まれないもの

Exchange.Common には、以下を含めない。

- 特定取引所専用の DTO
- API エンドポイントやパス
- HTTP / JSON / Transport 実装
- Retry / Policy の判断ロジック

これらはそれぞれ、
- `Exchange.Bitflyer` / `Exchange.Bittrade`
- `Common`
- `Common.Contracts`

の責務である。

---

## 上位・下位レイヤとの関係

```
[ User / Application ]
           |
        Factory
           |
    ------------------
    | Exchange.Common |
    ------------------
           |
  Exchange.Bitflyer / Exchange.Bittrade
           |
         Common
```

- Exchange.Common は **取引所実装の共通基盤**
- 上位（Factory / Client）は、ここで定義された語彙を前提に振る舞う
- 下位実装は、ここで定義された契約を満たさなければならない

---

## 設計原則

### 1. 最小共通集合に留める

- 全取引所で成立しない概念は入れない
- 無理な共通化は Adapter 側で吸収する

---

### 2. 技術詳細を持ち込まない

- HTTP StatusCode を含めない
- JSON 構造に依存しない
- 再試行・通信戦略を含めない

---

### 3. Factory の API を汚さない

- Exchange.Common の型は、
  Factory の公開 API に自然に現れるものだけに限定する

---

## 将来拡張について

- 新しい取引所が追加されても、
  Exchange.Common を変更せずに済むのが理想
- 変更が必要になった場合は、
  **本当にドメイン共通か**を必ず再検討する

---

## まとめ

- Exchange.Common は **取引所ドメイン共通の語彙集**
- 技術基盤（Common）とは明確に役割を分ける
- Factory と取引所実装をつなぐ中核

このレイヤを安定させることで、
取引所プラグインの追加・差し替えが安全に行える。

