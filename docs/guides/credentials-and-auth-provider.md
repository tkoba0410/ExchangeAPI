# Credentials And Auth Provider Guide

位置づけ: 利用ガイド

本書は、`IApiCredentialProvider` / `IApiCredentialSession` の使い方と判断基準を説明する。  
契約の正本は [`docs/spec.md`](../spec.md) の Private Auth / Signing 契約である。

## 1. 基本方針

ExchangeAPI core は、API key / secret の保存方式や暗号化方式を正本に含めない。  
core が必要とするのは、private request のための `ApiKey` と署名結果であり、secret storage そのものではない。

そのため、v2 では次の形を基本とする。

```csharp
public interface IApiCredentialProvider
{
    ValueTask<IApiCredentialSession> OpenSessionAsync(
        CancellationToken cancellationToken = default);
}

public interface IApiCredentialSession : IAsyncDisposable
{
    string ApiKey { get; }

    string Sign(string payload);
}
```

重要な点:

- `ApiSecret` は公開 API に出さない
- provider は credential source を開き、必要なら復号し、session を返す
- session は session 寿命中だけ `ApiKey` と署名機能を提供する
- client は `ApiKey` と `Sign(payload)` だけを使う
- v2 の署名 API は `Sign(string payload)` のみを固定する
- `payload` は venue ごとの canonical signing payload を UTF-8 文字列として構築したものとする
- byte sequence を直接扱う overload は v2 では追加しない
- storage / encryption recipe は provider 実装に閉じる
- provider は venue-specific とし、1 provider instance は 1 venue の credential source だけを扱う

## 2. 通常利用

通常利用では、利用者は provider を client / adapter へ注入するだけでよい。  
client / adapter は private call の実行時に session を開き、必要な request に署名し、session を閉じる。

概念例:

```csharp
var credentials = new BitflyerPlainTextApiCredentialProvider(
    apiKey: "...",
    apiSecret: "...");

using var client = BitflyerClientFactory.CreateNativeClientBundle(
    new BitflyerClientOptions
    {
        ApiCredentialProvider = credentials,
    });

var result = await client.Private.GetBalanceAsync(cancellationToken);
```

この使い方では、利用者は `OpenSessionAsync()` を直接呼ばない。  
session 寿命は client / adapter が operation 単位で管理する。

## 3. 明示 Session 利用

`age` 復号、OS keychain、外部 secret manager など、高コストな credential provider では、複数 private call の間だけ session を再利用したい場合がある。

その場合は、明示的に session を開いて寿命を限定する。

```csharp
await using var session = await credentials.OpenSessionAsync(cancellationToken);

var balance = await client.Private.GetBalanceAsync(
    new GetBalanceRequest(),
    session,
    cancellationToken);

var collateral = await client.Private.GetCollateralAsync(
    new GetCollateralRequest(),
    session,
    cancellationToken);
```

明示 session の基本:

- operation batch 単位で開く
- 必要な private call が終わったら閉じる
- client bundle 寿命まで無条件に延ばさない
- provider 内部の無制限 cache を正本にしない

明示 session overload の形:

```csharp
Task<CallResult<TRequest, TResponse>> EndpointAsync(
    TRequest request,
    IApiCredentialSession credentialSession,
    CancellationToken cancellationToken = default);
```

この overload は private endpoint にだけ用意する。  
渡された `credentialSession` の dispose は caller の責務であり、client / adapter は dispose しない。

## 4. PlainText Provider

平文 provider は、sample / test / local dev 用として用意してよい。

```csharp
public sealed class BitflyerPlainTextApiCredentialProvider : IApiCredentialProvider
public sealed class BinancePlainTextApiCredentialProvider : IApiCredentialProvider
```

位置づけ:

- sample 用
- test 用
- local dev 用
- production 推奨ではない
- secret storage / encryption 機能ではない

平文 provider を用意する理由は、API の使い方を最小構成で説明するためである。  
安全な保存方式を提供するためではない。

配置:

- project: `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj`
- package: `ExchangeApi.Optional.Credentials`
- namespace root: `ExchangeApi.Optional.Credentials`
- dependencies:
  - `ExchangeApi.Primitives`
  - venue `Composition` project には依存しない
  - venue `Protocol` / `Native` project には依存しない
- core auth contract が `Primitives` 以外に置かれる場合、optional package はその contract project のみに依存する

## 5. 外部 Provider

本番や長期運用では、用途に応じて provider を差し替える。

