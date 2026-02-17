# CHANGE-20260217-contracts-capability-boundary

## Summary

Contracts の公開面を再整理し、以下を実施した。

- `BatchError` から取引所識別情報を除去
- `GetCandlesticksAsync` を `IPublicApi` から分離し、`ICandlesticksApi` を追加
- `IExchangeClient` に nullable capability `Candlesticks` を追加

## What broke

- `IPublicApi` を直接利用して `GetCandlesticksAsync` を呼んでいたコードはコンパイルエラーになる
- `BatchError.Exchange` 参照コードはコンパイルエラーになる

## Why

- Contracts の取引所非依存（Exchange 識別情報禁止）を徹底するため
- 「未対応機能は NotSupported 常用ではなく nullable capability で表現する」契約に一致させるため

## Migration

1. `IPublicApi.GetCandlesticksAsync(...)` 呼び出しを `IExchangeClient.Candlesticks?.GetCandlesticksAsync(...)` に置換する  
   （null の場合は capability 未提供として分岐）
2. `BatchError.Exchange` 参照を削除し、必要な取引所識別は Composition / Application 側の文脈で管理する
3. 必要なら `ICandlesticksApi` 依存を明示して DI 配線を更新する

## Bot impact

- Bot 側で `IPublicApi` 依存のローソク足呼び出しがある場合は修正が必要
- Bot 側で `BatchError` から取引所識別を読み取っている場合は文脈注入方式へ変更が必要
