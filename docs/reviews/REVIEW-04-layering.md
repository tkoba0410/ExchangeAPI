# REVIEW-04: Layering / Dependency Boundary Review (`src/Exchanges`)

対象: `src/Exchanges/Bitflyer` / `src/Exchanges/Bittrade` / `src/Exchanges/Common`

前提の扱い:
- REVIEW-01（命名）・REVIEW-02（引数設計）・REVIEW-03（実装フロー/エラー分類/scalar）は再評価しない。
- 本書は「層責務・依存境界・配置の統一性」に限定した提案レビュー。

## A. 層責務の短い定義案 v1（機械判定向け）

1. **Wire層**は `WireCallSpec` の組み立て・送信のみを担当し、DTOの意味解釈をしない。
2. **Wire層**は `Raw/Normalized/Adapter` を参照してはならない。
3. **Raw層**は Wire endpoint 呼び出し + JSONデコード（Raw DTO化）までを担当する。
4. **Raw層**はドメイン正規化（symbol/period解釈、業務語彙）を持ってはならない。
5. **Normalized層**は Raw DTO → 正規化DTO 変換と交換所仕様差吸収を担当する。
6. **Normalized層**は `Wire.Internal` / `Wire.Constants` を直接参照してはならない（endpoint識別子はRaw由来メタを中継）。
7. **Adapter層**は Facade契約（Contracts）への最終写像とオーケストレーションのみを担当する。
8. **Adapter層**は `Normalized.Public/Private.Api` と `Contracts` のみを境界依存とし、`Normalized.Internal.*` を直接参照しない。
9. **PublicClient** は「Facade entry point」に限定し、実処理オーケストレーションは `*MarketApi` 等の専用クラスに委譲する。
10. **MarketResolver** は Common境界で共通化し、実装差は取引所モジュール内部へ閉じ込める。
11. `CallMeta` 構築/例外→Call変換は共通ユーティリティに集約し、各取引所で同一アルゴリズムを重複実装しない。
12. 依存方向は常に `Adapter -> Normalized -> Raw -> Wire` の単方向とし、逆方向参照は禁止。
13. ただし composition root（`ApiBundle`/`ClientFactory`）での生成時依存は例外許容し、実処理ロジックでの逆参照は禁止。

## B. 逸脱箇所一覧

- Issue: Publicオーケストレーションの配置が取引所間で非対称（`PublicClient`直書き vs `MarketApi`委譲）。
- Evidence: 修正前は `src/Exchanges/Bitflyer/Adapter/Public/Api/PublicClient.cs` に実処理が直書きされていた。修正後は `src/Exchanges/Bitflyer/Adapter/Public/Api/PublicClient.cs` / `src/Exchanges/Bittrade/Adapter/Public/Api/PublicClient.cs` ともに `MarketApi` 委譲のみ。オーケストレーション本体は `src/Exchanges/Bitflyer/Adapter/Public/Api/MarketApi.cs` / `src/Exchanges/Bittrade/Adapter/Public/Api/MarketApi.cs`。
- Why it matters: endpoint追加時の実装先・テスト単位（PublicClient単体かMarketApi単体か）が分岐し、横展開の事故率を上げる。
- Proposed rule: 全取引所で `PublicClient = entrypoint only`、業務オーケストレーションは `MarketApi`（または固定名の同等クラス）に集約する。
- Severity: P1

- Issue: Adapterが `Normalized.Internal` に直接依存しており、層境界が漏れている。
- Evidence: 現在は `rg -n "Normalized\\.Internal" src/Exchanges/Bitflyer/Adapter src/Exchanges/Bittrade/Adapter` で 0 件。`NormalizedMarketResolver` の契約型は `Normalized.Internal.Markets` から `Normalized.Api.Markets` へ昇格済み。
- Why it matters: Normalized内部リファクタがAdapter破壊に直結し、境界越え変更の影響範囲が急増する。
- Proposed rule: Adapterが参照可能なのは `Normalized.Api` と `Normalized.Public/Private.Dtos` のみ。`Normalized.Internal.*` 参照は禁止。
- Severity: P1

