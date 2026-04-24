# ExchangeAPI Document Inventory

最終更新: 2026-04-23  
位置づけ: 文書棚卸し

本書は、現行リポジトリ内の文書を `keep / rewrite / archive` に分類する。  
目的は、今後の再編で何を現行正本セットとして扱い、何を履歴へ退避し、何を再構成対象にするかを明確にすることにある。

## 1. Keep

- [`README.md`](../README.md)
  - 入口文書として維持する
- [`docs/docs-architecture.md`](./docs-architecture.md)
  - 文書体系ガイドとして維持する
- [`docs/spec.md`](./spec.md)
  - 共通正本として維持する
- [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md)
  - bitFlyer venue 台帳として維持する
- [`docs/endpoints-binance.md`](./endpoints-binance.md)
  - Binance venue 台帳として維持する
- [`docs/cli.md`](./cli.md)
  - CLI adapter 正本として維持する
- [`docs/mcp-server.md`](./mcp-server.md)
  - MCP Server adapter 正本として維持する
- [`docs/mcp-tool-catalog.md`](./mcp-tool-catalog.md)
  - MCP tool ledger として維持する
- [`docs/verification.md`](./verification.md)
  - endpoint ごとの live / manual verification 判断正本として維持する
- [`docs/guides/library-getting-started.md`](./guides/library-getting-started.md)
- [`docs/guides/cli-getting-started.md`](./guides/cli-getting-started.md)
- [`docs/guides/mcp-getting-started.md`](./guides/mcp-getting-started.md)
- [`docs/guides/credentials-and-auth-provider.md`](./guides/credentials-and-auth-provider.md)
- [`docs/guides/package-publish.md`](./guides/package-publish.md)
- [`docs/guides/troubleshooting.md`](./guides/troubleshooting.md)
- [`docs/distribution.md`](./distribution.md)
- [`docs/local-nuget-consumer.md`](./local-nuget-consumer.md)
- [`docs/breaking-changes-v2.0.0.md`](./breaking-changes-v2.0.0.md)
- [`docs/migration-v2.0.0.md`](./migration-v2.0.0.md)
- [`docs/release-notes/v2.0.0.md`](./release-notes/v2.0.0.md)
- [`docs/roadmap-post-v2.md`](./roadmap-post-v2.md)
  - `v2.0.0` 以降の検討候補として維持する

## 2. Rewrite

- [`docs/spec.md`](./spec.md)
  - version 固有議論や歴史的説明を減らし、共通原則へ寄せる余地がある
- [`docs/cli.md`](./cli.md)
  - adapter 正本として維持しつつ、inventory と原則の境界を引き直す余地がある
- [`docs/mcp-server.md`](./mcp-server.md)
  - MCP 全体契約として維持しつつ、tool ledger との境界を継続点検する

## 3. Archive

- [`docs/archive/plans/stage11.md`](./archive/plans/stage11.md)
  - phase 計画履歴
- [`docs/archive/plans/stage12.md`](./archive/plans/stage12.md)
  - phase 計画履歴
- [`docs/archive/drafts/v2.0.0-overview.md`](./archive/drafts/v2.0.0-overview.md)
  - 旧 `v2.0.0` draft overview
- [`docs/archive/drafts/v2.0.0-details.md`](./archive/drafts/v2.0.0-details.md)
  - 旧 `v2.0.0` draft details
- [`docs/archive/library-bootstrap-and-history.md`](./archive/library-bootstrap-and-history.md)
  - 初期 bootstrap / 実装順 / DoD の履歴
- [`docs/archive/adapter-status-and-history.md`](./archive/adapter-status-and-history.md)
  - adapter の status / verification 履歴
- [`docs/archive/endpoint-history-and-examples.md`](./archive/endpoint-history-and-examples.md)
  - venue endpoint 文書から切り出した履歴メモと代表 contract 例

## 4. Rehome

- [`docs/release-notes/v1.0.0.md`](./release-notes/v1.0.0.md)
  - archive ではなく version 文書の整理先へ移動した
- [`docs/release-notes/v2.0.0.md`](./release-notes/v2.0.0.md)
  - version 文書の整理先として維持する

## 5. 次の再編候補

- 必要なら `docs/spec.md` の補助文書
  - 共通原則と implementation notes の分離
