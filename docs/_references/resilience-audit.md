# Resilience Audit（429 / Timeout / Partial Failure）

本書は実装監査の参考資料（Informative）であり、規範は `docs/contracts/resilience.md` を正本とする。

## 1) HTTP 呼び出し箇所

### 1.1 共通送信経路

- `src/Transport/Protocol/RestClient.cs`
  - `SendRawAsync(...)` でポリシー適用後に `IHttpTransport.SendAsync` を呼ぶ。
- `src/Transport/Http/HttpTransport.cs`
  - 実際の `HttpClient.SendAsync` を実行。
- `src/Transport/Wire/WireTransport.cs`
  - `IRestClient` を `Call<WireCallSpec, WireResponse>` にラップ。

### 1.2 ポリシー適用点

- `src/Transport/Policy/HttpPolicyFactory.cs`
  - `RateLimit -> CircuitBreaker -> Retry -> Timeout` の順で既定パイプラインを構築。
- `src/Composition/Bootstrap/Transport/RestClientFactory.cs`
  - 既定 `HttpPolicyFactory.CreateDefault()` を適用。
- 取引所 Factory:
  - `src/Exchanges/Bitflyer/Adapter/Internal/Factory/ClientFactory.cs`
  - `src/Exchanges/Bittrade/Adapter/Internal/Factory/ClientFactory.cs`

## 2) 現状の 429 / Timeout / Retry ロジック

### 2.1 429

- `src/Transport/Policy/RetryHttpPolicy.cs`
  - 429 をリトライ対象として扱う。
  - ただし `Retry-After` 未対応。

### 2.2 バックオフ

- `src/Transport/Policy/RetryHttpPolicy.cs`
  - 指数バックオフあり（`baseDelay * 2^(attempt-1)`）。
  - ジッター未実装。
  - 総リトライ時間上限未実装。

### 2.3 Timeout / Cancellation

- `src/Transport/Policy/TimeoutHttpPolicy.cs`
  - linked CTS で timeout を実装。
  - timeout 時は `TaskCanceledException` で上流へ伝搬。
- `src/Transport/Protocol/RestClient.cs`
  - `TaskCanceledException` を `TransportException("timed out or was canceled")` へ統合。
  - 呼び出し元キャンセルと client timeout の区別なし。

## 3) 例外変換（Wire/Raw/Normalized/Adapter/Contracts）

- Transport 例外生成:
  - `src/Transport/Protocol/RestClient.cs`
  - `src/Exchanges/Bitflyer/Raw/Internal/RawJson.cs`
  - `src/Exchanges/Bittrade/Raw/Internal/RawJson.cs`
- Wire で Call 化:
  - `src/Transport/Wire/WireTransport.cs`（`CallErrorKind.Transport`）
- Adapter 側分類:
  - `src/Exchanges/Bitflyer/Adapter/Internal/Mappers/ErrorMapper.cs`
  - `src/Exchanges/Bittrade/Adapter/Internal/Mappers/ErrorMapper.cs`

## 4) 既存 batch / partial failure パターン

- Contracts DTO に `Completeness` / `PartialReason` は存在:
  - `src/Contracts/Common/Dtos/Completeness.cs`
  - `src/Contracts/Common/Dtos/PartialReason.cs`
- ただし、これは単一 endpoint の「取得件数制約による欠落」表現であり、
  「複数呼び出しにおける成功/失敗混在」の公式結果型は未定義。
- `Task.WhenAll` による multi-call 集約の公開 API 実装は現時点で見当たらない。

## 5) 最小変更計画（規約 → 実装）

1. 429 規約
- 対応先: `src/Transport/Policy/RetryHttpPolicy.cs`
- 変更:
  - `Retry-After` 優先
  - fallback 指数バックオフ + ジッター
  - 最大試行回数 + 総リトライ時間上限

2. Timeout 規約
- 対応先:
  - `src/Transport/Policy/TimeoutHttpPolicy.cs`
  - `src/Transport/Protocol/RestClient.cs`
  - `src/Transport/Protocol/TransportException.cs`
- 変更:
  - client timeout と caller cancellation を分類
  - リトライ可否判定で区別可能にする

3. Partial Failure 規約
- 対応先:
  - `src/Contracts/Common/Dtos/`（新規型追加）
- 変更:
  - `BatchResult<TItem>` / `BatchError` を追加
  - 判定用ヘルパープロパティを追加
  - 既存 multi-call 公開 API が未確認のため、型セマンティクスを単体テストで固定

4. テスト固定
- 対応先:
  - `tests/Common.Tests/Transport/Policy/HttpPolicyTests.cs`
  - `tests/Common.Tests/Transport/RestClientFaultInjectionTests.cs`
  - `tests/Common.Tests/Contracts/`（新規 batch result テスト）
- 追加観点:
  - 429 Retry-After 優先
  - Retry-After なし時の指数 + ジッター範囲
  - 最大試行 / 総時間上限
  - timeout vs cancellation 分類
  - partial failure 判定
