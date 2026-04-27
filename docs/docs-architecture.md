# ExchangeAPI Documentation Architecture

最終更新: 2026-04-27
位置づけ: 文書体系ガイド

## 1. 目的

本書は、ExchangeAPI における文書群の役割分担、正本の置き方、更新規則を定義する。
個別の API 仕様や adapter 契約を増やす前に、どの種類の情報をどの文書へ置くかを先に固定する。

目的は次の 3 点である。

- 文書ごとの責務重複を減らす
- 変更時にどの文書を更新すべきかを明確にする
- `v2.0.0` のような version 単位の検討文書と、恒久的な設計正本を分離する

## 2. 基本方針

- 文書は `入口 / 正本 / 台帳 / ガイド / 実施指示運用 / リリース単位文書 / 計画履歴` に分ける
- 「設計原則」と「対象一覧」を同じ文書に過剰混載しない
- version 固有の変更理由は恒久正本へ直接埋め込まず、ledger や migration 文書へ逃がす
- 正本は実装と test が従う契約だけを持つ
- guide は利用者支援を目的とし、正本そのものにはしない
- 計画文書は履歴として残してよいが、現行契約の説明責務を持たせすぎない

## 3. 文書種別

### 3.1 入口文書

役割:

- repo の入口になる
- 読む順序を示す
- 正本と補助文書への導線を示す

対象:

- [`README.md`](../README.md)

置くべき内容:

- リポジトリ概要
- 現行公開固定点
- 文書マップ
- 最小限の利用導線

置くべきでない内容:

- 詳細な設計規約
- endpoint ごとの exact contract
- version ごとの breaking changes の本文

### 3.2 共通正本

役割:

- library 全体に効く共通設計原則を定義する
- 層モデル、依存規約、error 契約、test 契約を固定する

対象:

- [`docs/spec.md`](./spec.md)

置くべき内容:

- `Protocol` / `Native` / `Unified` / `Composition` / adapter 境界
- 依存方向
- call / error / observability の共通契約
- scalar / nullability / JSON 処理の共通規約
- test taxonomy と live test の基本契約

置くべきでない内容:

- venue ごとの support inventory 全件
- version 固有の rename/remove 理由の列挙
- getting started 的な手順本文

### 3.3 Venue / Surface 台帳

役割:

- 対象 surface の一覧と固定状況を管理する
- exact contract や support boundary の台帳を持つ

対象:

- [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md)
- [`docs/endpoints-binance.md`](./endpoints-binance.md)
- [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md)
- [`docs/cli.md`](./cli.md)
- [`docs/mcp-server.md`](./mcp-server.md)
- [`docs/mcp-tool-catalog.md`](./mcp-tool-catalog.md)
- [`docs/verification.md`](./verification.md)

補足:

- venue endpoint 文書は library 側の inventory / contract ledger を担う
- adapter 文書は adapter 固有契約の正本を担う
- `mcp-tool-catalog.md` は MCP adapter の tool ledger を担う
- `verification.md` は endpoint metadata から live / manual verification の扱いを決める運用正本を担う

置くべき内容:

- 公開対象の範囲
- support / fixed / transitional の状態
- exact contract metadata
- adapter 固有の command / tool / input-output 契約
- endpoint ごとの verification risk と実行配置

置くべきでない内容:

- repo 全体の文書統治
- version 固有の移行説明の本文

### 3.4 利用ガイド

役割:

- 利用者や開発者に手順を示す
- 正本を前提に、実際の使い方へ落とす

対象:

- [`docs/guides/library-getting-started.md`](./guides/library-getting-started.md)
- [`docs/guides/cli-getting-started.md`](./guides/cli-getting-started.md)
- [`docs/guides/mcp-getting-started.md`](./guides/mcp-getting-started.md)
- [`docs/guides/package-publish.md`](./guides/package-publish.md)
- [`docs/guides/troubleshooting.md`](./guides/troubleshooting.md)
- [`docs/distribution.md`](./distribution.md)
- [`docs/local-nuget-consumer.md`](./local-nuget-consumer.md)

置くべき内容:

- セットアップ
- 実行手順
- publish / distribution の運用手順
- troubleshooting

置くべきでない内容:

- 設計判断の唯一根拠
- support inventory の正本

### 3.5 Version 単位文書

役割:

- ある version に閉じた変更を記録する
- 利用者移行と reviewer 判断を助ける

代表例:

- release note
- migration guide
- breaking changes ledger
- version draft overview
- version draft details

置くべき内容:

- 変更理由
- rename / remove / split / merge / contract tighten の一覧
- 利用者影響
- 移行手順

置くべきでない内容:

- version をまたいで維持される恒久原則

### 3.6 実施指示運用

役割:

