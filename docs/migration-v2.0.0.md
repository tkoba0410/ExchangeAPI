# ExchangeAPI v2.0.0 Migration Guide

最終更新: 2026-04-25  
位置づけ: version 単位文書  
状態: draft

## 1. 目的

本書は、`v1.0.0` 利用者が `v2.0.0` へ移行するための差分と対応手順を示す。  
breaking change の設計理由そのものは ledger 側に置き、本書では利用者観点の移行に集中する。

## 2. 対象読者

- library 利用者
- CLI 利用者
- MCP Server 利用者

## 3. 進め方

1. 使用中の surface を確認する
2. 対応する breaking change を ledger で確認する
3. rename / remove / contract tighten をコードへ反映する
4. 関連 guide と examples を差し替える
5. test と動作確認を行う

### 3.1 実装順の目安

repo 内で `v2.0.0` 実装を進める場合は、次の順を推奨する。

1. library public surface rename
2. `CallError` additive detail field 追加
3. auth provider / optional credentials 実装
4. CLI / MCP adapter 追従
5. human-facing timestamp display の更新
6. migration lock test の更新
7. 正本文書と release note の最終反映

## 4. 変更カテゴリ

### 4.1 Library

- `Call<TRequest, TResponse>` は `CallResult<TRequest, TResponse>` へ rename される
- facade public method は `*CallAsync(...)` から `*Async(...)` へ rename される
- `CallError` / `CallMeta` は維持する
- client 生成 method は `Create*Client(...)` から `Create*ClientBundle(...)` へ rename される
- `BitflyerClientFactory` / `BinanceClientFactory` の class 名は維持する
- endpoint `*Request` と `*ClientOptions` は維持する
- `CallError.Kind` taxonomy は維持する
- `CallError` に `HttpStatusCode`, `VenueErrorCode`, `VenueErrorMessage` を追加する
- CLI と MCP は追加 field を verbose/details 側で扱う
- raw error body の正本は引き続き `ProtocolResponse.BodyText` とする
- 数量系 scalar の基本型 `decimal`、timestamp scalar の基本型 `DateTimeOffset`、invariant culture parse は維持する
- Native DTO の `required / nullable` は version-wide な一括 tighten を行わず、endpoint ごとの exact contract に従って個別に是正する
- enum と string vocabulary の境界は維持し、closed-set は venue-local enum、open-set は string のままとする
- unit / contract test taxonomy と source/test project layout の大再編は行わない
- 採用した breaking change を固定する surface lock test は追加または更新する
- CLI と MCP の surface 自体は 1:1 へ統一しない
- CLI / MCP の error detail、timestamp、decimal string、venue / symbol などの基底語彙は揃える
- 内部 timestamp 契約は維持しつつ、human-facing な時刻表示は local with offset 優先へ寄せる
- private credentials の storage / encryption 方式は core 正本から外し、auth provider 契約へ責務を寄せる
- auth provider の具体 shape は `IApiCredentialProvider.OpenSessionAsync(...)` 型を採用する
- 通常利用では client 側が session を隠して扱い、明示管理が必要な場合だけ利用者が session を開く
- `ApiSecret` は public API に出さず、`IApiCredentialSession.Sign(string payload)` で署名する
- `Sign(string payload)` のみを v2 public signing API とし、byte sequence overload は持たない
- `PlainText` / `AgeFile` provider は `ExchangeApi.Optional.Credentials` package へ移る
- `AgeFile` provider の復号後 JSON は `version`, `venue`, `apiKey`, `apiSecret` を required とする
- credential failure は `ApiCredentialException.Kind` で分類し、CLI / MCP が通知へ写像する
- MCP では副作用系を除く read-only 情報を原則サポート対象にし、tool は `Core Bot Tools` と `Inspection Read Tools` に分ける
- verification は API 契約分類とは別に、`repo/local` 配置と `safe/tolerable/dangerous` の運用分類で整理する
- 実行結果、artifact、log、手動確認メモは `local/evidence/` に集約する
- distribution は library / optional package を NuGet、CLI / MCP を executable artifact として扱う

### 4.2 CLI

- facade naming change に伴い、内部実装と command 実行経路は `CallResult` / `*Async(...)` / `Create*ClientBundle(...)` を前提に更新される
- command identity 自体は endpoint `EndpointId` の kebab-case を維持する
- 既定 summary の shape は維持する
- `--verbose` 時は `CallError.Kind` に加えて `HttpStatusCode`, `VenueErrorCode`, `VenueErrorMessage` を出せるようにする
- `protocol` の raw body source は引き続き `Response.BodyText` とする
- CLI surface は MCP tool 名へ寄せず、endpoint inspection / execution 導線としての役割を維持する
- shared vocabulary として、error detail、timestamp、numeric representation、venue / symbol の意味を MCP と揃える
- runtime registry と adapter test を更新し、v2 rename と verbose detail を固定する
- CLI の human-facing な時刻表示は local with offset を優先してよい
- CLI canonical は API key / secret の直接引数入力を引き続き許可しない
- credential failure は stderr と exit code `2` へ写像する
- credential failure の verbose detail key は `credentialErrorKind`, `venue`, `provider`, `reason` とする

