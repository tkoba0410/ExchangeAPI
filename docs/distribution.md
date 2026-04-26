# Distribution Guide

## Purpose

この文書は、外部利用可能な成果物の**正式導線**、**生成先**、**git 管理方針**を固定する。

対象:

- Library
- Optional packages
- CLI
- MCP Server

生成物そのものを git 管理するのではなく、**生成方法と生成先を repo で固定する**ことを目的とする。

## Policy

- source of truth は source code / docs / scripts に置く
- 生成された `.nupkg` や executable は git 管理しない
- 生成物は `local/` 配下に集約する
- 外部利用者向けには「何をどう生成するか」を文書で固定する
- `v2.0.0` では配布方式自体を変更しない
- v2 の追加機能は、必要に応じて library package 群または executable に含める
- `v2.2.0` は operational / verification release として扱い、配布方式自体は変更しない
- `v3.0.0` では venue package を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` に集約する
- generated evidence / logs / release assets は default では作らず、明示 script 実行時のみ `local/` 配下へ作る

## Artifact Layout

### Library

- project:
  - `src/Exchanges/*`
  - `src/Primitives`
- formal distribution path:
  - `ProjectReference`
  - local NuGet feed
  - GitHub Packages
- generated output:
  - `local/nuget/`
- generation command:

```bash
bash scripts/pack-local-nuget.sh
```

- publish guide:
  - `docs/guides/package-publish.md`
- current published baseline:
  - `v2.2.0`
- current published verification:
  - `ExchangeApi.Exchanges.Bitflyer.Composition v1.0.0` の consumer smoke test を確認済み
  - `ExchangeApi.Exchanges.Binance.Composition v1.0.0` の consumer smoke test を確認済み
  - `ExchangeApi.Exchanges.Bitflyer.Composition v2.0.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Exchanges.Binance.Composition v2.0.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Credentials v2.0.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Credentials v2.0.0` の GitHub Packages publish を確認済み
  - `ExchangeApi.Exchanges.Bitflyer.Composition v2.1.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Credentials v2.1.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Logging v2.1.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Logging v2.1.0` の GitHub Packages publish を確認済み
  - `ExchangeApi.Exchanges.Bitflyer.Composition v2.2.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Credentials v2.2.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Logging v2.2.0` の GitHub Packages consumer smoke test を確認済み
  - `ExchangeApi.Optional.Logging v2.2.0` の GitHub Packages publish を確認済み
- v2.2.0 release verification:
  - local consumer smoke は `ExchangeApi.Optional.Logging` を含める
  - GitHub Packages consumer smoke は `scripts/smoke-github-packages-consumer.sh` で確認する
  - package / project consolidation は含めない

v3 方針:

- 通常利用者は venue ごとの aggregate package を参照する
- `ExchangeApi.Exchanges.Bitflyer` は bitFlyer の `Vocabulary` / `Protocol` / `Native` / `Composition` surface を含む
- `ExchangeApi.Exchanges.Binance` は Binance の `Vocabulary` / `Protocol` / `Native` / `Composition` surface を含む
- v2 の layer-specific venue package は v3.0.0 では publish 対象にしない

v2 方針:

- `v2.0.0` でも library は NuGet package を正式導線とする
- 通常利用者は venue ごとの `Composition` package を参照する
- `Protocol` / `Native` / `Vocabulary` / `Primitives` は、必要に応じて個別参照できる package として維持する
- `ProjectReference` は repo 内開発または近接開発向けであり、外部 consumer の第一導線にはしない

v3.0.0 package generation:

- generated:
  - `ExchangeApi.Primitives`
  - `ExchangeApi.Exchanges.Bitflyer`
  - `ExchangeApi.Exchanges.Binance`
  - `ExchangeApi.Optional.Credentials`
  - `ExchangeApi.Optional.Logging`
- not generated:
  - `ExchangeApi.Exchanges.Bitflyer.Vocabulary`
  - `ExchangeApi.Exchanges.Bitflyer.Protocol`
  - `ExchangeApi.Exchanges.Bitflyer.Native`
  - `ExchangeApi.Exchanges.Bitflyer.Composition`
  - `ExchangeApi.Exchanges.Binance.Vocabulary`
  - `ExchangeApi.Exchanges.Binance.Protocol`
  - `ExchangeApi.Exchanges.Binance.Native`
  - `ExchangeApi.Exchanges.Binance.Composition`

### Optional Packages

optional package は、core library の責務を薄く保つための追加 NuGet package として扱う。

v2.0.0 初手の対象:

- `ExchangeApi.Optional.Credentials`

v2.1.0 追加対象:

- `ExchangeApi.Optional.Logging`

役割:

- `PlainText` provider など sample / test / local dev 向け実装を提供する
- `AgeFile` provider など、core から外した credential storage / decrypt recipe を提供する
- `IApiCredentialProvider` / `IApiCredentialSession` の core 契約を実装する

配布方針:

- optional package は NuGet package として配布する
- CLI / MCP executable は、必要な optional 実装を参照して publish artifact に含めてよい
- optional package は core の必須依存にしない
- optional package の追加により、`ExchangeApi.Exchanges.*.Composition` の最小利用者が不要な storage / decrypt 実装を強制参照しないようにする
- optional package の生成先は library package と同じ `local/nuget/` とする
- logging / evidence helper は `ExchangeApi.Optional.Logging` に置き、core library の必須依存にしない

実装状態:

- `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj` は solution に含める
- `src/Optional/Logging/ExchangeApi.Optional.Logging.csproj` は solution に含める
- `scripts/pack-local-nuget.sh` は solution pack により `ExchangeApi.Optional.*` を生成対象に含める
- `scripts/push-github-packages.sh` は `ExchangeApi.Optional.*.<version>.nupkg` を publish 対象に含める
- package publish guide と local consumer guide は `ExchangeApi.Optional.Credentials` と `ExchangeApi.Optional.Logging` の参照例を含める

### CLI

- project:
  - `src/Adapters/Cli/ExchangeApi.Adapters.Cli.csproj`
- formal distribution path:
  - build 済み executable
- generated output:
  - `local/publish/cli/<rid>/exchangeapi`
- generation command:

```bash
bash scripts/publish-cli-local.sh
```

### MCP Server

- project:
  - `src/Adapters/McpServer/ExchangeApi.Adapters.McpServer.csproj`
- formal distribution path:
  - build 済み executable
- generated output:
  - `local/publish/mcp/<rid>/exchangeapi-mcp`
- generation command:

```bash
bash scripts/publish-mcp-local.sh
```

### Release Asset 方針

CLI / MCP Server は NuGet package ではなく executable artifact として扱う。

- local 生成先は `local/publish/<adapter>/<rid>/`
- `v2.0.0` 初手の public release asset は `linux-x64` のみを対象にする
- `v2.0.0` 初手の executable asset name は `exchangeapi-linux-x64` と `exchangeapi-mcp-linux-x64` とする
- `v2.0.0` 初手では各 executable asset と同じ release に SHA-256 checksum を置く
- checksum asset name は `exchangeapi-linux-x64.sha256` と `exchangeapi-mcp-linux-x64.sha256` とする
- `v2.0.0` release では上記 4 asset を GitHub Release に添付済み
- `v2.1.0` release では上記 4 asset を GitHub Release に添付済み
- `v2.2.0` release では上記 4 asset を GitHub Release に添付済み
- `v2.2.0` では `scripts/create-release-assets.sh` を標準 helper とし、`local/publish/release-assets/v<version>/` に上記 4 asset を生成する
- v2 初手では executable 配布方式の大規模変更は行わない
- 複数 RID matrix、installer、archive format、署名付き binary は post-v2 検討とする

v2.2.0 release asset 生成:

```bash
bash scripts/create-release-assets.sh 2.2.0 linux-x64 Release
```

期待 layout:

```text
local/publish/release-assets/v2.2.0/
  exchangeapi-linux-x64
  exchangeapi-linux-x64.sha256
  exchangeapi-mcp-linux-x64
  exchangeapi-mcp-linux-x64.sha256
```

`scripts/create-release-assets.sh` は `scripts/publish-cli-local.sh` と `scripts/publish-mcp-local.sh` を順番に呼ぶ。
CLI / MCP publish は共有 `bin/obj` を使うため、release asset helper 内では並列実行しない。

## Git Policy

- commit するもの:
  - source code
  - docs
  - generation scripts
  - config
- commit しないもの:
  - `local/nuget/*.nupkg`
  - `local/publish/**`
  - local-only credentials / launcher scripts

`local/` 配下の生成物は、再現可能な build output として扱い、repo の正本にはしない。

## References

- `README.md`
- `docs/local-nuget-consumer.md`
- `docs/guides/package-publish.md`
- `docs/release-checklist-v2.0.0.md`
- `docs/release-checklist-v2.2.0.md`
- `scripts/pack-local-nuget.sh`
- `scripts/push-github-packages.sh`
- `scripts/smoke-local-nuget-consumer.sh`
- `scripts/publish-cli-local.sh`
- `scripts/publish-mcp-local.sh`
- `scripts/create-release-assets.sh`
- `scripts/smoke-github-packages-consumer.sh`
- `scripts/run-safe-live-tests.sh`
- `scripts/run-v2-release-preflight.sh`
