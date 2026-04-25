# GitHub Packages Publish Guide

この文書は、ExchangeAPI の library / optional package を GitHub Packages へ publish する手順を固定する。
`v1.0.0` では library package を publish 済みであり、この文書は再現手順と次回 publish の基準を兼ねる。

注記:

- 現在の公開固定点は `v2.0.0` である
- 本書の `2.0.0` command 例は `v2.0.0` の publish 手順を示す
- `v2.0.0` publish 前の確認では、`2.0.0-local.*` のような local package version を使う
- `v2.0.0` publish 前の最終確認では、publish/tag/release は実行せず、`2.0.0-local.final` などの local version で preflight する

## Scope

publish 対象は library package と optional package とする。

- `ExchangeApi.Primitives`
- `ExchangeApi.Exchanges.Bitflyer.*`
- `ExchangeApi.Exchanges.Binance.*`
- `ExchangeApi.Optional.*`

次は package publish 対象にしない。

- `exchangeapi`
- `exchangeapi-mcp`

CLI / MCP Server は executable であり、release asset 側を正式導線とする。

v2 方針:

- `ExchangeApi.Optional.Credentials` は NuGet publish 対象に含める
- optional package は core library の必須依存ではない
- `age` などの credential storage / decrypt recipe は optional package または adapter executable 側へ寄せる

## 1. Local Pack

repo root で次を実行する。

```bash
bash scripts/pack-local-nuget.sh 2.0.0
```

生成先:

- `local/nuget/`

release 前に static test、local pack、local consumer smoke、CLI/MCP executable publish をまとめて確認する場合は、次を使う。

```bash
bash scripts/run-v2-release-preflight.sh 2.0.0-local.preflight
```

safe live verification まで含める場合だけ、次のように明示 opt-in する。

```bash
EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1 bash scripts/run-v2-release-preflight.sh 2.0.0-local.preflight
```

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
bash scripts/push-github-packages.sh 2.0.0
```

script を使わず個別 push したい場合は、`dotnet nuget push` を直接使ってよい。

```bash
dotnet nuget push "local/nuget/ExchangeApi.Primitives.2.0.0.nupkg" \
  --source "https://nuget.pkg.github.com/tkoba0410/index.json" \
  --api-key "$GITHUB_TOKEN" \
  --skip-duplicate
```

## 5. Verify

publish 後は GitHub Packages で package 一覧を確認する。

少なくとも次を確認する。

- publish 対象 version が見える
- CLI / MCP executable package が publish されていない
- `Composition` package が見える
- `v2.0.0` では `ExchangeApi.Optional.Credentials` package が見える

加えて、少なくとも 1 本は consumer 側で restore/build/run を確認する。

`v1.0.0` では次を確認済み:

- `ExchangeApi.Exchanges.Bitflyer.Composition 1.0.0`
- restore: GitHub Packages source から成功
- build/run: `BitflyerClientFactory` と `ProductCodes.BtcJpy` の参照成功
- `ExchangeApi.Exchanges.Binance.Composition 1.0.0`
- restore: GitHub Packages source から成功
- build/run: `BinanceClientFactory` と `BinanceSymbols.BtcJpy` の参照成功

`v2.0.0` では次を確認済み:

- GitHub Packages publish: `ExchangeApi.Primitives 2.0.0`
- GitHub Packages publish: `ExchangeApi.Exchanges.Bitflyer.* 2.0.0`
- GitHub Packages publish: `ExchangeApi.Exchanges.Binance.* 2.0.0`
- GitHub Packages publish: `ExchangeApi.Optional.Credentials 2.0.0`
- CLI / MCP executable package が NuGet package として publish されていないことを確認
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer.Composition 2.0.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Binance.Composition 2.0.0`
- GitHub Packages consumer smoke: `ExchangeApi.Optional.Credentials 2.0.0`

## Notes

- local NuGet feed と GitHub Packages feed を混同しない
- `stage` 系と `v1.0.0` は履歴であり、package の current public baseline は `v2.0.0`
- nuget.org 公開はこの文書の対象外
