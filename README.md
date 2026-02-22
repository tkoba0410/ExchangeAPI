# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための実装基盤です。
ドキュメントの参照起点は `docs/index.md` です。

## Quick Links

- Documentation index: `docs/index.md`
- Process: `docs/process/process.md`
- Contracts overview: `docs/normative/contracts/overview.md`
- Endpoints inventory: `docs/inventory/`

## 最初の1コール（Quickstart）

この Quickstart は、リポジトリを clone した開発者が手元で最短確認するための手順です。
利用導線の全体像は `docs/index.md` を参照してください。

### 1) Contracts（公開安定面 / 認証不要）

bitFlyer の Public Ticker を `Contracts`（安定）で叩く最短例です。

```bash
# repo root で実行
dotnet new console -n ExchangeApi.Quickstart
cd ExchangeApi.Quickstart
dotnet add reference ../src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj
```

再実行する場合は、既存の `ExchangeApi.Quickstart/` を削除するか別名を使用してください。

`Program.cs` を次で置き換え:

```csharp
using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Extensions;
using ExchangeApi.Exchanges.Bitflyer.Composition;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

var publicApi = BitflyerFactory.CreateContractPublicClient();
var call = await publicApi.GetTickerAsync(Symbol.ParseOrThrow("BTC_JPY"));
switch (call.Result)
{
    case CallResult<TickerResponse>.Ok ok:
        Console.WriteLine($"{ok.Response.Symbol} last={ok.Response.LastTradedPrice} at={ok.Response.Timestamp:O}");
        break;
    case CallResult<TickerResponse>.Err err:
        Console.WriteLine($"error kind={err.Error.Kind} status={err.Error.HttpStatus} message={err.Error.Message}");
        break;
}
```

実行:

```bash
dotnet run
```

期待結果: `BTC_JPY last=...` のような出力が1行出る（ネットワーク要）。

### 2) Normalized（取引所別 / 追従前提 / クライアント初期化時に資格情報が必要）

`Normalized` は「取引所別の機能網羅」を優先する利用面です（互換保証外・追従前提）。
この例の Ticker は public endpoint ですが、`CreateClient` は利用面を統一するため資格情報（または `RequestSigner`）を必須にしています。公開 endpoint だけを最短利用する場合は `CreateContractPublicClient` を使用してください。
資格情報の安全な運用は `docs/process/templates/README.md` を参照してください。

前提:

- `age` コマンドが利用できる
- `CREDENTIAL_FILE_PATH`（暗号化済み資格情報JSON）と `AGE_SECRET_KEY_PATH`（秘密鍵）のパスが指定されている

`Program.cs` 例（bitFlyer / Normalized Ticker）:

```csharp
using System;
using ExchangeApi.Composition.Providers.Credentials;
using ExchangeApi.Exchanges.Bitflyer.Composition;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

var encryptedPath = Environment.GetEnvironmentVariable("CREDENTIAL_FILE_PATH")
    ?? throw new InvalidOperationException("CREDENTIAL_FILE_PATH is required.");
var secretKeyPath = Environment.GetEnvironmentVariable("AGE_SECRET_KEY_PATH")
    ?? throw new InvalidOperationException("AGE_SECRET_KEY_PATH is required.");

var credentialProvider = new AgeEncryptedFileApiCredentialProvider(
    encryptedFilePath: encryptedPath,
    exchangeId: "bitflyer",
    secretKeyPath: secretKeyPath);

var api = BitflyerFactory.CreateClient(new BitflyerFactoryOptions
{
    CredentialProvider = credentialProvider,
    AccountId = "default",
});

var call = await api.GetTickerCallAsync(ProductCode.ParseOrThrowNormalized("BTC_JPY"));
switch (call.Result)
{
    case CallResult<GetTickerResponse>.Ok ok:
        Console.WriteLine($"{ok.Response.ProductCode} last={ok.Response.LastTradedPrice} at={ok.Response.Timestamp:O}");
        break;
    case CallResult<GetTickerResponse>.Err err:
        Console.WriteLine($"error kind={err.Error.Kind} status={err.Error.HttpStatus} message={err.Error.Message}");
        break;
}
```

実行:

```bash
dotnet run
```

期待結果: `BTC_JPY last=...` のような出力が1行出る（認証/ネットワーク要）。

## 安定保証の境界

- 公開安定面は Contracts 層のみです。詳細は `docs/normative/contracts/overview.md` と `docs/index.md` を参照してください。

## Contributions / Development

- 開発・文書更新の手順: `docs/process/process.md`
- 例外の記録先: `docs/process/exceptions.md`
