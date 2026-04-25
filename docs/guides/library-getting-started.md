# Library Getting Started

## 1. 目的

ExchangeAPI Library を別の .NET project から参照し、最小コードで 1 回成功させる。

## 2. 前提

- `dotnet` SDK `10.0`
- ExchangeAPI repo を checkout 済み
- local NuGet feed を使うため、ExchangeAPI repo 側で package を生成できる

## 3. 導入

ExchangeAPI repo root で local package を生成する。

```bash
bash scripts/pack-local-nuget.sh 0.1.0-local.1
```

consumer repo 側では `NuGet.config` に local feed を追加する。

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

その後、consumer project へ package を追加する。

```bash
dotnet add package ExchangeApi.Exchanges.Bitflyer.Composition --version 0.1.0-local.1
```

詳細な consumer 手順は [`../local-nuget-consumer.md`](../local-nuget-consumer.md) を参照する。

## 4. 最小例

`Program.cs`:

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
    Console.WriteLine($"{call.Response.ProductCode} ltp={call.Response.Ltp} at={call.Response.Timestamp:O}");
}
else
{
    Console.WriteLine($"error kind={call.Error?.Kind} message={call.Error?.Message}");
}
```

実行:

```bash
dotnet restore --configfile NuGet.config
dotnet run
```

## 5. 動作確認

成功時は次のような 1 行が出る。

```text
BTC_JPY ltp=... at=2026-03-31T...
```

追加の API contract や対象 endpoint は [`../spec.md`](../spec.md) と [`../endpoints-bitflyer.md`](../endpoints-bitflyer.md) を参照する。
