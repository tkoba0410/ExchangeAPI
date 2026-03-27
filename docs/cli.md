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

- canonical command tree
- 限定的な wizard
- 補助 shell の adapter 境界
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
- exchange state の保持

## 3. 依存規約

```text
CLI -> Composition -> Native | Protocol
```

- CLI は `Composition` だけに依存する
- CLI から concrete endpoint / runtime / signer / transport を直接配線してはならない
- 1 command は 1 回の facade call に対応する
- canonical CLI と wizard は stateless な単発実行を前提とする
- shell は process-local な選択状態を持ってよいが、実行時は canonical command descriptor を経由しなければならない
- `native` と `protocol` は明示選択であり、CLI が暗黙に切り替えてはならない
- `Unified` は未実装のため、現行 CLI 仕様には含めない

### 3.1 物理配置

- CLI project は `src/Adapters/Cli/ExchangeApi.Adapters.Cli.csproj` に置く
- CLI test project は `tests/Adapters/Cli.Tests/ExchangeApi.Adapters.Cli.Tests.csproj` に置く
- CLI は external adapter であり、`src/Exchanges/<Venue>/` 配下に置いてはならない
- CLI の direct project reference は venue ごとの `Composition` project に限定する
  - `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
  - `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`
- CLI から `Native` / `Protocol` / `Vocabulary` project を直接参照してはならない

推奨フォルダ構成:

```text
src/Adapters/Cli/
  ExchangeApi.Adapters.Cli.csproj
  Program.cs
  Commands/
    Bitflyer/
      Native/
        Public/
        Private/
      Protocol/
        Public/
        Private/
    Binance/
      Native/
        Public/
      Protocol/
        Public/
  Binding/
  Configuration/
  Formatting/
  Help/
  Safety/
  Infrastructure/
  Wizard/
  Shell/
