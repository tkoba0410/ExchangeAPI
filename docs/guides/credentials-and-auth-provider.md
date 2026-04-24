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
- storage / encryption recipe は provider 実装に閉じる

## 2. 通常利用

通常利用では、利用者は provider を client / adapter へ注入するだけでよい。  
client / adapter は private call の実行時に session を開き、必要な request に署名し、session を閉じる。

概念例:

```csharp
var credentials = new PlainTextApiCredentialProvider(
    apiKey: "...",
    apiSecret: "...");

var client = BitflyerClientFactory.CreateNativeClientBundle(
    options with { ApiCredentialProvider = credentials });

var result = await client.Private.GetBalanceAsync(cancellationToken);
```

この使い方では、利用者は `OpenSessionAsync()` を直接呼ばない。  
session 寿命は client / adapter が operation 単位で管理する。

## 3. 明示 Session 利用

`age` 復号、OS keychain、外部 secret manager など、高コストな credential provider では、複数 private call の間だけ session を再利用したい場合がある。

その場合は、明示的に session を開いて寿命を限定する。

```csharp
await using var session = await credentials.OpenSessionAsync(cancellationToken);

var balance = await client.Private.GetBalanceAsync(session, cancellationToken);
var collateral = await client.Private.GetCollateralAsync(session, cancellationToken);
```

明示 session の基本:

- operation batch 単位で開く
- 必要な private call が終わったら閉じる
- client bundle 寿命まで無条件に延ばさない
- provider 内部の無制限 cache を正本にしない

## 4. PlainText Provider

平文 provider は、sample / test / local dev 用として用意してよい。

```csharp
public sealed class PlainTextApiCredentialProvider : IApiCredentialProvider
```

位置づけ:

- sample 用
- test 用
- local dev 用
- production 推奨ではない
- secret storage / encryption 機能ではない

平文 provider を用意する理由は、API の使い方を最小構成で説明するためである。  
安全な保存方式を提供するためではない。

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

## 6. 失敗通知

credential provider は、credential を開けない場合に失敗理由を分類可能にする。

基本形:

```csharp
public sealed class ApiCredentialException : Exception
{
    public ApiCredentialErrorKind Kind { get; }
}
```

分類例:

- `NotConfigured`
- `SourceUnavailable`
- `DecryptFailed`
- `JsonParseFailed`
- `MissingRequiredField`
- `UnsupportedVersion`
- `VenueMismatch`
- `InvalidApiKey`
- `InvalidApiSecret`

通知責務:

- provider は secret-safe な exception を投げる
- library core は通知手段を持たない
- CLI は stderr と exit code へ写像する
- MCP は tool 公開制御、structured error、stderr diagnostic へ写像する
- application は UI、operator log、alert など用途に応じて写像する

## 7. 禁止事項

- `ApiSecret` を public API に出さない
- API key / secret を log に出さない
- API key / secret を exception message に含めない
- API key / secret を `CallResult`, `CallMeta`, `CallError` に含めない
- API key / secret を `local/evidence/` に残さない
- `PlainTextApiCredentialProvider` を production 推奨として扱わない
- storage / encryption 方式を ExchangeAPI core の必須正本にしない

## 8. 残る実装裁定

次の詳細は、実装時に確定する。

- `Sign(string payload)` だけでよいか、byte sequence を扱う overload を追加するか
- provider を venue 別にするか、provider / session へ venue 情報を渡すか
- `PlainTextApiCredentialProvider` をどの project に置くか
- 明示 session API をどの public surface に出すか
