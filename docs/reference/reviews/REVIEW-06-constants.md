# REVIEW-06: 定数・enum・文字列表現（マジックストリング）統一レビュー
Status: Active

## 対象と前提
- 対象: `src/` 全般（特に `Exchanges` / `Contracts`）および `docs/inventory` の endpointId/path 関連。
- 前提: REVIEW-01〜05 は受理済みとして再評価しない。
- 方針: 実装修正は行わず、統一ルールと改善提案のみを示す。

## A. 定数/enum/VO の統一ルール案 v1
1. **EndpointId / Path / QueryKey / HeaderKey / HttpMethod** は「Wire/Transport 境界語彙」として分類し、必ず定数化する。
2. EndpointId は `string` のままでも可だが、**定義元は取引所ごとに単一ファイル（Catalog + Ids）**へ限定する。
3. `Method: "GET"/"POST"` のような HTTP 動詞は `Transport` 共通定数（または enum）を唯一の正本にする。
4. `Layer` / `Component` は `CallMeta` 用の観測語彙として `Common` 側に定数集約し、直書きを禁止する。
5. 仕様準拠 JSON 名（`JsonPropertyName`）は DTO 層に閉じ込め、上位層で同一語を再記述しない。
6. 仕様由来の typo は「意図的 typo 定数」として Wire 層にのみ保持し、Normalized/Contracts へは漏らさない。
7. enum と string の併用が必要な場合、**境界で相互変換を 1 箇所に集約**し、同一文字列を複数 switch に重複させない。
8. `Closed<T>` を使う対象（仕様で拡張されうる列挙）と、厳密 enum 対象（固定語彙）を明文化する。
9. エラーメッセージ中のフィールド名は `FieldNames.*` 定数を使い、タイポ混入を機械的に防ぐ。
10. テストで markdown 表の列を読む場合、**列番号のマジックナンバー禁止**（列名解決を必須化）。
11. インベントリ参照パス（`docs/inventory/*.md`）はテスト共通ヘルパーへ寄せ、散在を防ぐ。
12. 新規取引所追加時は「Exchange 固有定数」と「Common 昇格候補」を初回 PR で必ず仕分ける。

## B. 問題箇所一覧（マジックストリング / 置き場所不統一 / 混在）

- Issue: `Layer` / `Component` が文字列直書きで散在し、観測語彙の単一正本がない。
- Evidence: `src/Exchanges/Bitflyer/Adapter/Internal/BitflyerMarketCatalogResolver.cs` / `src/Exchanges/Bittrade/Adapter/Internal/BittradeMarketCatalogResolver.cs` の `Component` と、`src/Exchanges/Common/Adapter/Internal/AdapterCallMapper.cs` の `Layer`。
- Why it matters: 監視タグ・ログ軸の表記揺れが起きると、集計クエリや障害解析が取引所追加時に壊れやすい。
- Proposed rule: `CallMetaVocabulary`（Common）を作り、`Layer/Component` 直書きを禁止する。
- Severity: P1

- Issue: HTTP method が exchange ごとに `"GET"/"POST"` 直書きされている。
- Evidence: `src/Exchanges/Bitflyer/Wire/Internal/WireSpecBuilder.cs` と `src/Exchanges/Bittrade/Wire/Internal/WireSpecBuilder.cs` の `Method: "GET"` / `"POST"`。
- Why it matters: メソッド追加（PUT/DELETE）時の重複実装・ typo リスクが増え、横展開コストが高くなる。
- Proposed rule: `Transport` 共通に `HttpMethods` 定数（または enum）を置き、WireSpecBuilder はそれのみ参照する。
- Severity: P1

- Issue: 同一ドメイン語（order type / order state）の string↔enum 変換ロジックが同一ファイル内で重複している。
- Evidence: `src/Exchanges/Bittrade/Normalized/Internal/Mappers/TradingMapper.cs` の `TryToRawOrderType` / `TryParseOrderType` / `ParseOrderTypeClosed`、および `TryParseOrderState` / `ParseOrderStateClosed`。
- Why it matters: endpoint 追加時に一方だけ更新されると、正規化結果・バリデーション・unknown handling が不一致になる。
- Proposed rule: 取引所ごとに `ExchangeOrderLexicon` を1箇所化し、双方向変換と Closed 判定を同一テーブルで定義する。
- Severity: P0

- Issue: フィールド名文字列がエラーメッセージ向けに散在し、一部 typo を含む。
- Evidence: `src/Exchanges/Bittrade/Normalized/Internal/Mappers/TradingMapper.cs` の `"amount"`, `"price"`, `"cash_amount"`, `"field-amount"`。
- Why it matters: エラー観測のキーが揺れると、テスト期待値や運用時のフィールド単位分析がドリフトする。
- Proposed rule: `FieldNames` 定数を mapper 内部に集約し、外部公開しない（仕様 typo も同所に閉じ込める）。
- Severity: P1