### 4.3 MCP Server

- tool surface 自体は責務単位を維持し、library endpoint や CLI command との 1:1 mirror には寄せない
- `upstream_error.details` は既存の `callErrorKind`, `callErrorMessage` に加えて `callHttpStatusCode`, `callVenueErrorCode`, `callVenueErrorMessage` を optional に持てるようにする
- decimal string と UTC timestamp の方針は維持する
- `ProtocolResponse.BodyText` の raw body をそのまま tool response 契約へ持ち込まず、必要な detail だけを露出する
- visible tool inventory は tool catalog を正本としつつ、shared vocabulary は CLI と整合させる
- adapter test と live test は維持し、tool schema と upstream error detail shape を固定する
- MCP の structured response は UTC / structured contract を維持してよい
- credential failure は private tool 非公開、または `upstream_error` / `account_unavailable` として表現する
- credential failure details は `credentialErrorKind`, `venue`, `provider`, `reason` を持つ

### 4.4 Optional Credentials

v2 では credential storage / decrypt recipe を core library から外し、optional package に置く。

追加 package:

```bash
dotnet add package ExchangeApi.Optional.Credentials --version 2.0.0
```

主な public type:

- `ExchangeVenue`
- `IAgeCredentialFileDecryptor`
- `AgeCliCredentialFileDecryptor`
- `BitflyerPlainTextApiCredentialProvider`
- `BinancePlainTextApiCredentialProvider`
- `BitflyerAgeFileApiCredentialProvider`
- `BinanceAgeFileApiCredentialProvider`
- `PlainTextApiCredentialProviderFactory`
- `AgeFileApiCredentialProviderFactory`

v1 系で `BitflyerApiCredentials` を直接渡していた利用者は、v2 では provider を渡す。

v1 例:

```csharp
using var client = BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions
{
    Credentials = new BitflyerApiCredentials
    {
        ApiKey = "...",
        ApiSecret = "...",
    },
});
```

v2 例:

```csharp
var credentials = new BitflyerPlainTextApiCredentialProvider(
    apiKey: "...",
    apiSecret: "...");

using var client = BitflyerClientFactory.CreateNativeClientBundle(
    new BitflyerClientOptions
    {
        ApiCredentialProvider = credentials,
    });
```

高コスト provider を複数 private call で再利用したい場合は、明示 session を使う。

```csharp
await using var session = await credentials.OpenSessionAsync(cancellationToken);

var balance = await client.Private.GetBalanceAsync(
    new GetBalanceRequest(),
    session,
    cancellationToken);
```

明示 session overload は private endpoint にだけ追加され、引数順は `request`, `credentialSession`, `cancellationToken` とする。

### 4.5 Distribution

- library package と optional package は NuGet package として配布する
- `ExchangeApi.Optional.Credentials` は v2 publish 対象に含める
- CLI と MCP Server は NuGet package ではなく executable artifact として配布する
- 生成物は `local/nuget/` と `local/publish/**` に置き、git 管理しない

## 4.6 先に確認すべき影響

- library 利用者は型名と method 名の rename 影響が最も大きい
- private endpoint 利用者は credentials 直渡しから auth provider 注入への変更を確認する
- CLI 利用者は command 名よりも verbose/error 表示の detail 追加と credential failure 通知を確認すればよい
- MCP 利用者は tool 名よりも `upstream_error.details`、credential failure details、shared vocabulary の更新を確認すればよい

## 5. 変更一覧