候補:

- environment variable
- `age` encrypted file
- OS keychain
- external secret manager
- process-local secure source

これらは ExchangeAPI core の正本ではない。  
それぞれの復号、取得、cache、監査、権限管理は provider 実装または上層 application の責務である。

v2 初手で同梱する optional provider:

```csharp
public sealed class BitflyerAgeFileApiCredentialProvider : IApiCredentialProvider
public sealed class BinanceAgeFileApiCredentialProvider : IApiCredentialProvider
```

`age` provider は、復号処理そのものを `IAgeCredentialFileDecryptor` に委譲する。  
標準実装は external `age` CLI を呼び出す `AgeCliCredentialFileDecryptor` とする。
CLI / MCP / live test で使う標準運用 recipe は、環境変数ではなく credential profile から `AgeFile` provider を作る。

factory:

```csharp
public static class PlainTextApiCredentialProviderFactory
public static class AgeFileApiCredentialProviderFactory
public static class CredentialProfileProviderFactory
```

factory は `ExchangeVenue` を受け取り、対応する venue-specific provider を返す。  
`ExchangeVenue` は `ExchangeApi.Optional.Credentials` に置く optional package 内 vocabulary とし、core library の共通 enum にはしない。

public type set:

```csharp
public enum ExchangeVenue
public interface IAgeCredentialFileDecryptor
public sealed class AgeCliCredentialFileDecryptor : IAgeCredentialFileDecryptor
public sealed class BitflyerPlainTextApiCredentialProvider : IApiCredentialProvider
public sealed class BinancePlainTextApiCredentialProvider : IApiCredentialProvider
public sealed class BitflyerAgeFileApiCredentialProvider : IApiCredentialProvider
public sealed class BinanceAgeFileApiCredentialProvider : IApiCredentialProvider
public static class PlainTextApiCredentialProviderFactory
public static class AgeFileApiCredentialProviderFactory
public sealed class CredentialProfile
public sealed class CredentialProfileEntry
public static class CredentialProfileDefaults
public static class CredentialProfileLoader
public static class CredentialProfileProviderFactory
```

## 6. Credential Profile

credential profile は、どの venue の credentials をどの provider から読むかを示す local-only 設定である。
API key / secret 本体は profile に置かない。

標準配置:

- `local/credentials/credential-profile.json`
- `local/credentials/current/age-identity.txt`
- `local/credentials/current/bitflyer.age`
- `local/credentials/current/binance.age`

canonical profile:

```json
{
  "version": 1,
  "credentials": {
    "bitflyer": {
      "provider": "age-file",
      "identityFilePath": "current/age-identity.txt",
      "credentialsFilePath": "current/bitflyer.age"
    }
  }
}
```

`identityFilePath` と `credentialsFilePath` は profile file からの相対 path または absolute path とする。
`current/` 配下は実ファイルではなく symlink として運用してよい。

CTradeBot 互換の flat settings も読み取れる:

```json
{
  "credentialsSource": "AgeFile",
  "credentialsAgeFile": "./bitflyer-credentials.age",
  "ageIdentityFile": "./age-identity.txt"
}
```

この互換 shape は CTradeBot から ExchangeAPI optional credentials へ逆追随しやすくするための bridge であり、ExchangeAPI 側の canonical profile は `credentials` object を持つ形式とする。

## 7. Age Credential File 作成支援

`scripts/create-age-credential-file.sh` は、ExchangeAPI が読む age 暗号化済み credentials file を作成するための local setup helper である。
credential 管理機構、secret manager、runtime provider ではない。

基本例:

```bash
bash scripts/create-age-credential-file.sh --venue bitflyer
```

既定値:

- identity: `~/.config/exchangeapi/keys/age.key`
- encrypted credentials: `local/credentials/current/<venue>.age`
- identity symlink: `local/credentials/current/age-identity.txt`
- credential profile: `local/credentials/credential-profile.json`

script が行うこと:

- `age` / `age-keygen` / `python3` の存在確認
- age identity file が無い場合の新規作成確認
- 既存 identity から `age-keygen -y` で recipient を取得
- API key / API secret の非表示対話入力
- canonical credentials JSON をメモリ上で作成
- 平文 file を作らず `age` へ pipe して暗号化
- encrypted credentials file、identity symlink、credential profile の作成または更新

script が行わないこと:

