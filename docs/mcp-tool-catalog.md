# MCP Tool Catalog

最終更新: 2026-04-22  
位置づけ: MCP tool ledger

本書は、MCP Server が公開する tool surface と tool ごとの契約を台帳として管理する。  
MCP Server 全体の責務、依存、動作モードは [`docs/mcp-server.md`](./mcp-server.md) を正本とする。

## 1. Current Tool Surface

tool surface は次の 2 層で管理する。

- `Core Bot Tools`
  - bot / LLM 本番導線で使う責務単位 tool
- `Inspection Read Tools`
  - 開発中確認と運用 inspection のための read-only tool

現行実装で visible な tool universe は [`2. Visible Tools`](#2-visible-tools) に列挙した tool だけである。
`Inspection Read Tools` は v2.1.0 から一部実装する。

- bitFlyer v1 core bot tool は `get_market_snapshot`、`get_account_snapshot`、`evaluate_order`、`evaluate_margin_order` の 4 つとする
- 初期 venue scope は bitFlyer を正本とする
- Binance など他 venue の account / evaluation 展開は、market rule / account / evaluation の導出元が固定できてから行う
- Binance public market data は例外として、`get_klines` を public read 拡張として追加してよい
- `list_markets` を market discovery tool として追加してよい
- `GetCollateralAccounts`、`GetBalanceHistory`、`GetCollateralHistory`、`GetChildOrders` は v2.1.0 の inspection read tool として visible surface に含める

### 1.1 bitFlyer v1 support matrix

- `get_market_snapshot`: `BTC_JPY`、`FX_BTC_JPY`
- `get_account_snapshot`: symbol input なし。spot balance と `FX_BTC_JPY` position を返す
- `evaluate_order`: `BTC_JPY` の `LIMIT` / `MARKET` child order のみ
- `evaluate_margin_order`: `FX_BTC_JPY` の `LIMIT` / `MARKET` child order のみ

補足:

- `get_account_snapshot.positions` は bitFlyer `GetPositions` の制約に従い、`FX_BTC_JPY` のみを対象とする
- spot 保有は `positions` ではなく `balance` に表現する
- `evaluate_order` は margin product を初期 scope に含めない

### 1.2 Binance public kline support matrix

- `get_klines`: public read only
- account snapshot は提供しない
- order evaluation は提供しない
- private credentials は不要

初期 support set:

- `BTCJPY`
- `ETHJPY`
- `XRPJPY`
- `BNBJPY`
- `BTCUSDT`
- `ETHUSDT`
- `SOLUSDT`
- `XRPUSDT`

### 1.3 bitFlyer private read expansion grouping

bitFlyer private read endpoint は、`Core Bot Tools` へ吸収するものと、`Inspection Read Tools` として独立追加するものを分けて扱う。

#### `get_account_snapshot` に吸収する候補

- `GetCollateralAccounts`
  - 通貨別の証拠金残高は account snapshot の自然な構成要素として扱ってよい
  - 既存の `margin.derivedAvailable` だけでは不足する確認用途を補える
  - ただし v2.0.0 の現行 `get_account_snapshot` schema にはまだ含めない

#### `Inspection Read Tools` として優先追加する候補

- `GetCollateralAccounts`
  - `get_collateral_accounts`
- `GetBalanceHistory`
  - `get_balance_history`
- `GetCollateralHistory`
  - `get_collateral_history`
- `GetExecutionsPrivate`
  - `get_private_executions`
- `GetChildOrders`
  - `get_child_orders`
- `GetParentOrders`
  - `get_parent_orders`
- `GetPositions`
  - `get_positions`
- `GetTradingCommission`
  - `get_trading_commission`

履歴系の扱い:

- `GetBalanceHistory` と `GetCollateralHistory` は、現在状態の要約ではなく時系列 inspection に属する
- したがって `get_account_snapshot` には吸収せず、独立した `Inspection Read Tool` として追加する

#### 既存 aggregate tool の内部利用に留めるもの

- `GetPermissions`
  - 現行は `accountReadiness` 導出の内部利用を正本とする

#### 当面追加しない候補

- `GetAddresses`
  - read-only だが bot 本番価値が低く、wallet destination 情報として感度も高い
- `GetBankAccounts`
  - read-only だが bot 本番価値が低く、出金先 metadata として感度が高い
- `GetCoinIns`
- `GetCoinOuts`
- `GetDeposits`
- `GetWithdrawals`
  - これらは入出金 lifecycle に属し、現行の「注文、キャンセル、入金出金を MCP 非対応とする」運用方針では当面スコープ外に置く
- `GetParentOrder`
  - 単一 parent order lookup は `get_parent_orders` を先に整備した上で必要性を再評価する

## 2. Visible Tools

1. `get_market_snapshot`
2. `list_markets`
3. `get_klines`
4. `get_account_snapshot`
5. `get_collateral_accounts`
6. `get_balance_history`
7. `get_collateral_history`
8. `get_child_orders`
9. `evaluate_order`
10. `evaluate_margin_order`

補足:

- 上記 10 個が現行実装の visible tool universe である
- `tools/list` は current process が実際に実行可能な visible tool set を返す
- `get_account_snapshot`、`get_collateral_accounts`、`get_balance_history`、`get_collateral_history`、`get_child_orders`、`evaluate_order`、`evaluate_margin_order` は private credentials を解決できない場合、`tools/list` から advertise しない
- `get_klines` は Binance public client が配線されている場合のみ advertise してよい

補足:

- `tools/list` は上記 tool から current process が実際に実行可能なものだけを返す
- `Inspection Read Tools` も同じ原則で current process が実際に実行可能な tool だけを返す
- ただし private credentials を解決できない場合、private inspection read tool は advertise してはならない
- private credentials の解決失敗は operator に通知してよいが、MCP client へは tool 非公開または structured error として表現する
- v2.1.0 の inspection response shape は `accounts` / `items` / `items` / `orders` で固定し、`venue` / `accountContext` は response に含めない

## 3. Common Rules

- tool input の数値は、価格、数量、金額に限り decimal string を使う
- count や boolean は JSON number / boolean を維持する
- timestamp は UTC の ISO 8601 string とする
- bitFlyer v1 private tools は `venue` と `accountContext` を明示 input とする
- `get_market_snapshot` は bitFlyer v1 固定の public tool とする
- Binance public `get_klines` は venue-explicit とし、tool input の `venue` は v1 では `binance` のみを許可する
- multi-venue を扱う tool は、hidden venue を増やさず `venue` または等価の account context を contract で明示する
- supported symbol は tool ごとに固定する
- market rule は runtime 推測で埋めてはならない

## 4. Registry And Config Rules

### 4.1 bitFlyer 共通導出ルール

- bitFlyer の公開文書と公開 API 文書を external source of truth とする
- `get_market_snapshot` と `evaluate_order` は adapter-owned の `BitflyerMarketRuleRegistry` を pinned operational config として利用する
- `BitflyerMarketRuleRegistry` の正本データは version 管理された data file とし、adapter code は loader / validator のみを持つ
- `BitflyerMarketRuleRegistry` に entry がない symbol は、MCP として未サポートとみなし `invalid_symbol` とする
- entry は version 管理対象とし、runtime observation から自動学習してはならない

### 4.2 `BitflyerMarketRuleRegistry` source hierarchy

1. 公式の公開文書に明示された定量値
2. 公式 API 文書の request / response contract
3. 上記で未公開の項目に限り、adapter-owned の明示設定値

### 4.3 bitFlyer v1 registry baseline

| symbol | minSize | sizeStep | priceStep | minSize source kind | minSize source ref | sizeStep source kind | sizeStep source ref | priceStep source kind | priceStep source ref | source note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `adapter_inferred` | `adapter://bitflyer-jpy-price-step.v1` | `minSize` と `sizeStep` は公式公開値、`priceStep` は adapter-owned 推論値 |
| `FX_BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `official_documented` | `https://bitflyer.com/pub/20241015-bitFlyerCryptoCFD-Minimum-Order-Change-en.pdf` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `adapter_inferred` | `adapter://bitflyer-jpy-price-step.v1` | `minSize` は 2024-10-21 以降の公式公開値、`sizeStep` は BTC 単位の公開値、`priceStep` は adapter-owned 推論値 |

### 4.4 Binance public kline support set

- `get_klines` の symbol support は adapter-owned の `BinanceKlineSymbolSet` を正本とする
- `BinanceSymbols` の known values 定数は convenience 用であり、MCP support set の正本ではない
- `get_klines.interval` は [`docs/endpoints-binance.md`](./endpoints-binance.md) の `GetKlines` fixed contract を正本とする
- `get_klines` は Binance の `timeZone` parameter を v1 では公開せず、UTC 固定で扱う

## 5. Tool Contracts

本節の `get_market_snapshot`、`list_markets`、`get_klines`、`get_account_snapshot`、`get_collateral_accounts`、`get_balance_history`、`get_collateral_history`、`get_child_orders`、`evaluate_order`、`evaluate_margin_order` は現行 tool contract である。
`get_parent_orders`、`get_private_executions`、`get_positions`、`get_trading_commission` は post-v2 の draft contract であり、実装されるまで現行 visible tool として扱わない。

### 5.1 `get_market_snapshot`

目的:

- 売買判断に必要な市場情報をまとめて取得する

入力:

```json
{
  "symbol": "BTC_JPY"
}
```

主要出力:

```json
{
  "symbol": "BTC_JPY",
  "bid": "12345000",
  "ask": "12346000",
  "last": "12345500",
  "timestamp": "2026-03-29T10:00:00Z",
  "rules": {
    "minSize": "0.001",
    "sizeStep": "0.00000001",
    "priceStep": "1"
  },
  "status": "active"
}
```

実装ルール:

- `symbol` は必須
- supported symbol のみ受け付ける
- 市場状態は library の venue-specific state から `active / halted / restricted / unknown` へ写像する
- `bid`、`ask`、`last`、`timestamp` は `GetTicker` を正本とする
- `status` は `GetBoardState.health` と `GetBoardState.state` の組み合わせで決定する
- `rules.*` の返却値は `BitflyerMarketRuleRegistry` pinned operational config から取る

### 5.2 `list_markets`

目的:

- current process が実際に提供できる market-specific capability を venue / symbol 単位で列挙する

入力:

```json
{}
```

実装ルール:

- `list_markets` は current process の visible tool set を基準に構成する
- market-specific でない `get_account_snapshot` は `capabilities` に含めない
- private credentials が無い場合は private capability を列挙しない

### 5.3 `get_account_snapshot`

目的:

- 売買判断に必要な口座情報を MVP 項目に限定して返す

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default"
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `balance` は `GetBalance` の `available` を通貨別 map へ正規化したものを正本とする
- `positions` は `GetPositions(product_code = FX_BTC_JPY)` を正本とする
- `margin.derivedAvailable` は `GetCollateral` 由来の導出値とする
- `accountReadiness` は `GetPermissions` による read capability 判定を正本とする

補足:

- `get_account_snapshot` は bot 向け aggregate tool として、口座の現在状態を要約して返す
- `GetCollateralAccounts` 由来の通貨別 collateral 残高は v2.1.0 では inspection tool `get_collateral_accounts` として独立提供する
- v2.0.0 の現行 `get_account_snapshot.margin` schema は `derivedAvailable` のみを持つ

### 5.3.1 `get_collateral_accounts`

目的:

- 通貨別の証拠金残高を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default"
}
```

主要出力:

```json
{
  "accounts": [
    {
      "currencyCode": "JPY",
      "amount": "5000000"
    },
    {
      "currencyCode": "BTC",
      "amount": "0.1"
    }
  ]
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `accounts` は `GetCollateralAccounts` response を secret-free な array へ写像したものを正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.2 `get_balance_history`

目的:

- 通貨別の残高変動履歴を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "currencyCode": "JPY",
  "count": 50,
  "before": null,
  "after": null
}
```

主要出力:

```json
{
  "items": [
    {
      "id": 1,
      "tradeDate": "2026-03-29T19:00:00+09:00",
      "eventDate": "2026-03-29T10:00:00Z",
      "productCode": "BTC_JPY",
      "currencyCode": "JPY",
      "tradeType": "BUY",
      "price": "12345000",
      "amount": "-10000",
      "quantity": "0.001",
      "commission": "0",
      "balance": "5000000",
      "orderId": "JRF..."
    }
  ]
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `currencyCode`, `count`, `before`, `after` は `GetBalanceHistory` request contract を正本とする
- `items` は `GetBalanceHistory` response を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.3 `get_collateral_history`

目的:

- 証拠金変動履歴を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "count": 50,
  "before": null,
  "after": null
}
```

主要出力:

```json
{
  "items": [
    {
      "id": 1,
      "currencyCode": "JPY",
      "change": "10000",
      "amount": "5010000",
      "reasonCode": "TRADE",
      "date": "2026-03-29T10:00:00Z"
    }
  ]
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `count`, `before`, `after` は `GetCollateralHistory` request contract を正本とする
- `items` は `GetCollateralHistory` response を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.4 `get_child_orders`

目的:

- child order 一覧を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "BTC_JPY",
  "count": 50,
  "before": null,
  "after": null,
  "childOrderState": null,
  "childOrderId": null,
  "childOrderAcceptanceId": null,
  "parentOrderId": null
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- filter input は `GetChildOrders` request contract を正本とする
- response `orders` は `GetChildOrders` response を secret-free な array へ写像したものを正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.5 `get_parent_orders` (post-v2 draft)

目的:

- parent order 一覧を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "BTC_JPY",
  "count": 50,
  "before": null,
  "after": null,
  "parentOrderState": null
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- filter input は `GetParentOrders` request contract を正本とする
- response `items` は `GetParentOrders` response を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.6 `get_private_executions` (post-v2 draft)

目的:

- private execution 一覧を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "BTC_JPY",
  "count": 50,
  "before": null,
  "after": null,
  "childOrderId": null,
  "childOrderAcceptanceId": null
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- filter input は `GetExecutions` request contract を正本とする
- response `items` は `GetExecutions` response を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.7 `get_positions` (post-v2 draft)

目的:

- margin position 一覧を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "FX_BTC_JPY"
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `productCode` は `GetPositions` request contract を正本とする
- response `items` は `GetPositions` response を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.3.8 `get_trading_commission` (post-v2 draft)

目的:

- symbol ごとの trading commission を inspection 用に取得する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "BTC_JPY"
}
```

主要出力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "productCode": "BTC_JPY",
  "commissionRate": "0.0015"
}
```

