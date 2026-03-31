# Distribution Guide

## Purpose

この文書は、外部利用可能な成果物の**正式導線**、**生成先**、**git 管理方針**を固定する。

対象:

- Library
- CLI
- MCP Server

生成物そのものを git 管理するのではなく、**生成方法と生成先を repo で固定する**ことを目的とする。

## Policy

- source of truth は source code / docs / scripts に置く
- 生成された `.nupkg` や executable は git 管理しない
- 生成物は `local/` 配下に集約する
- 外部利用者向けには「何をどう生成するか」を文書で固定する

## Artifact Layout

### Library

- project:
  - `src/Exchanges/*`
  - `src/Primitives`
- formal distribution path:
  - `ProjectReference`
  - local NuGet feed
- generated output:
  - `local/nuget/`
- generation command:

```bash
bash scripts/pack-local-nuget.sh
```

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
- `scripts/pack-local-nuget.sh`
- `scripts/publish-cli-local.sh`
- `scripts/publish-mcp-local.sh`