- チャットで合意した実施指示の保存先と更新規則を定義する
- release scope、non-scope、完了条件、裁定理由を repository 内で追跡可能にする
- `docs/plan-vX.Y.Z.md`、topic doc、roadmap、release note の使い分けを固定する

対象:

- [`docs/work-instruction-policy.md`](./work-instruction-policy.md)

置くべき内容:

- 実施指示の保存原則
- release plan の役割
- topic doc / roadmap / release note との境界
- 作業中に裁定が変わった場合の更新先

置くべきでない内容:

- 個別 release の実施指示本文
- endpoint / adapter / realtime などの exact contract

## 4. 正本の階層

現行の文書主従は次を基本とする。

1. 文書体系ガイド
   - [`docs/docs-architecture.md`](./docs-architecture.md)
2. 入口
   - [`README.md`](../README.md)
   - [`docs/document-inventory.md`](./document-inventory.md)
3. 実施指示運用
   - [`docs/work-instruction-policy.md`](./work-instruction-policy.md)
4. 共通正本
   - [`docs/spec.md`](./spec.md)
5. surface 別正本
   - [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md)
   - [`docs/endpoints-binance.md`](./endpoints-binance.md)
   - [`docs/cli.md`](./cli.md)
   - [`docs/mcp-server.md`](./mcp-server.md)
   - [`docs/mcp-tool-catalog.md`](./mcp-tool-catalog.md)
   - [`docs/verification.md`](./verification.md)
6. 利用ガイド
   - `docs/guides/*`, `docs/distribution.md`, `docs/local-nuget-consumer.md`
7. version 単位文書
   - release note, migration, breaking changes, draft
8. 計画履歴
   - `docs/archive/plans/*`

解釈ルール:

- 共通原則は `spec.md` を優先する
- endpoint exact contract と support metadata は venue 文書を優先する
- adapter 固有契約は `cli.md` / `mcp-server.md` を優先する
- guide が正本と矛盾する場合は guide を修正する
- version 文書は当該 version の変更判断を説明するが、恒久契約そのものを置き換えない

## 5. 粒度の判断基準

文書が曖昧すぎる場合:

- 読んでも更新対象が判断できない
- 原則だけあり、具体的な契約の置き場が不明
- 「必要に応じて」「適宜」が多く、実装判断へ落ちない

文書が決めすぎている場合:

- guide に正本レベルの契約を書いている
- `README` に inventory や exact contract を再保持している
- version 固有の一時判断を `spec.md` に埋め込んでいる
- runtime registry や code から導出すべき一覧を文書へ重複列挙している

適切な粒度の目安:

- `README` は地図に留める
- `spec.md` は原則と境界に集中する
- endpoint 文書は台帳として詳細を持つ
- guide は手順に集中する
- version 差分は version 文書へ分離する

## 6. 更新規則

変更時は、少なくとも次を確認する。

### 6.1 library 共通原則を変更する場合

- [`docs/spec.md`](./spec.md) を更新する
- 影響する venue 文書と adapter 文書を点検する
- guide の前提が変わるなら対応箇所を更新する

### 6.2 endpoint 契約や support boundary を変更する場合

- 対応する venue 文書を更新する
- `spec.md` に共通原則変更がある場合だけ追記する
- CLI / MCP へ波及するなら adapter 文書と test を更新する

### 6.3 adapter 契約を変更する場合

- [`docs/cli.md`](./cli.md) または [`docs/mcp-server.md`](./mcp-server.md) を更新する
- library 共通原則へ波及するかを確認する
- 利用ガイドと examples を更新する

### 6.4 version 固有の breaking change を追加する場合

- version 用の breaking changes ledger を更新する
- 必要なら migration guide を更新する
- 恒久契約へ昇格した内容だけを正本へ反映する

## 7. v2.0.0 への適用方針

`v2.0.0` の検討では、少なくとも次の文書を分けて持つのが望ましい。

- overview
  - 何を見直す phase か
- details
  - 検討論点の棚卸し
- breaking changes ledger
  - 採用候補の一覧
- migration guide
  - 利用者向け移行手順

`v2.0.0` draft は検討文書であり、単独で現行正本を置き換えない。
採用済みの変更は、最終的に `spec.md`、venue 文書、adapter 文書へ反映して固定する。

## 8. 当面の整備方針

- `README.md` は入口と文書マップへ寄せる
- `docs/archive/plans/*` は履歴・計画文書として扱う
- `docs/archive/drafts/*` は検討履歴として扱い、現行正本へ混在させない
- `docs/spec.md` は共通原則へ寄せ、version 固有の議論を増やしすぎない
- venue 文書は endpoint contract ledger として維持する
- `v2.0.0` は draft / ledger / migration を分離して育てる
