# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Factory レイヤー概要

Factory は Transport/Adapter を組み合わせて、bitFlyer/Bittrade 向けのクライアントを簡単に構築するヘルパを提供するレイヤーです。DI を使わないシナリオでも最小コードでクライアントを得られるのが役割です。

## 主なコンポーネント
- `BitflyerClientFactory`（adapter 側）: API キー/シークレットを渡して `BitflyerExchangeClient`（Facade）を構築。HTTP/署名/Raw API/Adapters/Apis/Facade をまとめて組み立てる。Public 専用の軽量クライアント `CreatePublic()` も提供。
- `BittradeClientFactory`: Bittrade の Public/Private クライアントを組み立てる。`CreatePublicClient()`（Market+ExchangeInfo）、`CreatePrivate()`（Market/Trading/Account/Raw）を用意。
- `MultiExchangeClientFactory`: bitFlyer/Bittrade の全部入り/ Public クライアントをまとめて生成する。
- `Credentials` サブフォルダ: 認証や署名に必要な構成ヘルパ。
- `Transport` サブフォルダ: RestClient やポリシー、HttpTransport を組み立てるヘルパ。
- `JsonExchangeInfoApi`（Factory 直下 ExchangeInfo）: ExchangeInfo を JSON から読み込む `IExchangeInfoApi` 実装。複数ファイルマージ/更新検知/キャッシュTTL対応。

## 典型的な使い方
- 最小例: `BitflyerClientFactory.Create(apiKey, apiSecret)` でクライアントを取得し、`GetTickerAsync` など抽象 API を呼ぶ。
- 認証不要の Public だけ使う場合: `BitflyerClientFactory.CreatePublic()` / `BittradeClientFactory.CreatePublicClient()`。
- 複数取引所を一度に扱いたい場合: `MultiExchangeClientFactory.CreateDefault(...)`（Private 全部入り）や `CreatePublic()`（Public まとめ）で束ねる。
- ポリシーやロギングをカスタムする場合は `Transport` ヘルパを経由して RestClient 構成を差し替える。
- ExchangeInfo を JSON で外部化したい場合は `JsonExchangeInfoApi` を DI で `IExchangeInfoApi` として登録し、フォールバックで既存の bitFlyer 固定値実装を残すラッパを組む。

## ペンディング/検討事項
- Factory 内での JSON ExchangeInfo 切り替えフラグや DI 登録サンプルを追記する。
- 複数取引所に対応する際の Factory 構成テンプレートを追加する。
