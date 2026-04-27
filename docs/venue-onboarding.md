# Venue Onboarding Guide

最終更新: 2026-04-27
位置づけ: venue onboarding guide

本書は、新しい取引所 venue を ExchangeAPI に追加する前の準備、設計判断、実装 checklist を定義する。
v3.2.0 では v4.0.0 の new venue public read MVP に備えるための guide / template として扱う。

本書は実装指示ではない。
特定 venue の正式実装を始める場合は、対象 version の `docs/plan-vX.Y.Z.md` と venue 固有の endpoint matrix を先に作成する。

## 1. Onboarding Scope

v4.0.0 の new venue MVP は public read に限定する。

含めるもの:

- venue aggregate project / package
- `Vocabulary`
- public read `Protocol`
- public read `Native`
- `Composition` factory
- endpoint matrix
- deterministic tests
- opt-in safe live read verification
- local consumer smoke
- package / docs / release checklist

含めないもの:

- private endpoint
- order / cancel / deposit / withdraw
- state-changing operation
- Unified implementation
- Realtime API
- credentials provider 拡張
- error taxonomy 大変更
- public DTO / response shape の広範囲な cross-venue normalization

## 2. Candidate Evaluation

新 venue 候補は、実装前に次を評価する。

| Item | Question | Prefer |
| --- | --- | --- |
| API docs | Public read API の仕様が機械的に読めるか | endpoint / request / response / error が明示されている |
| Public read coverage | MVP に必要な market data があるか | markets, ticker, order book, executions, candles |
| Authentication | private を後回しにできるか | public read が credentials 不要 |
| Rate limit | safe live verification が組めるか | public read の短時間実行が許容される |
| Response shape | DTO を stable に固定できるか | top-level object / array が明確 |
| Timestamp | timezone / epoch / precision が説明されているか | UTC / epoch milliseconds などが明記 |
| Decimal values | price / size を decimal として扱えるか | string or number contract が安定 |
| Product code | market symbol の規則が明確か | canonical symbol と display name を区別できる |
| Error contract | HTTP status と venue error body が分かるか | error code / message が明示 |
| Live safety | public read only で確認できるか | credentials なし、state change なし |
| Maintenance | docs / API の変更頻度が許容できるか | breaking change が少ない |
| User value | 利用想定に合うか | JPY pair / major market / bot 利用価値がある |

評価結果は `docs/roadmap-post-v2.md` または対象 version plan に残す。

## 3. Required Documents

新 venue を正式に追加する場合、最低限次を用意する。

- `docs/endpoints-<venue>.md`
  - venue endpoint matrix
  - current rule
  - timestamp / decimal / symbol notes
- `docs/plan-vX.Y.Z.md`
  - scope / non-scope
  - implementation order
  - verification
  - completion criteria
- `docs/verification.md`
  - live verification risk handling が既存方針で足りない場合だけ更新
- `docs/local-nuget-consumer.md`
  - new venue package を consumer smoke 対象へ入れる場合だけ更新
- `docs/guides/package-publish.md`
  - publish / GitHub Packages smoke 対象に new venue を含める場合だけ更新
- `docs/document-inventory.md`
  - new venue matrix を keep に追加

README は入口と文書 map に留める。
exact contract は README に重複させない。

## 4. Endpoint Matrix Template

`docs/endpoints-<venue>.md` は次の構成を基本とする。

```markdown
# <Venue> Endpoint Matrix

最終更新: <yyyy-mm-dd>
位置づけ: <Venue> venue ledger

本書は、<Venue> venue の endpoint metadata、公開範囲、固定状況を管理する現行正本である。
library 共通原則は [`docs/spec.md`](./spec.md) を参照し、本書では <Venue> 固有の matrix と補助台帳だけを扱う。

## Values

<ExposeInProtocol / ExposeInNative / LiveTestPhase / RequestDtoStatus / ResponseDtoStatus / ExpectedStatus / ResponseShape / WritesState / CleanupPolicy / AliasPath / AuthType / OptionalOmissionRule の説明>

## Facade + Endpoint Module Rule

<既存 venue と同じ rule を記載>

## Matrix

| EndpointId | Method | Path | Scope | ExposeInProtocol | ExposeInNative | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus | ExpectedStatus | ResponseShape | WritesState | CleanupPolicy | AliasPath | AuthType | OptionalOmissionRule |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetMarkets | GET | /... | public | Yes | Yes | Phase1-Read | Transitional | Transitional | 200 | Array | No | None | - | None | - |

## Current Rule

<MVP 対象 endpoint と non-scope を記載>

## Timestamp / Decimal / Symbol Notes

<venue 固有の解釈を記載>
```

`TBD` の扱い:

