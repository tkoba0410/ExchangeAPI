# Library Bootstrap And History Notes

最終更新: 2026-04-22  
位置づけ: アーカイブ補助文書

本書は、過去 phase で使っていた初期対象 endpoint、bootstrap 手順、実装順、DoD、既存実装の流用判断を履歴として残す。  
現行の library 正本は [`../spec.md`](../spec.md) であり、本書を直接の契約根拠にしない。

## 1. 初期対象 endpoint

過去の初期優先対象は次だった。

- `GetMarkets`
- `GetBoard`
- `GetExecutionsPublic`
- `GetTicker`
- `GetBalance`
- `GetCollateral`
- `GetCollateralAccounts`
- `GetChildOrders`
- `GetExecutionsPrivate`
- `GetPositions`
- `GetCollateralHistory`
- `GetTradingCommission`
- `SendChildOrder`
- `CancelChildOrder`
- `CancelAllChildOrders`

当時の役割分担:

- `GetMarkets`
  - public top-level array response の template
- `GetBoard`
  - public object + nested array response の template
- `GetExecutionsPublic`
  - public paging/filter array response の template
- `GetTicker`
  - public read の template
- `GetBalance`
  - private read と top-level array 契約の template
- `GetCollateral`
  - private object response の template
- `GetCollateralAccounts`
  - private array response の空 request template
- `GetChildOrders`
  - optional query と paging/filter を持つ private read endpoint の template
- `GetExecutionsPrivate`
  - required query + optional paging/filter を持つ private read endpoint の template
- `GetPositions`
  - required query を持つ private read endpoint の template
- `GetCollateralHistory`
  - paging only private read endpoint の template
- `GetTradingCommission`
  - required query + object response を持つ private read endpoint の template
- `SendChildOrder`
  - private write と request encode の template
- `CancelChildOrder`
  - 注文 lifecycle 補助 endpoint の template
- `CancelAllChildOrders`
  - destructive private write + `Unit` response の template

## 2. 過去実装の扱い

### 2.1 正本にしないもの

- git history 上の file 配置
- `partial` 前提の facade 実装
- facade に endpoint 実装を直接生やす構成
- `Native` の validation 実装を中央集約フォルダ構成の正本として扱うこと

### 2.2 流用してよいもの

- transport
- signer
- runtime
- DTO 契約
- encoder / converter / validator の中身
- test assertion
- live test 基盤

### 2.3 判断原則

- 過去コードの場所ではなく、新しい責務境界を優先する
- 「そのまま残せるか」ではなく「新しい endpoint module へ安全に移せるか」で流用可否を判断する

## 3. Blank-Slate Bootstrap

blank slate から実装を再開する際の当時の想定は次だった。

```text
src/Exchanges/Bitflyer/
  Protocol/
  Native/
  Composition/
  Vocabulary/
tests/Exchanges/Bitflyer/
  Protocol.Tests/
  Native.Tests/
  Composition.Tests/
  LiveTests/

src/Exchanges/Binance/
  Protocol/
  Native/
  Composition/
  Vocabulary/
tests/Exchanges/Binance/
  Protocol.Tests/
  Native.Tests/
  Composition.Tests/
  LiveTests/
```

当時の原則:

- venue ごとに `Protocol` / `Native` / `Composition` の 3 project を作る
- 次に `Protocol.Tests` / `Native.Tests` / `Composition.Tests` を作る
- `LiveTests` は read endpoint の parity が通ってから追加する
- `ExchangeApi.slnx` は上記 project を追加するまで空のままでよい

### 3.1 Bootstrap Manifest

