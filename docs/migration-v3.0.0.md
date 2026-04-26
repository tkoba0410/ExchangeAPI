# ExchangeAPI v3.0.0 Migration Guide

最終更新: 2026-04-26
位置づけ: v3.0.0 migration guide

## 1. Package Reference

bitFlyer consumer は package reference を変更する。

Before:

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer.Composition --version 2.2.0
```

After:

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer --version 3.0.0
```

Binance consumer は package reference を変更する。

Before:

```bash
dotnet add package ExchangeApi.Exchanges.Binance.Composition --version 2.2.0
```

After:

```bash
dotnet add package ExchangeApi.Exchanges.Binance --version 3.0.0
```

## 2. Source Code

v3.0.0 の project/package consolidation では namespace を維持する。

既存の using はそのまま使える。

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
```

## 3. Optional Packages

optional packages は v3.0.0 でも維持する。

- `ExchangeApi.Optional.Credentials`
- `ExchangeApi.Optional.Logging`

credentials や logging を使う consumer は、必要に応じて引き続き明示参照する。

## 4. Repository Contributors

repo 内では、venue layer project は削除される。

Before:

```text
src/Exchanges/Bitflyer/Vocabulary/ExchangeApi.Exchanges.Bitflyer.Vocabulary.csproj
src/Exchanges/Bitflyer/Protocol/ExchangeApi.Exchanges.Bitflyer.Protocol.csproj
src/Exchanges/Bitflyer/Native/ExchangeApi.Exchanges.Bitflyer.Native.csproj
src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj
```

After:

```text
src/Exchanges/Bitflyer/ExchangeApi.Exchanges.Bitflyer.csproj
```

`Protocol` / `Native` / `Composition` / `Vocabulary` folder は維持される。
test project の `Protocol.Tests` / `Native.Tests` / `Composition.Tests` は、package/project 境界ではなく設計境界の test taxonomy として扱う。