- Issue: 署名ヘッダー/署名クエリキーが RequestSigner に直書きで、語彙定義の所在が不統一。
- Evidence: `src/Exchanges/Bitflyer/Adapter/Internal/RequestSigner.cs` の `"ACCESS-KEY"` 等、`src/Exchanges/Bittrade/Adapter/Internal/RequestSigner.cs` の `"AccessKeyId"`, `"SignatureMethod"` 等。
- Why it matters: 認証仕様差分の追従時に、変更対象探索が実装クラス依存になりメンテコストが上がる。
- Proposed rule: 取引所配下 `Adapter/Internal/Constants/AuthKeys.cs` へ集約（Common には上げない）。
- Severity: P1

- Issue: inventory パーサが列番号マジックナンバー依存で、表構造変更に脆い。
- Evidence: `tests/Inventory/InventoryEndpointIdParser.cs` の `EndpointIdColumnIndex = 5`, `PresentInColumnIndex = 6`。
- Why it matters: docs 側で列順変更が入ると、endpoint 一貫性テストが silently 誤判定するリスクがある。
- Proposed rule: ヘッダー行から列名解決し、列番号定数の直接利用を禁止する。
- Severity: P1

- Issue: inventory ファイル相対パス文字列が複数テストに散在。
- Evidence: `tests/Docs.Inventory.Tests/EndpointTypeInventoryTests.cs` の `"docs/inventory/endpoints-bittrade.md"` / `"...bitflyer.md"` と、`tests/Exchanges/*/Raw.Endpoints.Tests/Inventory/*` の同等パス組み立て。
- Why it matters: ファイル分割・リネーム時に修正漏れが起き、テストの保守コストが増える。
- Proposed rule: `InventoryPaths` 共通ヘルパーを tests 共通に置いて再利用する。
- Severity: P2

## C. “Commonへ寄せる候補” と “取引所に残す候補”

### Commonへ寄せる候補
- Issue: HTTP method 文字列（GET/POST）が exchange 間で同一語彙なのに重複管理。
- Evidence: `src/Exchanges/Bitflyer/Wire/Internal/WireSpecBuilder.cs`, `src/Exchanges/Bittrade/Wire/Internal/WireSpecBuilder.cs`。
- Why it matters: 仕様変更時の横展開ミスを防げる。
- Proposed rule: `src/Transport`（または `src/Primitives/CallCommon`）に共通定義を置く。
- Severity: P1

- Issue: `Layer` 文字列語彙（Contracts/Raw/Normalized など）の正本がない。
- Evidence: `src/Exchanges/Common/Adapter/Internal/*.cs` と `CallMeta.CreateInternal("Raw", ...)` を使う複数箇所。
- Why it matters: 観測軸の表記揺れを全取引所で一括抑止できる。
- Proposed rule: `CallMeta` 周辺へ Layer 定数群を追加して共通利用する。
- Severity: P1

- Issue: docs inventory 参照パスの定義がテスト横断で重複。
- Evidence: `tests/Docs.Inventory.Tests/EndpointTypeInventoryTests.cs` と `tests/Exchanges/*/Raw.Endpoints.Tests/Inventory/*.cs`。
- Why it matters: inventory 拡張時の変更点を一箇所化できる。
- Proposed rule: tests 共通に inventory パス定数を集約する。
- Severity: P2

### 取引所に残す候補
- Issue: API 認証ヘッダー/署名クエリキーは取引所固有仕様。
- Evidence: `src/Exchanges/Bitflyer/Adapter/Internal/RequestSigner.cs` と `src/Exchanges/Bittrade/Adapter/Internal/RequestSigner.cs` のキー名差分。
- Why it matters: Common 化すると抽象化過剰になり、むしろ仕様追従性が落ちる。
- Proposed rule: 取引所ごと Constants に閉じ込め、共通化は「命名規約」までに留める。
- Severity: P2

- Issue: 仕様 typo を含む path（例: `currencys`）は exchange 固有の互換資産。
- Evidence: `src/Exchanges/Bittrade/Wire/Constants/Paths.cs` の `CommonCurrenciesPath = "/v1/common/currencys"`。
- Why it matters: 上位層に漏らさず Wire 内で保持すれば、互換性と可読性を両立できる。
- Proposed rule: typo は「意図的仕様値」として exchange `Paths` のみに残し、Normalized/Contracts へ伝播させない。
- Severity: P1

- Issue: 注文状態・注文種別の raw 文字列は取引所間で語彙非互換。
- Evidence: `src/Exchanges/Bittrade/Normalized/Internal/Mappers/TradingMapper.cs` の `buy-limit` 系、`src/Exchanges/Bitflyer/Normalized/Internal/Mappers/TradingMapper.cs` の `LIMIT/MARKET` 系。
- Why it matters: Common enum を強引に導入すると、unknown 値ハンドリングの柔軟性を損ねる。
- Proposed rule: raw lexicon は exchange 内部に残し、Contracts には既存 Domain enum / VO のみ露出する。
- Severity: P1
