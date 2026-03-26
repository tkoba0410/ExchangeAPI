# Local NuGet Consumer Guide

この文書は、別の .NET project から ExchangeAPI の local NuGet feed を使う手順を定義する。

## 1. 前提

- ExchangeAPI repository 側で local package を生成済みであること
- consumer project 側が `net10.0` を target できること
- local feed は machine-local 用途であり、共有 feed や公開 feed の代替ではない

ExchangeAPI repository 側では、repo root で次を実行する。

```bash
bash scripts/pack-local-nuget.sh 0.1.0-local.1
```

生成先は `local/nuget`。

## 2. Consumer Repo に Package Source を追加する

consumer repo の root に `NuGet.config` を置く。
path は absolute path を推奨する。

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="exchangeapi-local" value="/absolute/path/to/ExchangeAPI/local/nuget" />
  </packageSources>
</configuration>
```

`<clear />` を入れる理由は、machine ごとの source 差分に依存しない restore に固定するためである。

## 3. 追加する Package

通常は `Composition` package を参照する。
`Composition` package は必要な `Native`、`Protocol`、`Vocabulary`、`Primitives` を dependency として引く。

bitFlyer を使う場合:

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer.Composition --version 0.1.0-local.1
```

Binance を使う場合:

```bash
dotnet add package ExchangeApi.Exchanges.Binance.Composition --version 0.1.0-local.1
```

より狭い依存だけ欲しい場合は、個別 package を直接参照してよい。

- `ExchangeApi.Primitives`
- `ExchangeApi.Exchanges.Bitflyer.Vocabulary`
- `ExchangeApi.Exchanges.Bitflyer.Protocol`
- `ExchangeApi.Exchanges.Bitflyer.Native`
- `ExchangeApi.Exchanges.Bitflyer.Composition`
- `ExchangeApi.Exchanges.Binance.Vocabulary`
- `ExchangeApi.Exchanges.Binance.Protocol`
- `ExchangeApi.Exchanges.Binance.Native`
- `ExchangeApi.Exchanges.Binance.Composition`

## 4. Restore と Build

consumer repo で次を実行する。

```bash
dotnet restore --configfile NuGet.config
dotnet build
```

## 5. Version 更新ルール

local feed へ再 pack するときは、同じ version を上書きするより version を増やすほうが安全である。

推奨:

```bash
bash scripts/pack-local-nuget.sh 0.1.0-local.2
```

その後、consumer repo 側でも package version を更新する。

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer.Composition --version 0.1.0-local.2
```

同じ version を再利用すると、consumer 側の global packages cache により古い package が使われ続けることがある。
同じ version を再利用する場合は restore 前に cache を削除する。

```bash
dotnet nuget locals global-packages --clear
```

## 6. Scope

- bitFlyer は現行 Stage10 の主対象であり、最も広い実装済み surface を持つ
- Binance は public `GetKlines` のみをサポートする
- `Unified` は未実装

API の使い方自体は repository root の `README.md` を参照する。
