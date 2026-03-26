# CLI（Stage11 現行仕様）

最終更新: 2026-03-27  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は Stage11 における CLI adapter の現行仕様である。  
library 本体の設計正本は [`docs/spec.md`](./spec.md) に置き、  
endpoint ごとの公開範囲と contract metadata の正本は以下に置く。

- [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md)
- [`docs/endpoints-binance.md`](./endpoints-binance.md)

CLI は library surface を terminal 向けに写像する adapter であり、  
本書では current branch で固定できる内容だけを SSOT として定義する。

## 2. 責務

CLI は以下を所有する。

- command tree
- request input の受け取りと facade 呼び出しへの束縛
- credentials / option の読み込み
- stdout / stderr の分離
- exit code
- write command の確認 UX

CLI は以下を所有しない。

- endpoint 実装
- transport / signer / runtime
- native contract / protocol contract の定義
- exchange 固有 business rule
- retry / rate limiting / fallback policy
- state 保持

## 3. 依存規約

```text
CLI -> Composition -> Native | Protocol
```

- CLI は `Composition` だけに依存する
- CLI から concrete endpoint / runtime / signer / transport を直接配線してはならない
- 1 command は 1 回の facade call に対応する
- CLI は stateless な単発実行を前提とする
- `native` と `protocol` は明示選択であり、CLI が暗黙に切り替えてはならない
- `Unified` は未実装のため、現行 CLI 仕様には含めない

## 4. コマンドモデル

### 4.1 基本形

```text
exchangeapi <venue> <surface> <scope> <command> [options]
```

### 4.2 固定 token

- `venue`
  - `bitflyer`
  - `binance`
- `surface`
  - `native`
  - `protocol`
- `scope`
  - `public`
  - `private`

### 4.3 command 名

- command 名は endpoint matrix の `EndpointId` を kebab-case 化したものとする
- command の存在可否は endpoint matrix の公開設定から導出する
  - `native`: `ExposeInNative = Yes`
  - `protocol`: `ExposeInProtocol = Yes`
- `scope` は endpoint matrix の `Scope` と一致しなければならない

例:

- `GetTicker` -> `get-ticker`
- `GetExecutionsPublic` -> `get-executions-public`
- `CancelAllChildOrders` -> `cancel-all-child-orders`

### 4.4 現行公開範囲

- bitFlyer
  - `native public`
  - `native private`
  - `protocol public`
  - `protocol private`
- Binance
  - `native public`
  - `protocol public`

現行 command inventory の正本は endpoint matrix とし、本書で重複列挙しない。

## 5. 入力契約

### 5.1 `native`

- `native` command の入力は対応する request DTO の JSON とする
- 入力経路は以下のいずれかとする
  - `--request-json <json>`
  - `--request-file <path>`
- `--request-file -` は stdin を意味する
- empty request DTO の endpoint は request input 省略を許可してよい
- JSON field 名は DTO の serialization contract に従う
  - `JsonPropertyName` がある場合はその名前
  - ない場合は DTO property 名

### 5.2 `protocol`

- `protocol` command は raw request を扱う
- query-only endpoint は以下のいずれかで query object を受け取る
  - `--query-json <json>`
  - `--query-file <path>`
- body を持つ endpoint は以下のいずれかで body text を受け取る
  - `--body-json <json>`
  - `--body-file <path>`
- `--query-file -` と `--body-file -` は stdin を意味する
- query object の key は exchange API の query parameter 名に合わせる
- body は exchange へ送る JSON text をそのまま受け取る
- `protocol` command は native DTO decode や contract validation を行わない

## 6. 出力契約

### 6.1 基本原則

- 現行 phase で固定する出力形式は `json` のみとする
- stdout は成功時データのみを出す
- stderr は診断メッセージのみを出す
- stdout に説明文や装飾文字列を混ぜてはならない

### 6.2 成功時

- `native` command は native `Response` を JSON で出力する
- `protocol` command は `ProtocolResponse` を JSON で出力する
- JSON は単一 document とし、pipe 処理可能でなければならない
- decimal / int / bool を string 化してはならない
- `ProtocolResponse.BodyText` は raw text を保持する string として出力する

### 6.3 `native` と `protocol` の意味

- 正規化済み contract を見たい場合は `native` を使う
- raw response を見たい場合は `protocol` を使う
- 現行 phase では `--raw` / `--normalized` の二重出力は固定しない
- raw / normalized の切り替えは surface 選択で表現する

## 7. 認証と設定

### 7.1 認証

- API key / secret を CLI 引数で受け取ってはならない
- 現行 phase で固定する credentials input は環境変数のみとする
  - `BITFLYER_API_KEY`
  - `BITFLYER_API_SECRET`
- bitFlyer private command は credentials を解決できない場合、facade call 前に失敗させる
- Binance は現行公開範囲に private surface を持たない

### 7.2 option

現行 phase で固定する option は、`Composition` 直結のものと CLI 固有のものに分ける。

`Composition` 直結:

- `--base-uri <absolute-uri>`
- `--timeout-ms <int>`
- `--enable-protocol-debug-log`
- `--protocol-debug-log-dir <path>`

CLI 固有:

- `--verbose`
- `--yes`

bitFlyer 固有 option:

- `--use-ticker-alias-path`

### 7.3 優先順位

- CLI option > environment variable
- 現行 phase では config file 契約を固定しない

## 8. 安全制約

- write command の判定は endpoint matrix の `WritesState = Yes` を正本とする
- write command は `--yes` なしに non-interactive 実行してはならない
- interactive 実行では確認 prompt を出してよい
- 利用者が確認を拒否した場合、facade call を行ってはならない
- 現行 phase では generic `dry-run` 契約を持たない
- 現行 phase では generic `client-order-key` 契約を持たない
- ログや error message に secret を出してはならない

## 9. Exit Code

| code | 内容 |
| --- | --- |
| 0 | success |
| 1 | unexpected internal error |
| 2 | argument / config / safety error |
| 3 | facade call failure |

補足:

- `facade call failure` は `Call.IsSuccess = false` を意味する
- `--verbose` 指定時は stderr に `CallError.Kind` と endpoint 情報を追加してよい

## 10. Help

- help は階層的でなければならない
- 各階層で usage 例を少なくとも 1 つ示す
- write command の help では `--yes` 要件を明示する

## 11. 現行非スコープ

以下は旧草案から intent は引き継ぐが、現行 branch では仕様固定しない。

- `Unified` command tree
- `normalize`
- `parity-check`
- `replay`
- `table` 出力
- `production|sandbox` の抽象 `--env`
- generic `dry-run`
- generic idempotency key

## 12. 例

bitFlyer native public:

```bash
exchangeapi bitflyer native public get-ticker \
  --request-json '{"product_code":"BTC_JPY"}'
```

bitFlyer protocol public:

```bash
exchangeapi bitflyer protocol public get-ticker \
  --query-json '{"product_code":"BTC_JPY"}'
```

Binance native public:

```bash
exchangeapi binance native public get-klines \
  --request-json '{"Symbol":"BTCJPY","Interval":"1h","Limit":2}'
```

bitFlyer private write:

```bash
exchangeapi bitflyer native private cancel-all-child-orders \
  --request-json '{"product_code":"BTC_JPY"}' \
  --yes
```
