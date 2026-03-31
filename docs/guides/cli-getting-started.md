# CLI Getting Started

## 1. 目的

ExchangeAPI CLI を起動し、最小の public read command を 1 回成功させる。

CLI は人間向けの運用・検証・デバッグ成果物であり、Bot の実行面としては使わない。

## 2. 前提

- `dotnet` SDK `10.0`
- ExchangeAPI repo を checkout 済み

## 3. 導入

CLI executable を local publish する。

```bash
bash scripts/publish-cli-local.sh
```

生成先:

```text
local/publish/cli/linux-x64/exchangeapi
```

## 4. 最小例

bitFlyer public ticker を取得する。

```bash
./local/publish/cli/linux-x64/exchangeapi \
  bitflyer native public get-ticker \
  --product-code BTC_JPY \
  --summary --pretty
```

## 5. 動作確認

成功時は `stdout` に JSON、`stderr` に summary が出る。

`stdout` 例:

```json
{
  "product_code": "BTC_JPY"
}
```

`stderr` 例:

```text
bitflyer native public get-ticker: success
```

CLI の詳細 contract は [`../cli.md`](../cli.md) を参照する。
