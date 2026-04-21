# ExchangeAPI v1.0.0 Release Note

## Purpose
GitHub Release にそのまま貼れる `ExchangeAPI v1.0.0` の release note を置く。

## Summary
`ExchangeAPI v1.0.0` は、学習・形成フェーズとして積み上げてきた `stage` 系列から、
version 主体の公開系列へ移る最初の固定点である。

この release では少なくとも次を固定する。
- bitFlyer を主対象とした library / adapter 基盤
- Binance `GetKlines` を含む現行サポート範囲
- CLI adapter
- MCP Server adapter
- local NuGet / local publish の current recipe

## Support Boundary
- bitFlyer が main supported surface
- Binance は public `GetKlines` のみ
- `Unified` は未実装
- CLI と MCP Server は current branch で利用可能

## Distribution
- 現段階の正規導線は source checkout + `ProjectReference`
- local NuGet feed と local publish recipe は用意されている
- library package は GitHub Packages に publish 済み
- executable は release asset 側を正式導線とする

公開済み library package:
- `ExchangeApi.Primitives`
- `ExchangeApi.Exchanges.Bitflyer.*`
- `ExchangeApi.Exchanges.Binance.*`

## Verified Current State
- solution build と test を current fixed point として確認
- `CTradeBot` 側の `v1.0.0` 固定点は `ExchangeAPI v1.0.0` を前提にする
- `ExchangeApi.Exchanges.Bitflyer.Composition v1.0.0` は GitHub Packages からの consumer smoke test を確認

## Notes
- `stage` 系 tag は履歴として残す
- 破壊的変更の将来設計はこの release のスコープ外