実装ルール:

- `venue` と `accountContext` は必須
- v1 では `venue = bitflyer`、`accountContext = default`
- `productCode` は `GetTradingCommission` request contract を正本とする
- `commissionRate` は `GetTradingCommissionResponse.commission_rate` を正本とする
- current process が private credentials を解決できない場合、`tools/list` から advertise してはならない

### 5.4 `evaluate_order`

目的:

- spot 注文要求が現在の市場、口座、制約の下で機械的に成立可能かを評価する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "symbol": "BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.3",
  "price": null
}
```

実装ルール:

- `canPlace = false` は tool-level error ではなく正常 response として返す
- 入力不正、upstream 取得失敗、想定外障害のみを tool-level error とする
- `referencePrice` は `market buy -> ask`、`market sell -> bid`、`limit -> input price` とする
- `sizeRuleOk` は `BitflyerMarketRuleRegistry` に対する適合で判定する
- `priceRuleOk` は `priceStep` 適合と正値条件で判定する
- `warnings` は closed set として扱う

### 5.5 `evaluate_margin_order`

目的:

- margin 注文要求が現在の市場、証拠金、維持率、制約の下で機械的に成立可能かを評価する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "symbol": "FX_BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.1",
  "price": null
}
```

実装ルール:

- `evaluate_order` は spot evaluator、`evaluate_margin_order` は margin evaluator として分離する
- `currentMaxLeverage` は `GetCorporateLeverage.current_max` を正本とする
- `currentKeepRate` は `GetCollateral.keep_rate` を正本とする
- `minimumKeepRate` は `bitflyer-margin-rules.v1.json` を正本とする

