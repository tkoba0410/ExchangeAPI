# ExchangeAPI v2.0.0 Release Checklist

位置づけ: release 前チェックリスト

この文書は、`v2.0.0` を publish / release する直前に確認する作業順を固定する。  
設計正本ではない。契約判断は `docs/spec.md`、endpoint matrix、adapter 正本を優先する。

## 1. Preflight

repo root で release preflight を実行する。

```bash
bash scripts/run-v2-release-preflight.sh 2.0.0-local.preflight
```

release 前の最終確認では、publish 予定 version と混同しない local suffix を使う。

```bash
bash scripts/run-v2-release-preflight.sh 2.0.0-local.final
```

含まれる確認:

- `dotnet build ExchangeApi.slnx`
- `dotnet test ExchangeApi.slnx --no-build`
- `bash scripts/pack-local-nuget.sh <version>`
- `bash scripts/smoke-local-nuget-consumer.sh <version>`
- `bash scripts/publish-cli-local.sh`
- `bash scripts/publish-mcp-local.sh`

safe live verification も含める場合だけ、明示 opt-in する。

```bash
EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1 bash scripts/run-v2-release-preflight.sh 2.0.0-local.preflight
```

write live test は release 前の通常手順に含めない。必要な場合は endpoint ごとの risk / runbook に基づき、別判断で実行する。

## 2. Package Publish

ここから先は release 実行時の作業であり、release 前の最終確認では実行しない。

正式 version で package を生成する。

```bash
bash scripts/pack-local-nuget.sh 2.0.0
```

publish 対象:

- `ExchangeApi.Primitives`
- `ExchangeApi.Exchanges.Bitflyer.*`
- `ExchangeApi.Exchanges.Binance.*`
- `ExchangeApi.Optional.*`

GitHub Packages へ push する。

```bash
bash scripts/push-github-packages.sh 2.0.0
```

## 3. Release Assets

CLI / MCP Server は NuGet package ではなく executable artifact として扱う。

```bash
bash scripts/publish-cli-local.sh linux-x64 Release
bash scripts/publish-mcp-local.sh linux-x64 Release
```

生成先:

- `local/publish/cli/linux-x64/exchangeapi`
- `local/publish/mcp/linux-x64/exchangeapi-mcp`

public release では、必要な RID matrix と checksum を release asset 側で扱う。

## 4. Post Publish Verification

この節は publish 後に実行する。release 前の最終確認では未完了でよい。

publish 後に確認する。

- GitHub Packages に `2.0.0` の library / optional package が見える
- CLI / MCP executable が NuGet package として publish されていない
- `ExchangeApi.Optional.Credentials` が publish 対象に含まれている
- 外部 consumer で `ExchangeApi.Exchanges.Bitflyer.Composition` を restore/build できる
- 外部 consumer で `ExchangeApi.Optional.Credentials` を restore/build できる

## 5. Documentation

release 前に確認する。

- `README.md` の公開固定点を `v2.0.0` へ更新するか判断する
- `docs/distribution.md` の current published baseline を更新する
- `docs/guides/package-publish.md` の publish 後確認欄に `v2.0.0` 結果を追記する
- `docs/release-notes/v2.0.0.md` が GitHub Release 本文として使える状態である
- `docs/migration-v2.0.0.md` が v1 利用者向けに読める状態である

## 6. Do Not Commit

次は commit しない。

- `local/nuget/*.nupkg`
- `local/publish/**`
- `local/evidence/**` の run directory
- credentials、署名値、API key / secret を含む出力
