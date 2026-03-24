# Stage10（ゴール）

最終更新: 2026-03-24  
対象ブランチ: `stage10`

## 1. 位置づけ

本書は、Stage10 の背景、ゴール、非ゴール、設計原則だけを示す goal document である。  
library の詳細仕様は [`docs/spec.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/spec.md) を正本とする。

Stage10 は、既存試作や旧文書の都合から切り離して、`Facade + Endpoint Module` を前提に ExchangeAPI library を再構築する作業である。  
現行ブランチには source / tests / solution 構成が含まれてよいが、設計判断の正本は文書に置く。

## 2. ゴール

- venue ごとの `Protocol` / `Native` client を、同一の Stage10 規約で追加できるようにする
- 公開面は `Facade`、実装単位は `Endpoint Module` とする
- `Native` を exchange-native contract 層として固定する
- `Unified` は、意味同一性を保証できる capability だけを公開する将来層として定義する
- `Native` と `Unified` は層としては分離しつつ、利用者公開面では sibling surface として提示できるようにする
- library と adapter 文書を分離し、CLI / MCP Server を将来追加できるようにする

## 3. 非ゴール

- `Unified` の実装
- CLI の実装
- MCP Server の実装
- retry / rate limiting / circuit breaker の既定実装

## 4. 設計原則

- 設計判断の正本はコードではなく文書に置く
- `Protocol` は venue-specific execution runtime、`Native` は exchange-native contract として責務を分離する
- `Unified` は便利層ではなく、意味同一性を保証できる capability だけを公開する将来層とする
- library と adapter 文書を分離し、利用者向けの入口と設計正本を混ぜない
- venue ごとの差分は endpoint matrix で管理し、library の共通規約は `docs/spec.md` に集約する

## 5. 参照先

- [`docs/spec.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/spec.md)
  - Stage10 library の設計正本
  - 層モデル、依存規約、公開面、Call/Error 契約、test 契約、変更ポリシーを定義する
- [`docs/endpoints-bitflyer.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/endpoints-bitflyer.md)
  - bitFlyer endpoint の運用正本
- [`docs/endpoints-binance.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/endpoints-binance.md)
  - Binance endpoint の運用正本
- [`docs/cli.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/cli.md)
  - CLI adapter の設計補助文書
- [`docs/mcp-server.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/mcp-server.md)
  - MCP Server adapter の設計補助文書