```text
src/Exchanges/Bitflyer/Protocol/ExchangeApi.Exchanges.Bitflyer.Protocol.csproj
  RootNamespace: ExchangeApi.Exchanges.Bitflyer.Protocol

src/Exchanges/Bitflyer/Native/ExchangeApi.Exchanges.Bitflyer.Native.csproj
  RootNamespace: ExchangeApi.Exchanges.Bitflyer.Native

src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj
  RootNamespace: ExchangeApi.Exchanges.Bitflyer.Composition

tests/Exchanges/Bitflyer/Protocol.Tests/ExchangeApi.Exchanges.Bitflyer.Protocol.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests

tests/Exchanges/Bitflyer/Native.Tests/ExchangeApi.Exchanges.Bitflyer.Native.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests

tests/Exchanges/Bitflyer/Composition.Tests/ExchangeApi.Exchanges.Bitflyer.Composition.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Bitflyer.Composition.Tests

tests/Exchanges/Bitflyer/LiveTests/ExchangeApi.Exchanges.Bitflyer.LiveTests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests

src/Exchanges/Binance/Protocol/ExchangeApi.Exchanges.Binance.Protocol.csproj
  RootNamespace: ExchangeApi.Exchanges.Binance.Protocol

src/Exchanges/Binance/Native/ExchangeApi.Exchanges.Binance.Native.csproj
  RootNamespace: ExchangeApi.Exchanges.Binance.Native

src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj
  RootNamespace: ExchangeApi.Exchanges.Binance.Composition

src/Exchanges/Binance/Vocabulary/ExchangeApi.Exchanges.Binance.Vocabulary.csproj
  RootNamespace: ExchangeApi.Exchanges.Binance.Vocabulary

tests/Exchanges/Binance/Protocol.Tests/ExchangeApi.Exchanges.Binance.Protocol.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Binance.Protocol.Tests

tests/Exchanges/Binance/Native.Tests/ExchangeApi.Exchanges.Binance.Native.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Binance.Native.Tests

tests/Exchanges/Binance/Composition.Tests/ExchangeApi.Exchanges.Binance.Composition.Tests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Binance.Composition.Tests

tests/Exchanges/Binance/LiveTests/ExchangeApi.Exchanges.Binance.LiveTests.csproj
  RootNamespace: ExchangeApi.Tests.Exchanges.Binance.LiveTests
```

当時の project reference 正本:

- `Native` -> `Protocol`
- `Composition` -> `Protocol`, `Native`
- `Protocol.Tests` -> `Protocol`
- `Native.Tests` -> `Native`, `Protocol`
- `Composition.Tests` -> `Composition`, `Protocol`, `Native`
- `LiveTests` -> `Composition`, `Protocol`, `Native`

venue-specific `Vocabulary` project を作る場合の当時の正本:

- `Protocol` -> `Vocabulary`
- `Native` -> `Vocabulary`, `Protocol`
- `Composition` -> `Vocabulary`, `Protocol`, `Native`

## 4. 実装順

1. 文書を正本として固定する
2. `Protocol` / `Native` の `GetMarkets` を facade + endpoint module に移す
3. `GetTicker` を移す
4. `GetBoard` を移す
5. `GetExecutionsPublic` を移す
6. `GetBalance` を移す
7. `GetCollateral` / `GetCollateralAccounts` を移す
8. `GetChildOrders` / `GetExecutionsPrivate` / `GetCollateralHistory` を移す
9. `GetPositions` を移す
10. `SendChildOrder` / `CancelChildOrder` を移す
11. `CancelAllChildOrders` を移す
12. module 集約 object を導入して facade constructor を整理する
13. `Composition` を更新する
14. test を facade / endpoint module / composition に役割分離する
15. `partial` 依存構成と不要 helper を整理する

### 4.1 Codex 実装戦略

1. endpoint metadata を確認する
2. `Protocol` endpoint module を生成する
3. `Native` DTO を生成する
4. `Native` endpoint module を生成する
5. facade forwarding method を生成する
6. `Composition` で配線する
7. endpoint test / facade test / composition test を追加する

## 5. DoD

- `Protocol` / `Native` の責務境界が明確
- facade と endpoint module の役割分担が明確
- 文書統治が定義され、`docs/spec.md` と matrix の主従が固定されている
- 依存規約が文書化され、破ってよい場所が `Composition` に限定されている
- architecture enforcement の対象が明記されている
- facade の主公開面が `*CallAsync(...)` に固定されている
- `Call` の最低要件と nested `Protocol` call が定義されている
- error kind の使い分けが固定されている
- `Transport` / `Http` / `Codec` / `Semantic` / `Mapping` の境界が定義されている
- `Protocol` endpoint に共通 interface を置かず、endpoint-specific interface を使う方針が定義されている
- `Call` の success / failure 不変条件が定義されている
- `Native` が API contract rule までを扱い、business rule を持たないことが定義されている
- validation stage 語彙が本書に従っている
- test の役割分担が固定されている
- endpoint metadata の必須列が定義されている
- 公開対象 row に `TBD` を残さない規則が定義されている
- compatibility / versioning 方針が定義されている
- write safety 規約が定義されている
- `Native` が exchange-native contract として定義されている
- `Unified` の意味同一性ルールが定義されている
- library と external adapter の境界が定義されている
- `Unified` を上位層として追加できる
- endpoint 運用正本が venue ごとの `docs/endpoints-<venue>.md` に固定されている
- 既存試作は移行材料であって設計正本ではないことが明記されている
