# ExchangeAPI Agent Guide

このファイルは、ExchangeAPI で作業するエージェントと開発者向けの作業規約である。  
設計正本ではない。判断に迷う場合は、下記の正本文書を優先する。

## 1. Read First

作業前に、目的に応じて次を確認する。

- 文書体系: [`docs/docs-architecture.md`](docs/docs-architecture.md)
- 文書棚卸し: [`docs/document-inventory.md`](docs/document-inventory.md)
- library 共通正本: [`docs/spec.md`](docs/spec.md)
- bitFlyer endpoint 正本: [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md)
- Binance endpoint 正本: [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
- CLI adapter 正本: [`docs/cli.md`](docs/cli.md)
- MCP Server adapter 正本: [`docs/mcp-server.md`](docs/mcp-server.md)
- MCP tool ledger: [`docs/mcp-tool-catalog.md`](docs/mcp-tool-catalog.md)
- verification 正本: [`docs/verification.md`](docs/verification.md)
- credentials / auth provider guide: [`docs/guides/credentials-and-auth-provider.md`](docs/guides/credentials-and-auth-provider.md)
- v2 breaking changes: [`docs/breaking-changes-v2.0.0.md`](docs/breaking-changes-v2.0.0.md)
- v2 migration guide: [`docs/migration-v2.0.0.md`](docs/migration-v2.0.0.md)

## 2. Current Baseline

- `docs/spec.md` は `v2.0.0` の library 共通正本として読む。
- `Call<TRequest, TResponse>` ではなく `CallResult<TRequest, TResponse>` を前提にする。
- facade public method は `*Async(...)` を前提にする。
- factory method は `CreateProtocolClientBundle(...)` / `CreateNativeClientBundle(...)` を前提にする。
- `BitflyerClientFactory` / `BinanceClientFactory` の class 名は維持する。
- endpoint matrix の facade rule と実装 surface の rename は、実装変更と同時に反映する。
- auth provider は `IApiCredentialProvider` / `IApiCredentialSession` / `OpenSessionAsync(...)` を前提にする。
- `ApiSecret` は公開 API に出さず、session の `Sign(payload)` で署名する。
- v2 の署名 API は `Sign(string payload)` のみとし、byte sequence overload は post-v2 検討に回す。
- provider は venue-specific class とする。
- `PlainText` / `AgeFile` provider は `ExchangeApi.Optional.Credentials` に置く。
- optional credentials project は `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj` とする。
- CLI / MCP / live test の API key 読み込みは `--credential-profile <path>` または `local/credentials/credential-profile.json` を前提にし、環境変数を使わない。
- credential profile は `local/credentials/current/` 配下の symlink convention を使ってよい。
- `scripts/create-age-credential-file.sh` は age 暗号化済み credentials file を作成する local setup helper であり、credential 管理機構として扱わない。
- `AgeFile` provider の復号後 JSON は `version`, `venue`, `apiKey`, `apiSecret` を required とする。
- 明示 session overload は private endpoint のみに追加し、`EndpointAsync(request, credentialSession, cancellationToken)` の順にする。

## 3. Documentation Rules

- README は入口と文書マップに留める。
- guide は利用手順に集中させ、正本レベルの契約を重複保持しない。
- endpoint ごとの exact contract は endpoint matrix に置く。
- adapter 固有契約は CLI / MCP の正本文書に置く。
- v2 の採用判断は breaking changes ledger と migration guide に残す。
- archive 配下は履歴であり、現行正本として扱わない。

## 4. Verification And Evidence

- deterministic test 本体は `tests/` に置く。
- manual / live verification の本体、runbook、scenario、replay input template は `verification/` に置く。
- 実行結果、artifact、log、手動確認メモは `local/evidence/` に置く。
- `local/evidence/<phase>/<yyyymmdd>-<label>/runtime/{artifacts,logs}` と `notes/` を標準構成とする。
- phase は `static`, `verification`, `local-live`, `test-operation` を使う。
- `local/evidence/` 配下の run directory は repository の正本ではない。
- credentials、署名値、API key / secret を evidence、log、exception、result に含めてはならない。

## 5. Implementation Rules

- 既存変更を勝手に戻さない。
- 文書正本を更新せずに public surface、endpoint contract、adapter contract を変えない。
- endpoint matrix の `ExpectedStatus`, `ResponseShape`, `AuthType`, `OptionalOmissionRule` と実装をずらさない。
- `Protocol` / `Native` / `Composition` の依存方向を守る。
- `Native` は exchange-native contract 層であり、取引所横断正規化を持ち込まない。
- `Unified` は未実装であり、意味同一性を保証できる capability だけを将来載せる。
- MCP は read-only 情報取得を拡張してよいが、注文、キャンセル、入金、出金などの state-changing operation は扱わない。

## 6. Local Files

- `local/` 配下は原則 local-only とする。
- `local/evidence/README.md` と phase directory の `.gitkeep` 以外の evidence run は commit しない。
- raw protocol log は `local/logs/` に出してよいが、後から確認する証跡は必要範囲を `local/evidence/` に整理する。
- secret や認証済み実行結果を repository artifact にしない。
