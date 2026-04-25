# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための library / adapter 基盤です。  
現行の library 実装範囲では bitFlyer を主対象とし、Binance は public `GetKlines` を主な公開範囲とします。

この `README.md` は入口文書です。  
文書の主従、正本、履歴の扱いは [`docs/docs-architecture.md`](docs/docs-architecture.md) を参照してください。

現時点の公開固定点は **`v2.0.0`** です。
`v2.0.0` の release 手順と確認結果は [`docs/release-checklist-v2.0.0.md`](docs/release-checklist-v2.0.0.md) を参照します。
`stage` 系 tag と `v1.0.0` は履歴として残しますが、現在の安定固定点としては `v2.0.0` を参照します。

## Quick Links

- Documentation architecture: [`docs/docs-architecture.md`](docs/docs-architecture.md)
- Document inventory: [`docs/document-inventory.md`](docs/document-inventory.md)
- Library spec: [`docs/spec.md`](docs/spec.md)
- CLI specification: [`docs/cli.md`](docs/cli.md)
- MCP Server specification: [`docs/mcp-server.md`](docs/mcp-server.md)
- MCP tool catalog: [`docs/mcp-tool-catalog.md`](docs/mcp-tool-catalog.md)
- Verification policy: [`docs/verification.md`](docs/verification.md)
- Bitflyer endpoints: [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md)
- Binance endpoints: [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
- `v2.0.0` breaking changes: [`docs/breaking-changes-v2.0.0.md`](docs/breaking-changes-v2.0.0.md)
- `v2.0.0` migration guide: [`docs/migration-v2.0.0.md`](docs/migration-v2.0.0.md)
- `v2.0.0` release checklist: [`docs/release-checklist-v2.0.0.md`](docs/release-checklist-v2.0.0.md)
- Credentials / auth provider guide: [`docs/guides/credentials-and-auth-provider.md`](docs/guides/credentials-and-auth-provider.md)
- Release note `v1.0.0`: [`docs/release-notes/v1.0.0.md`](docs/release-notes/v1.0.0.md)
- Release note `v2.0.0`: [`docs/release-notes/v2.0.0.md`](docs/release-notes/v2.0.0.md)
- Distribution guide: [`docs/distribution.md`](docs/distribution.md)
- Archive guide: [`docs/archive/README.md`](docs/archive/README.md)

## Reading Order

初見で repo を追う場合は、次の順で読む。

1. [`docs/docs-architecture.md`](docs/docs-architecture.md)
2. [`docs/spec.md`](docs/spec.md)
3. [`docs/cli.md`](docs/cli.md)
4. [`docs/mcp-server.md`](docs/mcp-server.md)
5. [`docs/mcp-tool-catalog.md`](docs/mcp-tool-catalog.md)
6. [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md) / [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
7. [`docs/document-inventory.md`](docs/document-inventory.md)

## Current Scope

- bitFlyer が現行の主対象であり、最も広い実装済み surface を持つ
- Binance は public `GetKlines` を中心とした限定公開範囲を持つ
- `Unified` は未実装
- CLI と MCP Server は利用可能
- endpoint ごとの exact contract は [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md) と [`docs/endpoints-binance.md`](docs/endpoints-binance.md) を正本とする

## Current Doc Set

- 共通正本
  - [`docs/spec.md`](docs/spec.md)
- venue 台帳
  - [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md)
  - [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
- adapter 正本
  - [`docs/cli.md`](docs/cli.md)
  - [`docs/mcp-server.md`](docs/mcp-server.md)
- adapter 補助台帳
  - [`docs/mcp-tool-catalog.md`](docs/mcp-tool-catalog.md)
- verification 正本
  - [`docs/verification.md`](docs/verification.md)
- 利用ガイド
  - [`docs/guides/library-getting-started.md`](docs/guides/library-getting-started.md)
  - [`docs/guides/cli-getting-started.md`](docs/guides/cli-getting-started.md)
  - [`docs/guides/mcp-getting-started.md`](docs/guides/mcp-getting-started.md)
  - [`docs/guides/credentials-and-auth-provider.md`](docs/guides/credentials-and-auth-provider.md)
  - [`docs/guides/package-publish.md`](docs/guides/package-publish.md)
  - [`docs/guides/troubleshooting.md`](docs/guides/troubleshooting.md)

## Getting Started

- library を使い始める場合は [`docs/guides/library-getting-started.md`](docs/guides/library-getting-started.md)
- CLI を使い始める場合は [`docs/guides/cli-getting-started.md`](docs/guides/cli-getting-started.md)
- MCP Server を使い始める場合は [`docs/guides/mcp-getting-started.md`](docs/guides/mcp-getting-started.md)
- credentials / auth provider の扱いは [`docs/guides/credentials-and-auth-provider.md`](docs/guides/credentials-and-auth-provider.md)

## Distribution

- 外部利用向け成果物の正式導線と生成先は [`docs/distribution.md`](docs/distribution.md)
- package publish 手順は [`docs/guides/package-publish.md`](docs/guides/package-publish.md)
- local NuGet consumer 導線は [`docs/local-nuget-consumer.md`](docs/local-nuget-consumer.md)
- v2 release 確認結果は [`docs/release-checklist-v2.0.0.md`](docs/release-checklist-v2.0.0.md)
- v2 の optional credentials 実装は `ExchangeApi.Optional.Credentials` package として扱う
- CLI local publish は `bash scripts/publish-cli-local.sh`
- MCP Server local publish は `bash scripts/publish-mcp-local.sh`

## Archive

- 過去 phase の計画文書と旧 draft は [`docs/archive/README.md`](docs/archive/README.md) を参照する
- 例:
  - [`docs/archive/plans/stage11.md`](docs/archive/plans/stage11.md)
  - [`docs/archive/plans/stage12.md`](docs/archive/plans/stage12.md)
