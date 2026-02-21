# REVIEW-03 実装パターン統一レビュー（Exchanges）
Status: Active

対象: `src/Exchanges` 配下（Bitflyer / Bittrade / Common）

前提:
- 命名規約は REVIEW-01 を前提（本レビューでは再評価しない）
- 引数設計は REVIEW-02 を前提（本レビューでは再評価しない）
- 本レビューは提案のみ（実装変更なし）

---

## A. 推奨する標準実装フロー

### 標準フロー（文章）
1. **HTTP 実行（Wire/Raw）**
   - `Wire` は transport の失敗（通信・タイムアウト）だけを `CallErrorKind.Transport` で返す。
   - `Raw` は HTTP ステータス判定（2xx 判定）と JSON デシリアライズのみを責務とし、`CallErrorKind.Http` / `Codec` に閉じる。
2. **業務エラー判定（Normalized）**
   - 2xx でも payload 内 `status` / `error_code` / `message` などの業務エラーは、ここで一律に `CallErrorKind.Semantic`（または専用分類）へ変換する。
   - HTTP エラーと payload 業務エラーを混ぜない。
3. **Raw → Normalized 変換（Mapper/Normalizer）**
   - 日時・decimal・null の扱いは「失敗時のルール」を共通化（例: `Fail-fast` か `Null許容` かをドキュメント化）し、取引所間で同形にする。
   - 変換失敗（例外/戻り値エラー）は `CallErrorKind.Mapping` へ集約する。
   - `CallErrorKind.Semantic` は **BusinessErrorDetector で検出した業務ルール違反に限定**する（例外型では分類しない）。
4. **Contracts 変換（Adapter）**
   - `Normalized` の `Call` を `ApiCallMapper` で `Contracts` DTO へ変換。
   - メタ情報（`children`, `endpointId`, `component`）の伝播を共通化。
5. **即時エラーの生成規約**
   - 事前バリデーション失敗（symbol未対応、入力不正など）は「共通 helper」で `CreateImmediateError` を生成。
   - `StartedAt/Duration/Meta` を統一フォーマットに固定。

### 簡易擬似コード
```pseudo
function UseCase(request):
  started = UtcNow

  rawCall = Raw.SendAndParse(request)
  if rawCall is Err:
    return Propagate(rawCall)   // HTTP/Codec/Transport

  biz = BusinessErrorDetector(rawCall.response)
  if biz is Err:
    return SemanticError(biz)

  normalized = Normalizer.Map(rawCall.response)
  if normalized is Err:
    return MappingError(normalized)

  contract = AdapterMapper.Map(normalized)
  if contract is Err:
    return MappingError(contract)

  return Ok(contract)
```

---

## B. 標準フローからの逸脱一覧

### 1) 取引所内での Public フロー配置が不統一（Bitflyer は PublicClient直書き / Bittrade は MarketApi 委譲）
- Issue:
  - 同じ Public API 層でも、Bitflyer は `PublicClient` に市場解決～例外変換まで実装され、Bittrade は `PublicClient` から `MarketApi` に委譲している。
- Evidence:
  - `src/Exchanges/Bitflyer/Adapter/Public/Api/PublicClient.cs` / `PublicClient.GetTickerAsync`, `GetBoardAsync`, `GetExecutionsPublicAsync`
  - `src/Exchanges/Bittrade/Adapter/Public/Api/PublicClient.cs` / `PublicClient.GetTickerAsync`（`_marketApi` へ委譲）
  - `src/Exchanges/Bittrade/Adapter/Public/Api/MarketApi.cs` / `GetDetailMergedCallAsync`, `GetDepthCallAsync`
- Why it matters:
  - 同形ユースケースのテスト対象粒度が取引所ごとに変わり、テスト設計（どこを直接テストするか）が分岐する。
- Proposed rule:
  - Public のオーケストレーション層は全取引所で「`MarketApi` など専用層に統一」または「`PublicClient` 直書きに統一」のどちらかに寄せる。
- Severity:
  - P2

### 2) 業務エラー（payload status）判定の位置と有無が不統一
- Issue:
  - Bittrade は `NormalizedPublicApi` で `status == ok` を業務エラー判定しているが、同形の判定ステージが他実装では明示的に見えにくい。
- Evidence:
  - `src/Exchanges/Bittrade/Normalized/Public/Api/NormalizedPublicApi.cs` / `TryRequireOk`, 各 API メソッド内 `TryRequireOk(...)` 呼び出し
  - `src/Exchanges/Bitflyer/Normalized/Public/Api/NormalizedPublicApi.cs` / `CreateCall(...)` 内で各 Normalizer は実行されるが、`TryRequireOk` 相当の専用 business error detector 関数が明示されていない
- Why it matters:
  - 「HTTPエラー」と「業務エラー」の責務境界が取引所別に読み取りづらく、障害切り分けと共通テストテンプレート作成が難しくなる。
