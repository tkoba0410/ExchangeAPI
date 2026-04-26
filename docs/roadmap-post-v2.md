# ExchangeAPI Post-v2 Roadmap

最終更新: 2026-04-24  
位置づけ: post-v2 roadmap

本書は、`v2.0.0` には含めないが、`v2.0.0` 以降に検討する候補を記録する。  
ここにある項目は採用済み正本ではない。実装する場合は、該当する正本文書と migration / release 文書を先に更新する。

## 1. 前向きに検討する候補

| 項目 | 状態 | 理由 | 備考 |
| --- | --- | --- | --- |
| `ExchangeApi.Optional.Logging` | 保留 | core を薄くし、CLI / MCP / live test / bot / local evidence など用途別に適した log writer を作れるため | `BC-V2-022` として記録済み |
| `Unified` 層の実装 | 将来 | 複数 venue 間で意味同一性を保証できる capability だけを載せるため | まず venue-native surface を安定させる |
| optional credentials provider 拡張 | 将来 | `age` 以外の env / keychain / external secret manager へ広げられるため | v2 初手は `PlainText` と `AgeFile` を優先 |
| `ExchangeApi.Optional.Configuration` | 将来候補 | env / config binding / provider factory を adapter 間で共通化できるため | adapter 側の重複が見えてから検討する |
| `ExchangeApi.Optional.Testing` | 将来候補 | live test helper、fake provider、record/replay support を core から外して提供できるため | まず `tests/` と `verification/` 整理を優先 |
| `ExchangeApi.Optional.Resilience` | 将来候補 | retry / backoff / rate limit / circuit breaker を core 正本に入れずに提供できるため | venue・利用者ごとに要件差が大きい |
| evidence 自動整理 | 将来候補 | `local/evidence/` 標準構成へ artifact / log / notes を自動配置できるため | まず標準構成だけ固定する |
| `samples/` directory | 将来候補 | guide 内サンプルが大きくなった場合に、実行可能サンプルとして分離できるため | 早期作成は保守対象を増やす |
| MCP client / human trial CLI | 将来候補 | 人間が MCP server を試す導線を用意できるため | v2 では MCP server 側の read-only surface を優先 |
| venue 単位 package / project consolidation | v3 候補 | 利用者導線を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` に整理し、package 数を減らすため | v2.1.0 では扱わず、破壊的変更を許容する v3 の主題候補とする |

## 1.1 v2.1.0 採用項目

`v2.1.0` では次を採用する。

- `ExchangeApi.Optional.Logging`
- safe redaction
- evidence directory helper
- MCP read-only inspection tools
  - `get_collateral_accounts`
  - `get_balance_history`
  - `get_collateral_history`
  - `get_child_orders`

`Unified`、`ExchangeApi.Optional.Resilience`、credentials provider 拡張、`samples/`、MCP client / human trial CLI、package / project consolidation は `v2.1.0` では扱わない。

## 2. optional project 候補

`optional` は、core 正本に入れると責務が太るが、実用上あると便利な具体実装を置く場所とする。

候補:

- `ExchangeApi.Optional.Credentials`
  - `PlainTextApiCredentialProvider`
  - `AgeFileApiCredentialProvider`
  - future: environment / keychain / external secret manager provider
- `ExchangeApi.Optional.Logging`
  - JSONL log writer
  - file log writer
  - redaction helper
  - local evidence writer
  - human-readable log writer
- `ExchangeApi.Optional.Configuration`
  - environment binding
  - config file binding
  - provider factory
  - adapter-shared config loader
- `ExchangeApi.Optional.Testing`
  - live test helper
  - fake credential provider
  - record/replay support
  - sanitized artifact generator
- `ExchangeApi.Optional.Resilience`
  - retry policy
  - backoff policy
  - rate limit helper
  - circuit breaker integration

## 3. optional に入れないもの

以下は core contract または exact contract に近いため、optional に逃がさない。

- `CallResult`
- `CallError`
- `CallMeta`
- `ProtocolRequest`
- `ProtocolResponse`
- endpoint request / response DTO
- endpoint module
- client factory
- endpoint matrix metadata
- `CallError.Kind` taxonomy

## 4. 基本やらない寄りの項目

次の項目は、現時点では再検討候補というより却下寄りである。

| 項目 | 理由 |
| --- | --- |
| CLI / MCP surface の 1:1 統一 | CLI は endpoint inspection / execution、MCP は bot-oriented tool surface で役割が異なるため |
| test taxonomy / project layout の大規模再編 | 現行の `Architecture / Protocol / Native / Composition / Live / Adapter` 分離が成立しているため |
| `CallError.Kind` taxonomy 再分類 | 現行の `Transport / Http / Codec / Semantic / Mapping` を維持する方が migration risk が小さいため |
| scalar / nullability / enum の横断再設計 | endpoint ごとの exact contract で個別に扱う方が安全なため |

## 5. 運用ルール

- 本書の項目は `v2.0.0` の実装対象に含めない
- 採用する場合は、対象の正本文書へ移してから実装する
- optional project を増やす場合は、core から参照しない依存方向を維持する
- optional project は便利実装であり、core contract の代替正本にしない
