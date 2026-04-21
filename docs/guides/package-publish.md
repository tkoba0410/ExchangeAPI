# GitHub Packages Publish Guide

この文書は、`ExchangeAPI v1.0.0` の library package を GitHub Packages へ publish する手順を固定する。

## Scope

publish 対象は library package のみ。

- `ExchangeApi.Primitives`
- `ExchangeApi.Exchanges.Bitflyer.*`
- `ExchangeApi.Exchanges.Binance.*`

次は package publish 対象にしない。

- `exchangeapi`
- `exchangeapi-mcp`

CLI / MCP Server は executable であり、release asset 側を正式導線とする。

## 1. Local Pack

repo root で次を実行する。

```bash
bash scripts/pack-local-nuget.sh 1.0.0
```

生成先:

- `local/nuget/`

## 2. GitHub Packages Source

GitHub Packages の NuGet feed URL:

```text
https://nuget.pkg.github.com/tkoba0410/index.json
```

## 3. Token / Permission

publish には package write 権限を持つ token が必要。

必要な権限の例:

- `write:packages`
- 必要に応じて `repo`

token は repo に保存しない。

例:

```bash
export GITHUB_TOKEN=...
```

## 4. Push

`dotnet nuget push` を使う。

```bash
bash scripts/push-github-packages.sh 1.0.0
```

script を使わず個別 push したい場合は、`dotnet nuget push` を直接使ってよい。

```bash
dotnet nuget push "local/nuget/ExchangeApi.Primitives.1.0.0.nupkg" \
  --source "https://nuget.pkg.github.com/tkoba0410/index.json" \
  --api-key "$GITHUB_TOKEN" \
  --skip-duplicate
```

## 5. Verify

publish 後は GitHub Packages で package 一覧を確認する。

少なくとも次を確認する。

- version が `1.0.0`
- CLI / MCP executable package が publish されていない
- `Composition` package が見える

## Notes

- local NuGet feed と GitHub Packages feed を混同しない
- `stage` 系は履歴であり、package の current public baseline は `v1.0.0`
- nuget.org 公開はこの文書の対象外
