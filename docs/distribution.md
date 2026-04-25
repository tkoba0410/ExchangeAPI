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
  - `v1.0.0`
- current published verification:
  - `ExchangeApi.Exchanges.Bitflyer.Composition v1.0.0` の consumer smoke test を確認済み
  - `ExchangeApi.Exchanges.Binance.Composition v1.0.0` の consumer smoke test を確認済み

v2 方針:

- `v2.0.0` でも library は NuGet package を正式導線とする
- 通常利用者は venue ごとの `Composition` package を参照する
- `Protocol` / `Native` / `Vocabulary` / `Primitives` は、必要に応じて個別参照できる package として維持する
- `ProjectReference` は repo 内開発または近接開発向けであり、外部 consumer の第一導線にはしない

### Optional Packages

optional package は、core library の責務を薄く保つための追加 NuGet package として扱う。

v2 初手の対象:

- `ExchangeApi.Optional.Credentials`

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

実装状態:

- `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj` は solution に含める
- `scripts/pack-local-nuget.sh` は solution pack により `ExchangeApi.Optional.Credentials` を生成対象に含める
- `scripts/push-github-packages.sh` は `ExchangeApi.Optional.*.<version>.nupkg` を publish 対象に含める
- package publish guide と local consumer guide は `ExchangeApi.Optional.Credentials` の参照例を含める

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
- public release では GitHub Releases asset など、executable を直接取得できる導線を想定する
- executable artifact の正式 name、RID matrix、checksum の有無は release 手順側で固定する
- v2 初手では executable 配布方式の大規模変更は行わない

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
- `scripts/pack-local-nuget.sh`
- `scripts/push-github-packages.sh`
- `scripts/smoke-local-nuget-consumer.sh`
- `scripts/publish-cli-local.sh`
- `scripts/publish-mcp-local.sh`
- `scripts/run-safe-live-tests.sh`
