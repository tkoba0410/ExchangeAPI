# Stage10 Implementation Instructions

## 1. Purpose

この指示書は、Stage10 実装を一気に進めるための実行用 brief である。  
設計判断の正本は [`stage10.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/stage10.md)、endpoint metadata の正本は [`stage10/endpoints-bitflyer.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/stage10/endpoints-bitflyer.md) とする。  
この brief はそれらを実装指示へ落としたものであり、判断が衝突した場合は `stage10.md` と matrix を優先する。

## 2. Source Of Truth

- 設計正本: [`stage10.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/stage10.md)
- endpoint 運用正本: [`stage10/endpoints-bitflyer.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/stage10/endpoints-bitflyer.md)
- 既存 `src-stage10` / `tests-stage10` は流用材料であり、構成の正本ではない

## 3. Scope

今回の対象は以下の 4 endpoint に限定する。

- `GetTicker`
- `GetBalance`
- `SendChildOrder`
- `CancelChildOrder`

対象 exchange は bitFlyer のみとする。  
`Unified`、`McpServer`、他 exchange 展開は対象外とする。

## 4. Target Architecture

実装は必ず `Facade + Endpoint Module` に寄せる。

- facade は薄い forward のみ
- endpoint ごとに独立 class を持つ
- `Protocol` は bitFlyer execution runtime
- `Native` は bitFlyer-native contract
- `Call` を唯一の返却形式とする

## 5. Mandatory Rules

### 5.1 Public Surface

- facade の主公開面は `*CallAsync(...)` とする
- `Protocol` facade は `Task<Call<WireCallSpec, WireResponse>>` を返す
- `Native` facade は `Task<Call<TRequest, TResponse>>` を返す
- DTO 直返し wrapper は実装しない

### 5.2 Protocol

- `Protocol` は method / path / query / body / canonical request / send / raw status を担当する
- `Protocol` は HTTP response を受け取った時点で `WireResponse` を返す
- `Protocol` は status code を `Http` error に変換しない
- `Protocol` endpoint に共通 interface は置かない
- `Protocol` endpoint は endpoint-specific interface を持つ

### 5.3 Native

- `Native` は request DTO / response DTO / request validation / request encode / protocol call / response decode / ContractValidation を担当する
- `ExpectedStatus` は `Native` が評価する
- `Native` call は child `Protocol` call を必ず保持する
- `MeaningValidation` という語は使わない
- validation stage は `JsonValidation -> Conversion -> ContractValidation` に統一する

### 5.4 Error Rules

- `Transport`: HTTP response 前の送信失敗
- `Http`: non-success status または expected status 不一致
- `Codec`: parse / shape / required raw field / scalar decode 失敗
- `Semantic`: request 条件違反または API contract rule violation
- `Mapping`: Stage10 初期実装では原則使わない

### 5.5 Physical Layout

以下の形へ寄せる。

```text
src-stage10/Bitflyer/
  Protocol/
    Public/Api/
    Public/Endpoints/GetTicker/
    Private/Api/
    Private/Endpoints/GetBalance/
    Private/Endpoints/SendChildOrder/
    Private/Endpoints/CancelChildOrder/
    Internal/Auth/
    Internal/Runtime/
    Internal/Shared/
  Native/
    Public/Api/
    Public/Endpoints/GetTicker/
    Private/Api/
    Private/Endpoints/GetBalance/
    Private/Endpoints/SendChildOrder/
    Private/Endpoints/CancelChildOrder/
    Internal/Shared/
```

### 5.6 Prohibited

- facade へ endpoint 実装を直接書くこと
- `partial` 前提で facade を肥大化させること
- `Native/Internal/Encoder` / `Conversion` / `MeaningValidation` の中央集約構成を正本として維持すること
- `Protocol` から `Native` へ依存すること
- `Native` から shared transport runtime へ直接依存すること

## 6. Endpoint-Specific Requirements

matrix に従い、初期 4 endpoint の metadata を固定して実装する。

### 6.1 GetTicker

- `Protocol`: `GET /v1/getticker`
- alias path: `/v1/ticker`
- auth: `None`
- expected status: `200`
- response shape: `Object`
- omission rule: `product_code = null` は query omitted

### 6.2 GetBalance

- `Protocol`: `GET /v1/me/getbalance`
- auth: `KeySecret`
- expected status: `200`
- response shape: `Array`
- omission rule: `-`

### 6.3 SendChildOrder

- `Protocol`: `POST /v1/me/sendchildorder`
- auth: `KeySecret`
- expected status: `200`
- response shape: `Object`
- cleanup policy: `Required`
- omission rule:
  - `minute_to_expire = null` は omitted
  - `time_in_force = null` は omitted
  - `price` は conditional required

### 6.4 CancelChildOrder

- `Protocol`: `POST /v1/me/cancelchildorder`
- auth: `KeySecret`
- expected status: `200`
- response shape: `EmptyOrObject`
- cleanup policy: `None`
- omission / rule:
  - `child_order_id` と `child_order_acceptance_id` は exactly one

## 7. Implementation Order

以下の順で止まらずに進める。

1. `Protocol` の `GetTicker` を facade + endpoint module に移す
2. `Native` の `GetTicker` を同じ形へ移す
3. `GetBalance` を移す
4. `SendChildOrder` を移す
5. `CancelChildOrder` を移す
6. `Composition` を新構成へ合わせる
7. tests を facade / endpoint module / composition に役割分離する
8. 不要になった旧構成を整理する

## 8. Test Requirements

### 8.1 Protocol Tests

各 endpoint で以下を検証する。

- method
- path
- query/body
- canonical request
- transport failure
- raw status の保持

### 8.2 Native Tests

各 endpoint で以下を検証する。

- request semantic rule
- omission rule
- response pipeline
- expected status 判定
- error kind 分類
- child protocol call の保持

### 8.3 Facade Tests

- thin forward のみを検証する

### 8.4 Composition Tests

- `Public` / `Private` bundle 構成
- credential 有無
- protocol/native の配線

### 8.5 Live Tests

- read parity test は `GetTicker` と `GetBalance` のみ
- write parity で二重送信しない
- write live test は optional
- `CleanupPolicy = Required` の endpoint は cleanup を同一 test に含める
- `CleanupPolicy = NotSupported` の endpoint は live write 対象にしない

## 9. Migration Rule

- 現在のコードは破棄前提ではなく、流用可能な中身だけ移す
- ただし file 配置、partial 構成、旧命名は引き継がない
- 既存 helper は、新しい endpoint module に安全に移せる場合だけ再利用する

## 10. Completion Criteria

完了時には以下を満たすこと。

- 初期 4 endpoint が `Protocol` / `Native` 新構成で実装されている
- facade の主公開面が `*CallAsync(...)` に揃っている
- `MeaningValidation` が source から消えている
- `Protocol` endpoint-specific interface が導入されている
- `Native` call が child `Protocol` call を保持している
- tests が新構成に追随している
- `dotnet test` の関連 project が通る

## 11. Final Report

完了時は以下を簡潔に報告する。

- 実装した範囲
- まだ未実装の範囲
- 実行したテスト
- 残リスク