- `ExposeInProtocol = Yes` または `ExposeInNative = Yes` の row に `ExpectedStatus` / `ResponseShape` / `AuthType` の `TBD` を残さない
- `ExposeInNative = Yes` の row に `OptionalOmissionRule` の `TBD` を残さない
- `TBD` は未公開 row にだけ許容する

## 5. Project / Folder Checklist

v3.0.0 以降、venue は 1 project / 1 package とする。

想定配置:

```text
src/Exchanges/<Venue>/
  ExchangeApi.Exchanges.<Venue>.csproj
  Vocabulary/
  Protocol/
    Public/
      Endpoints/
  Native/
    Public/
      Endpoints/
  Composition/
```

必須:

- aggregate project は `ExchangeApi.Primitives` のみを project reference する
- optional project は venue aggregate project から参照しない
- `Protocol` / `Native` / `Composition` / `Vocabulary` は folder / namespace / tests 上の設計境界とする
- `Protocol` は `Native` に依存しない
- `Native` は `Composition` に依存しない
- `Composition` だけが concrete wiring を所有する
- HTTP endpoint module と Realtime module を混ぜない

## 6. Implementation Checklist

public read endpoint ごとの基本手順:

1. endpoint matrix に row を追加する
2. request / response DTO status を `Transitional` で開始する
3. `Protocol/Public/Endpoints/<EndpointName>/` に request encode と transport call を置く
4. `Native/Public/Endpoints/<EndpointName>/` に decode と native DTO を置く
5. `Composition` factory へ wiring を追加する
6. deterministic tests を追加する
7. safe live read test を opt-in only で追加する
8. matrix の `ExpectedStatus`, `ResponseShape`, `AuthType`, `OptionalOmissionRule` と実装を照合する
9. DTO / timestamp / decimal が安定したら `Fixed` へ上げる

やらないこと:

- matrix 更新なしに public surface を追加しない
- endpoint DTO に cross-venue semantic normalization を入れない
- private credentials を public read MVP に持ち込まない
- CLI / MCP の本格 integration を venue 初期 MVP の blocker にしない

## 7. Deterministic Test Template

追加する test taxonomy:

```text
tests/Exchanges/<Venue>/Architecture.Tests/
tests/Exchanges/<Venue>/Protocol.Tests/
tests/Exchanges/<Venue>/Native.Tests/
tests/Exchanges/<Venue>/Composition.Tests/
tests/Exchanges/<Venue>/LiveTests/
```

最低限の deterministic tests:

- aggregate project reference rule
- layer dependency rule
- request query / body encode
- success response decode
- error response classification
- timestamp decode
- decimal decode
- optional query omission
- factory wiring
- DTO fixed contract

Architecture tests:

- venue aggregate project は `ExchangeApi.Primitives` のみ参照する
- optional project を venue aggregate project から参照しない
- `Protocol` namespace は `Native` namespace に依存しない
- `Native` namespace は `Composition` namespace に依存しない
- external adapter を `src/Exchanges/<Venue>/` 配下に置かない

## 8. Safe Live Read Verification Template

live test は opt-in only とする。

実行条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
```

public read MVP では credentials を要求しない。

確認項目:

- endpoint が短時間で成功する
- response shape が matrix と一致する
- timestamp / decimal の decode が deterministic tests と矛盾しない
- stdout / stderr / logs / evidence に secret が出ない
- opt-in なしでは skip する

evidence を残す場合:

```text
local/evidence/local-live/<yyyymmdd>-vX.Y.Z-<venue>-public-read/
  runtime/
    artifacts/
    logs/
  notes/
```

## 9. Package / Smoke / Release Checklist

package:

- `ExchangeApi.Exchanges.<Venue>` package が生成される
- layer-specific venue package は生成しない
- optional package を venue package の必須依存にしない

local consumer smoke:

- local NuGet feed から restore できる
- venue factory を参照できる
- public read request / DTO を参照できる
- output は secret-free である

GitHub Packages smoke:

- publish 後に restore / build / run できる
- token を stdout / stderr に出さない
- temp directory を使い、終了時に削除する

release:

- deterministic tests passed
- package generation passed
- local consumer smoke passed
- live tests skip safely without opt-in
- GitHub Packages consumer smoke passed
- release notes に new venue MVP scope / non-scope を記載する

## 10. v4 Candidate Comparison Template

候補比較は、正式実装前に次の表で残す。

| Venue | Public read coverage | Docs quality | Live safety | Auth complexity later | Timestamp clarity | Error clarity | User value | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| TBD | TBD | TBD | TBD | TBD | TBD | TBD | TBD | TBD |

採用判断は、実装しやすさだけでなく、長期保守、live verification safety、利用価値を含めて行う。
