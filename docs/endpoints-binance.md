# Binance Endpoint Matrix

最終更新: 2026-04-22  
位置づけ: Binance venue ledger

本書は、Binance venue の endpoint metadata、公開範囲、固定状況を管理する現行正本である。  
library 共通原則は [`docs/spec.md`](./spec.md) を参照し、本書では Binance 固有の matrix と補助台帳だけを扱う。

現在のコード配置は本書の従属物であり、判断根拠にはしない。  
削除済み inventory や他の補助文書を前提にせず、本書自身を endpoint 正本として扱う。

注記:

- 本文中に残る `Stage10` は履歴ラベルであり、現行の優先順位は文書体系ガイドに従う
- 初期ルールや代表 contract 例は [`docs/archive/endpoint-history-and-examples.md`](./archive/endpoint-history-and-examples.md) に切り出して管理する

## Values

- `ExposeInProtocol`
  - `Yes`: 現行 Binance slice の `Protocol` 公開面に含める
  - `Later`: 現行 slice ではまだ公開しない
- `ExposeInNative`
  - `Yes`: 現行 Binance slice の `Native` 公開面に含める
  - `Later`: 現行 slice ではまだ公開しない
- `LiveTestPhase`
  - `Phase1-Read`: 第1段階の read live test 対象
  - `Phase2-Write`: 第2段階の write live test 対象
  - `Later`: 後段導入
- `RequestDtoStatus` / `ResponseDtoStatus`
  - `Transitional`: 最終固定前
  - `Fixed`: 最終固定済み
- `ExpectedStatus`
  - `200`: HTTP 200 を成功とする
  - `TBD`: 後段で確定する
  - `Native` が評価し、`Protocol` は raw status を保持する
- `ResponseShape`
  - `Array`: top-level array
  - `ArrayOfArrays`: top-level array、各 item も array
  - `Object`: top-level object
  - `TBD`: 後段で確定する
- `WritesState`
  - `Yes`: venue state を変更する
  - `No`: read-only
- `CleanupPolicy`
  - `None`: cleanup 不要
  - `Required`: live test 後に cleanup を必須とする
  - `NotSupported`: 現行 slice では write live test 対象にしない
- `AliasPath`
  - path alias がある場合はその path を書く
  - なければ `-`
- `AuthType`
  - `None`: 認証不要
  - `ApiKey`: API key header のみ
  - `KeySecret`: API key / secret による signed private 認証
  - `TBD`: 後段で確定する
- `OptionalOmissionRule`
  - `-`: omission rule なし
  - `TBD`: 後段で確定する
  - 条件付き omission がある場合は簡潔に記述する

`TBD` の許容条件:

- `ExposeInProtocol = Yes` または `ExposeInNative = Yes` の row に、`ExpectedStatus` / `ResponseShape` / `AuthType` の `TBD` を残さない
- `ExposeInNative = Yes` の row に、`OptionalOmissionRule` の `TBD` を残さない
- `TBD` は `ExposeInProtocol != Yes` かつ `ExposeInNative != Yes` の row にのみ許容する

## Facade + Endpoint Module Rule

- `ExposeInProtocol = Yes`
  - facade に `*Async(...)` の endpoint-level method を公開する
  - 対応する独立 module class を `Protocol/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
- `ExposeInNative = Yes`
  - facade に `*Async(...)` の native call method を公開する
  - 対応する独立 module class を `Native/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
  - request DTO と response DTO は同 endpoint フォルダへ寄せてよい

## Matrix

| EndpointId | Method | Path | Scope | ExposeInProtocol | ExposeInNative | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus | ExpectedStatus | ResponseShape | WritesState | CleanupPolicy | AliasPath | AuthType | OptionalOmissionRule |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetKlines | GET | /api/v3/klines | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | ArrayOfArrays | No | None | - | None | startTime/endTime/timeZone/limit = null は query omitted |

## Current Rule

- 現行 Binance slice では `GetKlines` だけを扱う
- public read endpoint の template として `Phase1-Read` に置く
- `GetKlines` は Binance 現行 slice の read contract として `Fixed` に上げる

## Representative Contract Notes

代表 contract 例、初期ルール、`GetKlines` の詳細な facade/request/response 例は [`docs/archive/endpoint-history-and-examples.md`](./archive/endpoint-history-and-examples.md) を参照する。
