# Troubleshooting

## credential が見つからない

bitFlyer private read や private tool を使う場合は、credential profile を用意する。

既定 path:

```bash
local/credentials/credential-profile.json
```

例:

```json
{
  "version": 1,
  "credentials": {
    "bitflyer": {
      "provider": "age-file",
      "identityFilePath": "current/age-identity.txt",
      "credentialsFilePath": "current/bitflyer.age"
    }
  }
}
```

`current/age-identity.txt` と `current/bitflyer.age` は、実ファイルへの symlink として置いてよい。
CLI は `--credential-profile <path>` で別 profile を指定できる。MCP Server も `--credential-profile <path>` を受け取る。

age 暗号化済み credentials file を作成するだけなら、local setup helper を使える。

```bash
bash scripts/create-age-credential-file.sh --venue bitflyer
```

この script は API key / API secret を非表示の対話入力で受け取り、平文 file を作らずに `local/credentials/current/bitflyer.age` を作成する。
取引所 API には接続せず、API key の有効性確認も行わない。

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
