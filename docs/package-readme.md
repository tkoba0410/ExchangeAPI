# ExchangeAPI Packages

ExchangeAPI は、複数の暗号資産取引所 API を扱うための .NET library / adapter 基盤です。

## Support Boundary

- bitFlyer が主対象
- Binance は current `GetKlines` を含む限定サポート
- `Unified` は未実装

## Recommended Package Entry Point

通常は `Composition` package を入口に使います。

- `ExchangeApi.Exchanges.Bitflyer.Composition`
- `ExchangeApi.Exchanges.Binance.Composition`

これらは必要な `Native` / `Protocol` / `Vocabulary` / `Primitives` を dependency として引きます。

## Distribution Note

- CLI と MCP Server は executable であり、package ではなく release asset 側を正式導線とします
- consumer guidance の詳細は repository の `README.md` と `docs/local-nuget-consumer.md` を参照してください