- API key / API secret を command line 引数で受け取る
- API key / API secret を画面に表示する
- 平文 credentials JSON file を保存する
- 取引所 API に接続する
- API key の有効性確認を行う
- secret manager / keychain / 外部サービスに接続する
- credential lifecycle を管理する

script は起動時、API key / secret 入力直前、完了時に日本語で説明を表示する。
完了時には、生成・更新した file と、それぞれに含まれる情報、注意点を表示する。

## 8. Credential JSON Schema

`AgeFile` provider が復号後に受け取る JSON は次の flat object とする。

```json
{
  "version": 1,
  "venue": "bitflyer",
  "apiKey": "xxxxx",
  "apiSecret": "yyyyy",
  "label": "main-trade",
  "generatedAt": "2026-03-29T10:00:00+09:00",
  "expiresAt": "2026-06-30T00:00:00+09:00",
  "note": "main trading key"
}
```

required fields:

- `version`
- `venue`
- `apiKey`
- `apiSecret`

optional metadata:

- `label`
- `generatedAt`
- `expiresAt`
- `note`

validation:

- `version` は integer `1` のみを受け付ける
- `venue` は provider の venue と一致しなければならない
- canonical venue string は `bitflyer` / `binance` とする
- `apiKey` / `apiSecret` は empty、whitespace-only、前後 whitespace を invalid とする
- unknown field は許容し、v2 実装では無視してよい
- `generatedAt` / `expiresAt` は `DateTimeOffset` として parse 可能な文字列を推奨する
- `expiresAt` は v2 では metadata であり、期限 enforcement は行わない
- `label` は operator 向け metadata であり、routing、account selection、session identity に使わない

## 9. 失敗通知

credential provider は、credential を開けない場合に失敗理由を分類可能にする。

基本形:

```csharp
public sealed class ApiCredentialException : Exception
{
    public ApiCredentialErrorKind Kind { get; }
}
```

分類:

- `NotConfigured`
- `SourceUnavailable`
- `DecryptFailed`
- `JsonParseFailed`
- `MissingRequiredField`
- `UnsupportedVersion`
- `VenueMismatch`
- `InvalidApiKey`
- `InvalidApiSecret`

分類境界:

- `NotConfigured`: provider に必要な設定値が渡されていない
- `SourceUnavailable`: 設定値はあるが、credential source に到達できない
- `DecryptFailed`: credential source は読めたが復号に失敗した
- `JsonParseFailed`: 復号後 payload を credentials JSON として parse できない
- `MissingRequiredField`: `version`、`venue`、`apiKey`、`apiSecret` のいずれかが無い
- `UnsupportedVersion`: `version` が v2 実装の対応範囲外
- `VenueMismatch`: credentials JSON の `venue` が provider の venue と一致しない
- `InvalidApiKey`: `apiKey` が empty、whitespace-only、または前後 whitespace を含む
- `InvalidApiSecret`: `apiSecret` が empty、whitespace-only、または前後 whitespace を含む

通知責務:

- provider は secret-safe な exception を投げる
- library core は通知手段を持たない
- CLI は stderr と exit code へ写像する
- MCP は tool 公開制御、structured error、stderr diagnostic へ写像する
- application は UI、operator log、alert など用途に応じて写像する

adapter が公開してよい detail key:

- `credentialErrorKind`
- `venue`
- `provider`
- `reason`
- `requiredCredentialProfile`

`reason` は secret-safe な短文に限定する。file path は通常 summary に含めず、adapter の verbose / diagnostic 出力に限定する。

## 10. 禁止事項

- `ApiSecret` を public API に出さない
- API key / secret を log に出さない
- API key / secret を exception message に含めない
- API key / secret を `CallResult`, `CallMeta`, `CallError` に含めない
- API key / secret を `local/evidence/` に残さない
- `BitflyerPlainTextApiCredentialProvider` / `BinancePlainTextApiCredentialProvider` を production 推奨として扱わない
- storage / encryption 方式を ExchangeAPI core の必須正本にしない

## 11. 実装固定事項

v2 実装では次を固定する。

- `Sign(string payload)` のみを public signing API とする
- byte sequence overload は post-v2 検討とする
- provider は venue-specific class とする
- `PlainText` / `AgeFile` provider は `ExchangeApi.Optional.Credentials` に置く
- `ExchangeVenue` は optional package 内 vocabulary とする
- 通常 private call は client / adapter が session を内部で開閉する
- 明示 session overload は、複数 private call を同一 session で実行したい利用者向けに public surface へ出す
