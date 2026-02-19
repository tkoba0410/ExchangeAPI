# CHANGE-20260219-contract-client-interface-segregation

## Summary

Contracts の公開クライアント契約を `IExchangeClient`（nullable capability）から、
Public / Private / optional capability の interface 分離へ変更した。

## What broke

- `IExchangeClient` は削除された（破壊的変更）。
- Factory 戻り型は以下に変更された:
  - `CreateContractPublicClient(...) -> IContractPublicClient`
  - `CreateContractPrivateClient(...) -> IContractPrivateClient`
- optional capability は `null` 判定ではなく interface 判定に変更された
  （例: `IContractCandlesticksClient`）。

## Why

- nullable capability モデルでは、同一型に Public/Private/optional が混在し、
  呼び出し前提条件の論理が曖昧になりやすい。
- interface 分離により、入口契約と capability 判定を型で固定できる。

## Migration

1. `IExchangeClient` 参照を `IContractPublicClient` / `IContractPrivateClient` へ置換する。
2. `client.Public` / `client.Private` のプロパティ利用を廃止し、
   返却インターフェイスへ直接呼び出す。
3. optional capability 判定を `client.Candlesticks != null` から
   `client is IContractCandlesticksClient` へ置換する。

## Bot impact

- Contracts 利用コードは型置換と capability 判定の修正が必要。
- Normalized 利用コードへの影響はない。