### 5.6 `get_klines`

目的:

- market observation に必要な OHLCV 時系列を public read で取得する

入力:

```json
{
  "venue": "binance",
  "symbol": "BTCUSDT",
  "interval": "1h",
  "startTime": null,
  "endTime": null,
  "limit": 200
}
```

実装ルール:

- `venue` は `binance` のみ
- `symbol` は `BinanceKlineSymbolSet` のみ
- `interval` は [`docs/endpoints-binance.md`](./endpoints-binance.md) の `GetKlines` fixed contract に従う
- `startTime` と `endTime` は strict RFC 3339 parse を行い、UTC に正規化する
- `candles` は `GetKlines` を正本とし、raw tuple array は named field object に正規化する

## 6. MCP `tools/call` `_meta`

`tools/call` の result object には payload 本体と分離した `_meta` を含める。

```json
{
  "content": [
    {
      "type": "text",
      "text": "{\"symbol\":\"BTC_JPY\"}"
    }
  ],
  "structuredContent": {
    "symbol": "BTC_JPY"
  },
  "_meta": {
    "schemaVersion": "exchangeapi.mcp.get_market_snapshot.v1",
    "dataVersion": "bitflyer-market-rules.v1",
    "degraded": false
  },
  "isError": false
}
```

ルール:

- `_meta` は tool payload 本体に混ぜず、MCP call result envelope に置く
- `schemaVersion` は tool contract version を表す
- `dataVersion` は pinned config / support set / permission model の version を表す
- `degraded = true` は partial success で意味が弱まっている場合のみ返す

## 7. Error Model

- tool-level error category は `validation_error / upstream_error / domain_error / internal_error`
- `evaluate_order` の機械的な不成立は、可能な限り正常 response の `canPlace = false` で表現する
- `GetPermissions` failure は `get_account_snapshot` では degraded success とし、`accountReadiness = unknown` に写像してよい
- credential failure は private account capability の unavailable として扱う
- call 時点の credential failure は `upstream_error` / `account_unavailable` とする
- credential failure の details は `credentialErrorKind`、`venue`、`provider`、`reason` を持つ
- credential failure を `_meta.degraded = true` だけで表現してはならない

## 8. Related Documents

- [`docs/mcp-server.md`](./mcp-server.md)
- [`docs/spec.md`](./spec.md)
- [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md)
- [`docs/endpoints-binance.md`](./endpoints-binance.md)
- [`docs/archive/adapter-status-and-history.md`](./archive/adapter-status-and-history.md)
