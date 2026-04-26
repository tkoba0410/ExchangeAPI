# ExchangeAPI v3.0.0 Plan

最終更新: 2026-04-26
位置づけ: v3.0.0 実施計画

本書は `v3.0.0` の実施範囲を固定する。
`v3.0.0` では破壊的変更を許容し、package / project consolidation を主題にする。

## 1. 目的

`v3.0.0` では、v2 系で層別に公開していた venue package / project を、venue 単位 package / project に整理する。

目的:

- 外部 consumer の第一導線を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` にする
- `Vocabulary` / `Protocol` / `Native` / `Composition` の層別 package を公開導線から外す
- `Vocabulary` / `Protocol` / `Native` / `Composition` の層別 project を削除する
- package 数を減らし、利用者が選ぶ package 名を venue 単位にする
- 公開 package 単位と repo 内 project 単位を一致させる
- namespace / facade / endpoint contract は必要最小限の変更に留める

## 2. 採用範囲

### 2.1 Package Consolidation

追加する package:

- `ExchangeApi.Exchanges.Bitflyer`
- `ExchangeApi.Exchanges.Binance`

外部 consumer の推奨参照:

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer --version 3.0.0
dotnet add package ExchangeApi.Exchanges.Binance --version 3.0.0
```

v3.0.0 では、次の v2 package を publish 対象から外す。

- `ExchangeApi.Exchanges.Bitflyer.Vocabulary`
- `ExchangeApi.Exchanges.Bitflyer.Protocol`
- `ExchangeApi.Exchanges.Bitflyer.Native`
- `ExchangeApi.Exchanges.Bitflyer.Composition`
- `ExchangeApi.Exchanges.Binance.Vocabulary`
- `ExchangeApi.Exchanges.Binance.Protocol`
- `ExchangeApi.Exchanges.Binance.Native`
- `ExchangeApi.Exchanges.Binance.Composition`

維持する package:

- `ExchangeApi.Primitives`
- `ExchangeApi.Optional.Credentials`
- `ExchangeApi.Optional.Logging`

### 2.2 Project Consolidation

v3.0.0 では、内部の層別 project も削除し、venue ごとに 1 project へ統合する。
`Protocol` / `Native` / `Composition` / `Vocabulary` は package / project 境界ではなく、folder / namespace / tests 上の設計境界として維持する。

削除する project:

- `src/Exchanges/Bitflyer/Vocabulary/ExchangeApi.Exchanges.Bitflyer.Vocabulary.csproj`
- `src/Exchanges/Bitflyer/Protocol/ExchangeApi.Exchanges.Bitflyer.Protocol.csproj`
- `src/Exchanges/Bitflyer/Native/ExchangeApi.Exchanges.Bitflyer.Native.csproj`
- `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
- `src/Exchanges/Binance/Vocabulary/ExchangeApi.Exchanges.Binance.Vocabulary.csproj`
- `src/Exchanges/Binance/Protocol/ExchangeApi.Exchanges.Binance.Protocol.csproj`
- `src/Exchanges/Binance/Native/ExchangeApi.Exchanges.Binance.Native.csproj`
- `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`

維持する project:

- `src/Exchanges/Bitflyer/ExchangeApi.Exchanges.Bitflyer.csproj`
- `src/Exchanges/Binance/ExchangeApi.Exchanges.Binance.csproj`

test project の `Protocol.Tests` / `Native.Tests` / `Composition.Tests` という分類名は、設計境界の test taxonomy として当面維持する。
これらは package / project 境界ではない。

## 3. 非対象

以下は `v3.0.0` の初期 slice では扱わない。

- `Unified` 層の実装
- `ExchangeApi.Optional.Resilience`
- credentials provider 拡張
- full MCP client
- MCP write tool
- order / cancel / withdraw / deposit など state-changing operation の追加
- endpoint contract の意味変更
- namespace 全面 rename
- test taxonomy の大規模再編

## 4. Breaking Changes

`v3.0.0` の breaking change:

- venue の層別 package は publish 対象ではなくなる
- venue の層別 project は削除される
- consumer は `ExchangeApi.Exchanges.Bitflyer.Composition` の代わりに `ExchangeApi.Exchanges.Bitflyer` を参照する
- consumer は `ExchangeApi.Exchanges.Binance.Composition` の代わりに `ExchangeApi.Exchanges.Binance` を参照する

namespace と public API surface は、初期 slice では互換性を最大限維持する。
既存 source code の `using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;` などは、venue aggregate package 参照後も維持できる。

## 5. Verification

最低限の実行:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.0.0-local.project-consolidation
bash scripts/smoke-local-nuget-consumer.sh 3.0.0-local.project-consolidation
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- `ExchangeApi.Exchanges.Bitflyer.3.0.0-local.project-consolidation.nupkg` が生成される
- `ExchangeApi.Exchanges.Binance.3.0.0-local.project-consolidation.nupkg` が生成される
- v2 の venue layer package は生成されない
- venue layer project は solution に含まれない
- local consumer smoke は venue aggregate package を参照する
- live tests は opt-in なしで skip する
