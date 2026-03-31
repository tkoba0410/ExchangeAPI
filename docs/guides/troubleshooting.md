# Troubleshooting

## credential が見つからない

bitFlyer private read や private tool を使う場合は、起動プロセスに次の環境変数が必要。

```text
EXCHANGEAPI_AGE_IDENTITY_FILE_PATH
EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH
```

例:

```bash
export EXCHANGEAPI_AGE_IDENTITY_FILE_PATH=/abs/path/to/age.key
export EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH=/abs/path/to/credentials.enc.json
```

CLI / MCP Server は、起動時の環境変数を読む。変更したらプロセスを再起動する。

## NuGet 解決に失敗する

consumer repo の `NuGet.config` に local feed が入っているか確認する。

```xml
<add key="exchangeapi-local" value="/absolute/path/to/ExchangeAPI/local/nuget" />
```

local package を作り直したのに古い package が使われる場合は、version を増やす。

```bash
bash scripts/pack-local-nuget.sh 0.1.0-local.2
```

同じ version を再利用した場合は cache を消す。

```bash
dotnet nuget locals global-packages --clear
```

## venue 指定を間違えた

Library / CLI / MCP で使える venue は同じではない。

- CLI
  - `bitflyer`
  - `binance`
- MCP `get_klines`
  - `venue = "binance"`
- MCP private tool
  - `venue = "bitflyer"`
  - `accountContext = "default"`

迷ったら次を確認する。

- CLI: `exchangeapi --help`
- MCP: [`../mcp-server.md`](../mcp-server.md)
- endpoint support: [`../endpoints-bitflyer.md`](../endpoints-bitflyer.md), [`../endpoints-binance.md`](../endpoints-binance.md)