- Proposed rule:
  - 業務エラー判定は **必ず Normalized 層の専用関数（例: `TryRequireOk`/`TryParseBusinessError`）に集約**し、Raw 層では扱わないことを明文化する。
- Severity:
  - P1

### 3) Mapping 例外の分類規約が不統一（InvalidOperationException の扱い差）
- Issue:
  - Bitflyer `NormalizedPrivateApi.MapOk` は `InvalidOperationException` を `Semantic` として特別扱いするが、Bittrade 側は通常の `Exception` と同列に `Mapping` 扱い。
- Evidence:
  - `src/Exchanges/Bitflyer/Normalized/Private/Api/NormalizedPrivateApi.cs` / `MapOk(...)` の `catch (InvalidOperationException ex)`
  - `src/Exchanges/Bittrade/Normalized/Private/Api/NormalizedPrivateApi.cs` / `MapOk(...)` は一般 `catch (Exception ex)` のみ
- Why it matters:
  - 同種の失敗でも `CallErrorKind` が取引所でズレ、横断監視・アラート・テスト期待値が分岐する。
- Proposed rule:
  - 例外型ベース分類を禁止し、**発生段ベース**で分類を固定する。
  - `BusinessErrorDetector` 段の失敗のみ `CallErrorKind.Semantic`。
  - `Mapper/Normalizer` 段の失敗（`InvalidOperationException` を含む）は `CallErrorKind.Mapping`。
- Severity:
  - P1

### 4) decimal の不正値ハンドリングが不統一（Fail-fast vs null吸収）
- Issue:
  - Bittrade Normalizer は decimal 文字列不正を `CallError` として失敗させる一方、Bitflyer 一部処理は `decimal? null` に吸収。
- Evidence:
  - `src/Exchanges/Bittrade/Normalized/Internal/Mappers/Normalizer.cs` / `TryParseDecimal(...)`, `TryParseDecimalFlexible(...)`
  - `src/Exchanges/Bitflyer/Normalized/Private/Api/NormalizedPrivateApi.cs` / `TryParseDecimal(JsonElement)`, `TryParseDecimalString(...)`
- Why it matters:
  - 数値不正データに対する挙動が取引所で異なり、同形テストケース（不正decimal入力時）で期待結果が分岐する。
- Proposed rule:
  - decimal 変換は「必須項目は Fail-fast」「任意項目は null許容」など判定規則を共通化し、共通 helper へ寄せる。
- Severity:
  - P1

### 5) timestamp 欠損時の補完戦略が不統一（UtcNow補完の有無）
- Issue:
  - Bittrade では ticker/tickers で timestamp 欠損時に `UtcNow` を補完する箇所がある。
- Evidence:
  - `src/Exchanges/Bittrade/Normalized/Internal/Mappers/Normalizer.cs` / `TryNormalizeTicker`（`tick.Ts ?? response.Ts ?? DateTimeOffset.UtcNow`）
  - `src/Exchanges/Bittrade/Normalized/Internal/Mappers/Normalizer.cs` / `BuildTickers`（`var now = DateTimeOffset.UtcNow`）
  - `src/Exchanges/Bitflyer/Normalized/Internal/Mappers/TickerNormalizer.cs` / wire timestamp をそのまま採用
- Why it matters:
  - 実データ時刻と観測時刻が混在し、時系列比較・再現テストで取引所間の意味が揃わない。
- Proposed rule:
  - timestamp 欠損時は `UtcNow` で埋めず、ポリシーを `null許容` または `エラー` のいずれかに固定する。
  - 観測時刻が必要な場合は timestamp 本体へ混在させず、`ObservedAt` など別メタ項目で保持する。
- Severity:
  - P1

### 6) Endpoint component 生成が一部ハードコードされ、規約逸脱
- Issue:
  - Bittrade RawPrivateClient の一部メソッドが `Component(EndpointIds.XXX)` を使わず文字列直書き。
- Evidence:
  - `src/Exchanges/Bittrade/Raw/Private/Api/RawPrivateClient.cs` / `PostWithdrawVirtualByAddressIdCreateCallAsync`（`"Bittrade.PostWithdrawApiCreateByAddressId"`）
  - 同ファイル末尾の `Component(string endpointId)` 共通関数との不一致
- Why it matters:
  - ログ・メトリクス・テストアサーション（component名一致）が endpointId 駆動にならず、漏れや typo を生む。
- Proposed rule:
  - component は全メソッドで `Component(EndpointIds.XXX)` 経由のみ許可（直書き禁止）。
- Severity:
  - P2

### 7) 共通化可能な実装が取引所ごとに複製されている（Raw/Adapter基盤）
- Issue:
  - `RawCallExecutor` と `ApiCallMapperBase` が取引所ごとに実質同一実装。
