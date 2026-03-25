# Stage10 Endpoint Matrix — Binance

本書は、Binance の Stage10 実装に対する endpoint 運用正本である。  
本書は Stage10 の Binance slice における実装対象、DTO 固定状況、live test 導入順、endpoint metadata を自己完結に管理する。

現在の Stage10 コード配置は本書の従属物であり、判断根拠にはしない。  
削除済み inventory や他の補助文書を前提にせず、本書自身を endpoint 正本として扱う。

## Values

- `ExposeInProtocol`
  - `Yes`: Stage10 で `Protocol` 公開面に含める
  - `Later`: 後段で扱う
- `ExposeInNative`
  - `Yes`: Stage10 で `Native` 公開面に含める
  - `Later`: 後段で扱う
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
  - `NotSupported`: Stage10 では write live test 対象にしない
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
  - facade に `*CallAsync(...)` の endpoint-level method を公開する
  - 対応する独立 module class を `Protocol/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
- `ExposeInNative = Yes`
  - facade に `*CallAsync(...)` の native call method を公開する
  - 対応する独立 module class を `Native/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
  - request DTO と response DTO は同 endpoint フォルダへ寄せてよい

## Matrix

| EndpointId | Method | Path | Scope | ExposeInProtocol | ExposeInNative | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus | ExpectedStatus | ResponseShape | WritesState | CleanupPolicy | AliasPath | AuthType | OptionalOmissionRule |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetKlines | GET | /api/v3/klines | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | ArrayOfArrays | No | None | - | None | startTime/endTime/timeZone/limit = null は query omitted |

## Initial Rule

- Stage10 の Binance 初期 slice では `GetKlines` だけを扱う
- public read endpoint の template として `Phase1-Read` に置く
- `GetKlines` は Binance 初期 slice の read contract として `Fixed` に上げる

## Initial Endpoint Contract

### GetKlines

- vocabulary
  - `BinanceSymbols.BtcJpy = "BTCJPY"` を唯一の初期定数として用意してよい
  - request DTO の `Symbol` 自体は `string` のまま持つ
- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetKlinesCallAsync(string symbol, string interval, long? startTime = null, long? endTime = null, string? timeZone = null, int? limit = null, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesCallAsync(GetKlinesRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `Symbol: string`
  - `Interval: string`
  - `StartTime: long?`
  - `EndTime: long?`
  - `TimeZone: string?`
  - `Limit: int?`
- request rule
  - `Symbol` 必須、blank 不可
  - `Interval` 必須、case-sensitive
    - `1s`
    - `1m`, `3m`, `5m`, `15m`, `30m`
    - `1h`, `2h`, `4h`, `6h`, `8h`, `12h`
    - `1d`, `3d`
    - `1w`
    - `1M`
  - `Limit` は `1..1000`
  - `StartTime` と `EndTime` が両方ある場合は `StartTime <= EndTime`
  - `TimeZone = null` のとき query omitted
  - `TimeZone` がある場合は
    - hour-only または hour-minute offset 文字列を許可する
    - 例: `0`, `8`, `4`, `-1:00`, `05:45`
    - 範囲は `-12:00` から `+14:00` inclusive
  - `StartTime` と `EndTime` は常に UTC として解釈する
- response DTO
  - top-level array
  - 各 item は tuple array
  - `GetKlines.Item`
    - `OpenTime: long`
    - `OpenPrice: decimal`
    - `HighPrice: decimal`
    - `LowPrice: decimal`
    - `ClosePrice: decimal`
    - `Volume: decimal`
    - `CloseTime: long`
    - `QuoteAssetVolume: decimal`
    - `NumberOfTrades: int`
    - `TakerBuyBaseAssetVolume: decimal`
    - `TakerBuyQuoteAssetVolume: decimal`
  - tuple index `11` の unused field は ignore してよい
- response rule
  - top-level shape は `Array`
  - 各 item の shape は `Array`
  - 各 item は 12 要素前提
  - tuple length 不一致、index kind 不一致、required scalar parse failure は `Codec`
- `ExpectedStatus = 200`
- `ResponseShape = ArrayOfArrays`
- `AuthType = None`
- `AliasPath = -`
