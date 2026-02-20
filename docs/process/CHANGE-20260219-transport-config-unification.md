# CHANGE-20260219-transport-config-unification

## Summary

Transport の設定入口を `HttpClient` / `IHttpTransport` / `Timeout` の併用モデルから、
排他的 `TransportConfig` モデルへ統一した。

## What broke

- `BitflyerFactoryOptions` / `BittradeFactoryOptions` から次を削除:
  - `HttpClient`
  - `Transport`
- `Bitflyer` / `Bittrade` の Adapter `ClientOptions` から次を削除:
  - `HttpClient`
  - `Timeout`
- 置換として `TransportConfig` を追加:
  - `ExternalTransport`
  - `ExternalHttpClient`
  - `ManagedHttp`
- bitFlyer Adapter Factory の旧オーバーロードを整理:
  - `CreatePublic(ClientOptions, HttpClient?, IHttpTransport?)` を廃止
  - `Create(ClientCredentials, ClientOptions, HttpClient?, IHttpTransport?)` を廃止

## Why

- 既存モデルでは設定の同時指定が可能で、優先順位競合とサイレント無視が発生し得た。
- `transportOverride` 指定時に未使用 `HttpClient` が生成されるリーク経路が存在した。
- 取引所別実装で挙動が分岐し、`BittradeFactoryOptions.Transport` が Public 経路で無効化される不整合があった。

## Migration

1. `HttpClient` / `Transport` / `Timeout` の直接指定をやめ、`TransportConfig` を1つだけ指定する。
2. 外部 transport を使う場合:
   - `TransportConfig = new TransportConfig.ExternalTransport(myTransport)`
3. 外部 `HttpClient` を使う場合:
   - `TransportConfig = new TransportConfig.ExternalHttpClient(myHttpClient)`
4. ライブラリ管理 `HttpClient` を使う場合:
   - `TransportConfig = new TransportConfig.ManagedHttp(timeout: TimeSpan.FromSeconds(...))`

## Bot impact

- 既存 Bot が FactoryOptions / ClientOptions に `HttpClient` / `Transport` / `Timeout` を直接設定している場合は修正が必要。
- 設定競合の解釈がなくなり、接続経路が `TransportConfig` で一意に決定されるため、
  実行時挙動は追跡しやすくなる。
