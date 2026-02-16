# Public Surface for Bot Consumers

この文書は、Bot 別リポジトリから参照してよい公開面を最小固定するための参考文書です。

## 許可された参照先

- `src/Contracts/Common`（`ExchangeApi.Contracts.Common`）
- `src/Contracts/Facade`（`ExchangeApi.Contracts.Facade`）

## 非推奨ではなく禁止する参照先

- `src/Exchanges/*` 配下（Wire / Raw / Normalized / Adapter / Composition）
- `src/Transport` / `src/Utilities` / `src/Application` の内部実装層

## 運用ルール

- Bot 側は Contract 層インターフェースと DTO のみを依存対象とする。
- 取引所固有差分は Bot 側で直接解決せず、ExchangeAPI 側の Composition/Adapter に閉じ込める。
- 公開面の変更は `docs/contracts/contracts.md` と本書を同時に確認する。