| 項目 | 旧 | 新 | 対応 |
| --- | --- | --- | --- |
| result container type | `Call<GetTickerRequest, GetTickerResponse>` | `CallResult<GetTickerRequest, GetTickerResponse>` | 型名を置換する |
| facade public method naming | `GetTickerCallAsync(...)` | `GetTickerAsync(...)` | method 名を置換する |
| error / meta type naming | `CallError`, `CallMeta` | `CallError`, `CallMeta` | 変更なし |
| client factory method naming | `CreateNativeClient(...)` | `CreateNativeClientBundle(...)` | method 名を置換する |
| client factory method naming | `CreateProtocolClient(...)` | `CreateProtocolClientBundle(...)` | method 名を置換する |
| client factory class naming | `BitflyerClientFactory`, `BinanceClientFactory` | `BitflyerClientFactory`, `BinanceClientFactory` | 変更なし |
| endpoint request naming | `GetTickerRequest`, `SendChildOrderRequest` | `GetTickerRequest`, `SendChildOrderRequest` | 変更なし |
| client options naming | `BitflyerClientOptions`, `BinanceClientOptions` | `BitflyerClientOptions`, `BinanceClientOptions` | 変更なし |
| `CallError.Kind` taxonomy | `Transport / Http / Codec / Semantic / Mapping` | `Transport / Http / Codec / Semantic / Mapping` | 変更なし |
| `CallError` detail fields | `Kind`, `Message` | `Kind`, `Message`, `HttpStatusCode?`, `VenueErrorCode?`, `VenueErrorMessage?` | additive field を利用する |
| CLI verbose error detail | `CallError.Kind` と endpoint 情報 | 既存項目に加えて `HttpStatusCode?`, `VenueErrorCode?`, `VenueErrorMessage?` | verbose 出力を拡張する |
| MCP upstream error details | `callErrorKind`, `callErrorMessage` | 既存項目に加えて `callHttpStatusCode?`, `callVenueErrorCode?`, `callVenueErrorMessage?` | details key を拡張する |
| raw error body source | `ProtocolResponse.BodyText` | `ProtocolResponse.BodyText` | raw の正本は維持する |
| scalar base contract | `decimal`, `DateTimeOffset`, invariant parse | `decimal`, `DateTimeOffset`, invariant parse | 変更なし |
| Native DTO nullability policy | endpoint ごとの exact contract で判断 | endpoint ごとの exact contract で判断 | blanket tighten は行わない |
| enum / string vocabulary boundary | closed-set は venue-local enum、open-set は string | closed-set は venue-local enum、open-set は string | 変更なし |
| unit / contract test taxonomy and source/test project layout | `Architecture / Protocol / Native / Composition / Live / Adapter` | `Architecture / Protocol / Native / Composition / Live / Adapter` | production source と deterministic test の大枠は変更しない |
| v2 migration lock tests | endpoint / adapter ごとの既存 test | 既存 test に surface lock を追加または更新 | rename と additive field を固定する |
| CLI / MCP surface policy | adapter ごとに最適化された surface | adapter ごとに最適化された surface | 1:1 統一は行わない |
| CLI / MCP shared vocabulary | 部分的に整合 | error detail、timestamp、decimal string、venue / symbol を揃える | adapter ごとの出力 key / verbose 表示を更新する |
| human-facing timestamp display | UTC 基本 | local with offset 優先 | machine canonical は維持し、CLI / log 表示を更新する |
| private credentials policy | `age` 前提の色が強い | auth provider 契約を core に置き、storage/encryption recipe は外に出す | CLI 直引数は許可しない |
| auth provider shape | 暗黙または未固定 | `IApiCredentialProvider.OpenSessionAsync(...)` 型 | 通常利用では自動 session、必要時だけ明示 session を使う |
| credential implementation package | core / adapter に混在 | `ExchangeApi.Optional.Credentials` | provider 実装が必要な consumer は optional package を追加する |
| private credential injection | credentials object 直渡し | `IApiCredentialProvider` 注入 | `BitflyerPlainTextApiCredentialProvider` などへ置換する |
| explicit credential session | なし、または未固定 | private endpoint の `EndpointAsync(request, credentialSession, cancellationToken)` | 高コスト provider で複数 private call をまとめる場合に使う |
| credential failure notification | string message 中心 | `ApiCredentialException.Kind` を adapter が通知へ写像 | CLI は stderr/exit code 2、MCP は account_unavailable details |
| MCP read-only surface policy | bot 向け最小 tool にかなり限定 | 副作用系を除く read-only 情報を原則サポートし、`Core Bot Tools` と `Inspection Read Tools` に分ける | 既存 aggregate tool を維持しつつ、inspection 用 read-only tool を追加する |
| verification layout and risk model | `tests` / `LiveTests` と marker 中心 | test 本体を `tests` / `verification` に置き、evidence を `local/evidence` に集約し、`safe/tolerable/dangerous` を導入する | API 契約分類は維持しつつ、manual/live verification と evidence 管理を分離する |
| distribution artifact shape | library package と executable が混在気味 | library / optional は NuGet、CLI / MCP は executable artifact | optional package を publish 対象に加え、CLI/MCP は release asset として扱う |

## 6. 関連文書

- [`docs/breaking-changes-v2.0.0.md`](./breaking-changes-v2.0.0.md)
- [`docs/spec.md`](./spec.md)
- [`docs/cli.md`](./cli.md)
- [`docs/mcp-server.md`](./mcp-server.md)
- [`docs/verification.md`](./verification.md)
- [`docs/guides/credentials-and-auth-provider.md`](./guides/credentials-and-auth-provider.md)
- [`docs/distribution.md`](./distribution.md)
