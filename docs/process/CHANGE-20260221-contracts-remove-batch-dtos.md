# CHANGE-20260221-contracts-remove-batch-dtos

## Summary

Contracts から未使用の Batch 系 DTO を削除した。

- `BatchResult<TItem>` を削除
- `BatchError` を削除
- `BatchErrorKind` を削除

あわせて、関連する契約文書（`contracts.md` / `overview.md` / `resilience.md`）を現行実装に整合させた。

## What broke

- `ExchangeApi.Contracts.Common` から `BatchResult` / `BatchError` / `BatchErrorKind` を参照しているコードはコンパイルエラーになる

## Why

- 現行の Contracts/Adapter 公開面では Batch 系 DTO を返す API が存在しないため
- 未使用 DTO を公開面に残すと、契約文書と実装の整合を崩しやすいため

## Migration

1. `BatchResult` / `BatchError` / `BatchErrorKind` の参照を削除する
2. 集約結果を扱う必要がある場合は、利用層（Normalized または Composition）で取引所固有 DTO に基づく結果型を定義する
3. 将来 Contracts に集約 API を導入する場合は、`docs/normative/contracts/resilience.md` の導入時規約に従って型を新設する

## Bot impact

- Bot が `ExchangeApi.Contracts.Common` の Batch 系 DTO に直接依存している場合は修正が必要
- 現行 Contracts API（`IContractPublicClient` / `IContractPrivateClient`）のメソッド署名自体は変更なし