```

補足:

- `Commands/` は command tree の物理写像として扱う
- `Wizard/` は限定的な対話補助を扱うが、独自の実行能力を持ってはならない
- `Shell/` は補助 UI を扱うが、独自の adapter 契約を持ってはならない
- venue 横断で重複が固まるまでは、追加の common project を先行導入しない

### 3.2 インターフェース階層

CLI adapter の interface tier は以下を正本とする。

#### Tier 1: Canonical CLI

- canonical non-interactive CLI を正式インターフェースとする
- automation、script、CI、pipe 前提の契約は canonical CLI のみに対して固定する
- stdout / stderr / exit code / option / input / output 契約の正本は canonical CLI とする
- command inventory の正本は endpoint matrix とし、canonical CLI がそれを terminal 向けに写像する

#### Tier 2: Wizard

- wizard は限定的な人間向け補助 interface とする
- wizard は selected command に対してのみ導入してよい
- wizard は canonical CLI に存在しない capability を追加してはならない
- wizard は最終的に canonical CLI と等価な request を生成しなければならない
- wizard の human-facing prompt / prose / layout は正式な automation 契約に含めない
- 現行 phase の最小 wizard は、selected command の scalar input を収集し、等価な canonical invocation を提示する形でよい

#### Tier 3: Shell

- shell は反復的な手動操作を軽くする補助 interface とする
- shell は正式インターフェースに含めない
- shell は venue / surface / scope などの process-local default を持ってよい
- shell の実行は canonical command descriptor を再利用しなければならない
- shell 独自の request / response 契約、独自 capability、独自 safety rule を導入してはならない
- 現行 phase の shell helper は venue / surface / scope の default 保持と `run` 委譲に限定する

#### TUI

- full-screen TUI は当面導入しない
- TUI は CLI adapter の current contract に含めない

## 4. コマンドモデル

本節は Tier 1 canonical CLI の command contract を定義する。

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

本節は Tier 1 canonical CLI の input contract を定義する。

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

### 5.3 人間向け入力補助

機械向け正本は JSON input のままとし、CLI はその上に lossless な人間向け利便層を追加してよい。

- common scalar field が中心の command には endpoint-specific な convenience flag を追加してよい
- convenience flag は canonical な request / query / body に一意に変換できなければならない
- nested object、array、conditional omission が多い複雑 command では canonical JSON input を主経路とする
- 同一 command で canonical JSON input と convenience flag を併用してはならない
- CLI は以下の template 補助を持ってよい
  - `--request-template`
  - `--query-template`
  - `--body-template`
- template 補助は canonical な JSON 雛形を stdout に出し、facade call を行わず exit code `0` で終了する

例:

- `get-ticker --product-code BTC_JPY`
- `get-klines --symbol BTCJPY --interval 1h --limit 2`
- `cancel-all-child-orders --product-code BTC_JPY --yes`

### 5.4 wizard

- wizard input は人間向け prompt であり、formal automation contract に含めない
- wizard は selected command の limited field だけを収集してよい
- wizard は canonical CLI と等価な request を生成しなければならない
- wizard が command line を提示する場合、canonical input option を使ってよい
  - 例: `--request-json`
- wizard は convenience flag のみで安定して表現できる単純 command から導入する
- 複雑 command、nested object、array、conditional omission が多い command は canonical CLI を優先し、wizard 導入を急がない

### 5.5 shell

- shell の入力体験は人間向け補助であり、formal automation contract に含めない
- shell の session default は canonical CLI の path token や option の短縮入力にのみ使ってよい
- shell は execution 時に canonical command descriptor へ解決しなければならない
- 現行 phase の shell built-in は以下に限定してよい
  - `help`
  - `show`
  - `use venue <value>`
  - `use surface <value>`
  - `use scope <value>`
  - `run <command> [options]`
  - `run <venue> <surface> <scope> <command> [options]`
  - `exit`
  - `quit`

## 6. 出力契約

本節は Tier 1 canonical CLI の output contract を定義する。

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

### 6.4 人間向け表示補助

- `--pretty` を持ってよい
- `--pretty` は stdout の JSON を整形するだけで、データ内容を変えてはならない
- `--summary` を持ってよい
- `--summary` は人間向け要約を stderr にのみ出してよい
- `--summary` は stdout の JSON 出力契約を変えてはならない
- `--summary` の最小内容は command identity と success / failure 判定とする
- `--verbose` 指定時は `--summary` の詳細版として追加診断を stderr に出してよい

### 6.5 wizard と shell

- wizard と shell の prompt / banner / prose は人間向け補助出力であり、formal automation contract に含めない
- wizard が canonical invocation を提示する場合、その提示内容は canonical CLI 契約と矛盾してはならない
- shell が結果を表示する場合でも、基礎となる実行結果の意味は canonical CLI と同一でなければならない

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
- `--pretty`
- `--summary`
- `--yes`
- `--request-template`
- `--query-template`
- `--body-template`

bitFlyer 固有 option:

- `--use-ticker-alias-path`

command-specific convenience flag:

- 例: `--product-code`
- 例: `--symbol`
- 例: `--interval`
- 例: `--limit`

### 7.3 優先順位

- CLI option > environment variable
- 現行 phase では config file 契約を固定しない

## 8. 安全制約

- write command の判定は endpoint matrix の `WritesState = Yes` を正本とする
- write command は `--yes` なしに non-interactive 実行してはならない
- interactive 実行では確認 prompt を出さなければならない
- interactive 判定は少なくとも stdin と stderr が TTY であることを条件としてよい
- 確認 prompt では command identity と送信対象 request の要約を表示しなければならない
- 利用者が確認を拒否した場合、facade call を行ってはならない
- 利用者が確認を拒否した場合は exit code `2` で終了する
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

### 9.1 stderr 契約

- failure 時の stderr 1 行目は短い人間可読 summary でなければならない
- summary は原因分類が分かる粒度を持たなければならない
  - 例: missing credential
  - 例: invalid argument
  - 例: protocol transport failure
  - 例: native codec failure
- 設定エラーでは不足している環境変数名または invalid option 名を示さなければならない
- request validation error では invalid field 名を示さなければならない
- `--verbose` 指定時は以下を追加してよい
  - `CallError.Kind`
  - endpoint id
  - protocol path
  - protocol status code
- secret は常に redact しなければならない

## 10. Help

- root help は canonical CLI に加えて wizard と shell の存在または current phase の扱いが分かるようにしてよい
- help は階層的でなければならない
- 各階層で usage 例を少なくとも 1 つ示す
- write command の help では `--yes` 要件を明示する
- command help は以下を含まなければならない
  - 認証要否
  - canonical JSON input 例
  - 提供する convenience flag 一覧
  - write safety の有無
- help から `--request-template` / `--query-template` / `--body-template` の存在が分かるようにしなければならない

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
- full-screen TUI

## 11.1 再導入条件

現行非スコープ項目は、対応する library / adapter 契約が先に固定された場合にのみ再導入してよい。

- `normalize`
  - native DTO と protocol raw response の対応関係を、単一 command 内で安定して表現できる差分 schema が必要
- `parity-check`
  - fixture corpus、期待判定、CLI と CI が共有する oracle が必要
- `replay`
  - deterministic な request/response log format と replay 対象境界の固定が必要
- `table` 出力
  - endpoint ごとの列定義と列安定性ポリシーが必要
- 抽象 `--env`
  - venue 共通で意味の一致する environment model が必要
- generic `dry-run`
  - write command に対する preview semantics を facade 層または adapter 層で一貫して定義できることが必要
- generic idempotency key
  - 複数 venue にまたがって意味の一致する idempotency 契約が必要

上記条件が満たされない限り、CLI は Stage11 の current contract を優先し、非スコープ項目を先行導入してはならない。

## 12. 例

bitFlyer native public:

```bash
exchangeapi bitflyer native public get-ticker \
  --product-code BTC_JPY \
  --pretty
```

bitFlyer native public canonical JSON:

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
  --symbol BTCJPY \
  --interval 1h \
  --limit 2 \
  --pretty
```

Binance native public canonical JSON:

```bash
exchangeapi binance native public get-klines \
  --request-json '{"Symbol":"BTCJPY","Interval":"1h","Limit":2}'
```

bitFlyer private write:

```bash
exchangeapi bitflyer native private cancel-all-child-orders \
  --product-code BTC_JPY \
  --yes
```

bitFlyer template:

```bash
exchangeapi bitflyer native private send-child-order \
  --request-template
```

bitFlyer wizard:

```bash
exchangeapi wizard bitflyer native public get-ticker
```

補助 shell の想定形:

```text
exchangeapi shell
exchangeapi> use venue bitflyer
exchangeapi> use surface native
exchangeapi> use scope public
exchangeapi> run get-ticker --product-code BTC_JPY
```
