# Endpoint History And Examples

最終更新: 2026-04-22  
位置づけ: アーカイブ補助文書

本書は、venue endpoint 文書から切り出した実装順、初期ルール、代表 contract 例を履歴として残す。  
現行の venue 台帳正本は [`../endpoints-bitflyer.md`](../endpoints-bitflyer.md) と [`../endpoints-binance.md`](../endpoints-binance.md) を参照する。

## 1. bitFlyer 実装順メモ

- `GetMarkets` で public top-level array response の基準形を作る
- `GetTicker` で public object response の基準形を作る
- `GetBoard` を追加し、public object with nested array response の形を固定する
- `GetExecutionsPublic` を追加し、public paging/filter array response の形を固定する
- `GetBalance` を追加し、private top-level array response の基準形を作る
- `GetCollateral` / `GetCollateralAccounts` を追加し、private object と private array の空 request read endpoint を固定する
- `GetChildOrders` / `GetExecutionsPrivate` / `GetCollateralHistory` を追加し、paging/filter を持つ private read endpoint の形を固定する
- `GetPositions` を追加し、required query を持つ private read endpoint の形を固定する
- `GetTradingCommission` を追加し、required query + object response の単純 private read endpoint を固定する
- `SendChildOrder` / `CancelChildOrder` を追加し、body encode を持つ write endpoint の形を固定する
- `CancelAllChildOrders` を追加し、body encode + `Unit` response の destructive write endpoint を固定する

## 2. bitFlyer 代表 contract 例

### GetMarkets

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetMarketsCallAsync(CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsCallAsync(GetMarketsRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `GetMarketsRequest` は空 DTO
  - JSON body なし
- response DTO
  - top-level array
  - `GetMarkets.Item`
    - `ProductCode: string`
    - `MarketType: string`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetBoard

- public object with nested arrays の代表例として使っていた
- exact contract は当時の実装確認用メモであり、現行の詳細正本は matrix と code/test で追う

## 3. Binance 初期ルール

- Binance 初期 slice では `GetKlines` だけを扱う
- public read endpoint の template として `Phase1-Read` に置く
- `GetKlines` は Binance 初期 slice の read contract として `Fixed` に上げる

## 4. Binance `GetKlines` 代表 contract 例

- vocabulary
  - convenience 用の known symbol 定数を用意してよい
  - known interval 定数を用意してよい
  - `Symbol` の known values 定数は convenience 用であり、closed set の正本として扱わない
- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetKlinesCallAsync(string symbol, string interval, long? startTime = null, long? endTime = null, string? timeZone = null, int? limit = null, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesCallAsync(GetKlinesRequest request, CancellationToken cancellationToken = default)`
- request rule
  - `Symbol` 必須、blank 不可
  - `Interval` 必須
  - `Limit` は `1..1000`
  - `StartTime <= EndTime`
  - `TimeZone = null` のとき query omitted
- response rule
  - top-level shape は `Array`
  - 各 item の shape は `Array`
  - 各 item は 12 要素前提
  - tuple length 不一致、index kind 不一致、required scalar parse failure は `Codec`

## 5. bitFlyer 詳細 contract 例の退避方針

- 2026-04-22 の文書再編で、bitFlyer venue 台帳から endpoint ごとの詳細 request/response 例を外した
- 現行正本は [`../endpoints-bitflyer.md`](../endpoints-bitflyer.md) の matrix、current rule、timestamp ledger とする
- 詳細 contract 例を再導入する場合は、まず code/test と現行 matrix を照合してから archive へ再構成する
- 退避対象だった endpoint 群:
  - `GetTicker`
  - `GetBalance`
  - `GetCollateral`
  - `GetCollateralAccounts`
  - `GetChildOrders`
  - `GetExecutionsPrivate`
  - `GetPositions`
  - `GetCollateralHistory`
  - `GetTradingCommission`
  - `Withdraw`
  - `SendChildOrder`
  - `CancelChildOrder`
  - `SendParentOrder`
  - `GetParentOrders`
  - `GetParentOrder`
  - `CancelParentOrder`
  - `CancelAllChildOrders`
