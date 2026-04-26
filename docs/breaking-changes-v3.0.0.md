# ExchangeAPI v3.0.0 Breaking Changes

最終更新: 2026-04-26
位置づけ: v3.0.0 breaking changes ledger

## BC-V3-001 Venue Package / Project Consolidation

v3.0.0 では、venue ごとの外部 consumer package と repo 内 project を venue 単位に整理する。

v2 package:

- `ExchangeApi.Exchanges.Bitflyer.Composition`
- `ExchangeApi.Exchanges.Binance.Composition`
- layer-specific `Vocabulary` / `Protocol` / `Native` packages

v3 package:

- `ExchangeApi.Exchanges.Bitflyer`
- `ExchangeApi.Exchanges.Binance`

削除する layer project:

- `ExchangeApi.Exchanges.Bitflyer.Vocabulary`
- `ExchangeApi.Exchanges.Bitflyer.Protocol`
- `ExchangeApi.Exchanges.Bitflyer.Native`
- `ExchangeApi.Exchanges.Bitflyer.Composition`
- `ExchangeApi.Exchanges.Binance.Vocabulary`
- `ExchangeApi.Exchanges.Binance.Protocol`
- `ExchangeApi.Exchanges.Binance.Native`
- `ExchangeApi.Exchanges.Binance.Composition`

理由:

- 外部 consumer が選ぶ package 名を venue 単位にする
- 層別 package の選択を利用者へ強制しない
- 公開 package 単位と repo 内 project 単位を一致させる
- package 数を減らし、distribution surface を読みやすくする

移行:

```bash
dotnet remove package ExchangeApi.Exchanges.Bitflyer.Composition
dotnet add package ExchangeApi.Exchanges.Bitflyer --version 3.0.0
```

```bash
dotnet remove package ExchangeApi.Exchanges.Binance.Composition
dotnet add package ExchangeApi.Exchanges.Binance --version 3.0.0
```

source namespace は初期 slice では維持する。
`Protocol` / `Native` / `Composition` / `Vocabulary` は folder / namespace / tests 上の設計境界として維持する。