- Evidence:
  - `src/Exchanges/Bitflyer/Raw/Api/RawCallExecutor.cs` と `src/Exchanges/Bittrade/Raw/Api/RawCallExecutor.cs`
  - `src/Exchanges/Bitflyer/Adapter/Internal/ApiCallMapperBase.cs` と `src/Exchanges/Bittrade/Adapter/Internal/ApiCallMapperBase.cs`
- Why it matters:
  - 仕様変更（例えば HTTP/Codec エラー整形）の際に二重修正が必要で、ドリフトの温床になる。
- Proposed rule:
  - 取引所非依存処理は `Exchanges/Common` へ抽出し、拡張点のみ取引所側へ残す。
- Severity:
  - P1

---

## C. 共通化候補パターンのグルーピング（実装はまだ行わない）

### Group-1: Call パイプライン骨格（高優先）
- 対象:
  - `RawCallExecutor`（HTTP2xx判定・Codec変換）
  - `ApiCallMapperBase`（Normalized→Contracts変換）
  - `CreateCall/MapOk/CreateImmediateError` 系 helper
- 狙い:
  - エラー分類・メタ伝播・例外ハンドリングの統一

### Group-2: Business Error Detector（高優先）
- 対象:
  - Bittrade `TryRequireOk` 相当
  - 将来の `error_code`/`message` 解析
- 狙い:
  - HTTP と業務エラーの段階分離を固定化

### Group-3: Scalar Normalization Policy（高優先）
- 対象:
  - 日時補完ルール（timestamp）
  - decimal 変換ルール（必須/任意の失敗方針）
  - null 許容の統一
- 狙い:
  - 取引所差分を「仕様差」に限定し、実装差分を減らす

### Group-4: Component/Endpoint 命名生成（中優先）
- 対象:
  - `Component(EndpointId)` の利用統一
  - 直書き component 排除
- 狙い:
  - ログ・メトリクス・テストの安定化

### Group-5: Public API orchestration 配置（中優先）
- 対象:
  - `PublicClient` 直書き vs `MarketApi` 委譲
- 狙い:
  - テスト構造の統一（共通 fixture/template 化）

---

## 総評
- 現状は「Raw→Normalized→Contracts」という大枠は概ね揃っているが、
  - 業務エラー判定の見せ方
  - 例外分類の粒度
  - scalar 変換（日時/decimal/null）
  - 共通骨格コードの重複
  にズレがある。
- まずは **Group-1〜3** の規約化（ドキュメント化）を先行し、その後に段階的な共通化を進めると、テスト分岐と運用ドリフトを最小化できる。

---

## 実施メモ（2026-02-11）
- 指摘 2（業務エラー判定の位置/有無の不統一）は対応済み。
- `Bitflyer` / `Bittrade` の `NormalizedPublicApi.CreateCall` に business error detector を必須引数として追加し、`MapOk` の先頭で必ず評価する形へ統一。
- `Bittrade` は `TryRequireOk` のインライン呼び出しを廃止し、`DetectInvalidStatus` を detector 段として適用。
- `Bitflyer` は payload 業務エラーがない endpoint でも `NoBusinessError` detector を明示的に通す構成に変更。
- 指摘 3（Mapping 例外分類の不統一）は対応済み。
- `Bitflyer NormalizedPrivateApi.MapOk` の `InvalidOperationException => Semantic` 特例を削除し、`MapOk` 例外は `Mapping` に一本化。
- 指摘 4（decimal 不正値ハンドリング不統一）は対応済み。
- decimal 方針を「必須は Fail-fast（Mapping）／任意は null 許容。ただし不正フォーマットは Mapping」に固定。
- `Bitflyer` の `TradingCommissionResponse.commission_rate` は空/null を `null` 許容し、不正値は `Mapping` で失敗する実装へ変更。
- 指摘 5（timestamp 欠損時の補完戦略不統一）は対応済み。
- `Bittrade` の `UtcNow` 補完を除去し、endpoint ごとの `TimestampPolicy`（`Required` / `Optional`）で欠損時挙動を固定。
- `GetDetailMerged` は `Required`（欠損時 `Mapping`）、`GetTickers` は `Optional`（欠損時 `null`）として実装。
- 指摘 6（Endpoint component 直書き）は対応済み。
- `Bittrade RawPrivateClient.PostWithdrawVirtualByAddressIdCreateCallAsync` の component 直書きを廃止し、`Component(EndpointIds.PostWithdrawVirtualByAddressIdCreate)` へ統一。
- 指摘 7（`RawCallExecutor` / `ApiCallMapperBase` 重複）は対応済み。
- `RawCallExecutor` は `src/Exchanges/Common/Raw/Api/RawCallExecutor.cs` へ一本化し、Bitflyer/Bittrade Raw から共通実装を利用。
- `ApiCallMapperBase` は Bitflyer/Bittrade 両方で削除し、`ApiCallMapper` から `AdapterCallMapper` へ直接委譲する形に統一。
