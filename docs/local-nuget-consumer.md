# Local NuGet Consumer Guide

この文書は、別の .NET project から ExchangeAPI の local NuGet feed を使う手順を定義する。

外部 consumer 向けの推奨導線は local NuGet feed とする。`ProjectReference` は repo 内開発または近接開発向けであり、外部 consumer の推奨導線ではない。

注記:

- 現在の作業固定点は `v3.4.0` である
- 本書の package version と API 例は v3 系の local consumer 導線を示す
- release 前確認では、`3.4.0-local.*` のような local package version を使ってよい

## 1. 前提

- ExchangeAPI repository 側で local package を生成済みであること
- consumer project 側が `net10.0` を target できること
- local feed は machine-local 用途であり、共有 feed や公開 feed の代替ではない

ExchangeAPI repository 側では repo root で次を実行する。

```bash
bash scripts/pack-local-nuget.sh 3.4.0-local.check
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

通常は venue aggregate package を参照する。
venue aggregate package は当該 venue の `Native`、`Protocol`、`Vocabulary`、`Composition` surface を含む。

bitFlyer を使う場合:

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer --version 3.4.0-local.check
```

Binance を使う場合:

```bash
dotnet add package ExchangeApi.Exchanges.Binance --version 3.4.0-local.check
```

v3.0.0 では、venue layer-specific package は外部 consumer 向け publish 対象にしない。
venue layer-specific project も廃止する。
layer-specific namespace は aggregate package 内に残るが、package reference と repo 内 project は venue 単位にする。

維持する個別 package:

- `ExchangeApi.Primitives`

credential provider 実装が必要な場合は optional package を追加する。

```bash
dotnet add package ExchangeApi.Optional.Credentials --version 3.4.0-local.check
```

`ExchangeApi.Optional.Credentials` は、core library の必須依存ではない。  
平文 provider、age-backed provider などの storage / decrypt recipe が必要な consumer だけが参照する。

secret-safe logging / evidence helper が必要な場合は optional logging package を追加する。

```bash
dotnet add package ExchangeApi.Optional.Logging --version 3.4.0-local.check
```

`ExchangeApi.Optional.Logging` は、core library の必須依存ではない。
JSONL writer、redaction helper、evidence directory helper が必要な consumer だけが参照する。

## 4. 最小利用例

以下は v3 系の package 導線と既存 API 名を使う例である。

consumer app の `Program.cs`:

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

using var client = BitflyerClientFactory.CreateNativeClientBundle();

var call = await client.Public.GetTickerAsync(new GetTickerRequest
{
    ProductCode = ProductCodes.BtcJpy,
});

if (call.IsSuccess && call.Response is not null)
{
    Console.WriteLine($"{call.Response.ProductCode} ltp={call.Response.Ltp}");
}
else
{
    Console.WriteLine($"error kind={call.Error?.Kind} message={call.Error?.Message}");
}
```

## 5. Restore と Build

consumer repo で次を実行する。

```bash
dotnet restore --configfile NuGet.config
dotnet build
```

ExchangeAPI repo 側で local feed と v3 API surface の consumer smoke を確認する場合は、次を実行する。

```bash
bash scripts/smoke-local-nuget-consumer.sh 3.4.0-local.check
```

この smoke は一時 consumer project を作成し、`ExchangeApi.Exchanges.Bitflyer`、`ExchangeApi.Optional.Credentials`、`ExchangeApi.Optional.Logging`、`ExchangeApi.Optional.Testing` を local feed から restore して build / run する。
実 API には接続しない。
`BitflyerClientFactory`、`BitflyerRealtimeClientFactory`、`BitflyerRealtimeChannels`、`BitflyerRealtimeStreamEvent<T>`、`BitflyerRealtimeClientOptions`、`PlainTextApiCredentialProviderFactory`、`Redactor`、`RealtimeReplayFrame`、`BitflyerRealtimeReplayRunner` を参照できること、secret value が `[REDACTED]` になること、smoke output が secret-free であることを確認する。

## 6. Version 更新ルール

local feed へ再 pack するときは、同じ version を上書きするより version を増やすほうが安全である。

推奨:

```bash
bash scripts/pack-local-nuget.sh 3.0.0-local.1
```

その後、consumer repo 側でも package version を更新する。consumer repo は floating version ではなく、明示 version を固定する。

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer --version 3.0.0-local.1
```

同じ version を再利用すると、consumer 側の global packages cache により古い package が使われ続けることがある。
同じ version を再利用する場合は restore 前に cache を削除する。

```bash
dotnet nuget locals global-packages --clear
```

## 7. Scope

- bitFlyer は現行 library slice の主対象であり、最も広い実装済み surface を持つ
- bitFlyer Realtime API は public / private read MVP と stream envelope / reconnect foundation までを含む
- Binance は public `GetKlines` のみをサポートする
- `Unified` は未実装

API の使い方自体は repository root の `README.md` を参照する。