- Issue: Normalizedが `Wire.Constants` を直接参照し、`Normalized -> Raw -> Wire` の段階境界を飛び越えている。
- Evidence: 修正前は `src/Exchanges/Bitflyer/Normalized/Public/Api/NormalizedPublicApi.cs` / `src/Exchanges/Bitflyer/Normalized/Private/Api/NormalizedPrivateApi.cs` / `src/Exchanges/Bittrade/Normalized/Public/Api/NormalizedPublicApi.cs` / `src/Exchanges/Bittrade/Normalized/Private/Api/NormalizedPrivateApi.cs` が `Wire.Constants` を参照。修正後は `src/Exchanges/Bitflyer/Normalized/Internal/Constants/EndpointIds.cs` / `src/Exchanges/Bittrade/Normalized/Internal/Constants/EndpointIds.cs` へ置換済み。
- Why it matters: endpoint ID語彙の変更が Normalized まで波及し、Raw/Wireの改修を局所化できない。
- Proposed rule: EndpointId/Component語彙はRawが確定して `CallMeta` に載せ、Normalizedは受け渡しのみ行う。
- Severity: P1

- Issue: `Call`写像ユーティリティがExchange単位で重複し、Common境界が曖昧。
- Evidence: 修正前は取引所ごとに `ApiCallMapperBase` 相当実装が重複していた。現在は `src/Exchanges/Common/Application/Adapter/Internal/AdapterCallMapper.cs` に集約され、`src/Exchanges/Bitflyer/Adapter/Internal/ApiCallMapper.cs` / `src/Exchanges/Bittrade/Adapter/Internal/ApiCallMapper.cs` から委譲される構成。
- Why it matters: エラー変換・meta構築方針の微差が将来発生し、取引所ごとの挙動差（監視/障害解析の非対称）を招く。
- Proposed rule: `Call<TReq,TOk>` 生成テンプレート（FromCall/MapCall/FromException）をCommonへ一本化し、Exchange固有差分はフック関数で注入する。
- Severity: P2

- Issue: 市場解決エラー組み立て（`MarketResolutionError` / `SymbolNotSupported`）が各実装クラスに重複している。
- Evidence: 修正前は `src/Exchanges/Bitflyer/Adapter/Public/Api/PublicClient.cs` / `src/Exchanges/Bitflyer/Adapter/Private/Api/ExchangeClient.cs` / `src/Exchanges/Bittrade/Adapter/Public/Api/MarketApi.cs` に同型ヘルパー。修正後は `src/Exchanges/Common/Application/Adapter/Internal/MarketResolutionCallMapper.cs` に集約。
- Why it matters: 例外分類や `CallMeta` 仕様の変更時に二重修正が必要になり、片側のみ修正の事故が起こりやすい。
- Proposed rule: resolver起因エラー変換は Common.Adapter ヘルパーへ寄せ、取引所側は operation名だけを渡す。
- Severity: P2

## C. Commonへ寄せるべき候補 / 取引所に残す拡張点

### Commonへ寄せるべき候補
- `Call` 写像テンプレート（`MapCall` / `FromCall` / `FromException`）の共通実装。
- resolver失敗時の `CallMeta` / `CallError` 組み立てヘルパー。
- Public市場系（ticker/orderbook/executions/candlesticks）のオーケストレーション骨格（resolve -> normalized call -> map -> wrap）。
- `PublicClient` の責務テンプレート（entrypointのみ）とテスト観点テンプレート。

### 取引所に残す拡張点
- Raw DTO定義・JSON decode仕様・Wire endpoint定義（Path/Query/HTTP method）。
- Normalizedの交換所仕様吸収ロジック（status判定、symbol/period変換、型変換）。
- Adapter最終マッピングの交換所固有差分（Contracts DTOへの個別整形）。
- MarketCatalog の静的定義と resolver 実装。

---

総評:
- プロジェクト参照方向（`Adapter -> Normalized -> Raw -> Wire`）自体は健全。
- ただし「参照可能な語彙境界（Internal / Constants）」と「オーケストレーション配置」の統一が不十分で、将来追加時の判断分岐が残っている。

## D. 対応結果（現状スナップショット）

- `PublicClient = entrypoint only` に統一し、bitFlyer 側へ `src/Exchanges/Bitflyer/Adapter/Public/Api/MarketApi.cs` を新設。
- Adapter の `Normalized.Internal.*` 直接参照は除去済み（例外なし）。
- Normalized の `Wire.Constants` 直接参照を除去（`rg -n "Wire\\.(Constants|Internal)" src/Exchanges/Bitflyer/Normalized src/Exchanges/Bittrade/Normalized` で 0 件）。
- `Call` 写像を `src/Exchanges/Common/Application/Adapter/Internal/AdapterCallMapper.cs` に共通化。
- resolver起因エラー変換を `src/Exchanges/Common/Application/Adapter/Internal/MarketResolutionCallMapper.cs` に共通化。
