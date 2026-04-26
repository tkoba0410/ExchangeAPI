# MCP Inspection Live Verification

位置づけ: v2.2.0 MCP inspection live verification runbook

本 runbook は、bitFlyer private read inspection tools を実 API 接続で確認するための手順である。
state-changing operation は対象にしない。

## 1. 対象 Tools

- `get_collateral_accounts`
- `get_balance_history`
- `get_collateral_history`
- `get_child_orders`

期待 response shape:

| Tool | Top-level shape |
| --- | --- |
| `get_collateral_accounts` | `accounts` |
| `get_balance_history` | `items` |
| `get_collateral_history` | `items` |
| `get_child_orders` | `orders` |

## 2. 実行条件

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
credential profile configured at local/credentials/credential-profile.json
```

credential profile は `local/credentials/credential-profile.json` を標準位置とする。
別 path を使える実行導線では `--credential-profile <path>` を使ってよい。

raw credential profile は evidence へコピーしない。

## 3. Evidence Directory

標準証跡先:

```text
local/evidence/local-live/<yyyymmdd>-v2.2.0-mcp-inspection/
  runtime/
    artifacts/
    logs/
  notes/
```

保存してよいもの:

- secret-free summary JSON
- secret-free command result summary
- secret-free stdout / stderr copy
- operator notes

保存してはいけないもの:

- raw credential profile
- API key
- API secret
- signature
- Authorization header
- `ACCESS-KEY`
- `ACCESS-SIGN`
- credential file path 以外の credential body

## 4. 実行

opt-in なしでは live tests は skip する。

```bash
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

live verification を実行する場合:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test tests/Adapters/McpServer.LiveTests/ExchangeApi.Adapters.McpServer.LiveTests.csproj --no-restore
```

## 5. 確認項目

- private credentials がある場合、`tools/list` に対象 4 tools が出る
- credential 未設定時、対象 4 tools は advertise されない
- response shape は `accounts` / `items` / `items` / `orders`
- state-changing operation は増えていない
- result / error / stdout / stderr に secret がない
- evidence は標準証跡先に残す

## 6. Secret Scan

stdout / stderr / evidence に対して、少なくとも次の marker を確認する。

```bash
rg -n "apiKey|apiSecret|signature|Authorization|ACCESS-KEY|ACCESS-SIGN|X-Bitflyer-Access-Key|X-Bitflyer-Access-Sign" local/evidence/local-live/<yyyymmdd>-v2.2.0-mcp-inspection/
```

期待:

- secret marker が検出されない
- 検出された場合は release evidence として採用しない
- secret を含む可能性がある artifact は削除し、原因を修正して再実行する

## 7. 記録 Template

`notes/summary.md`:

```markdown
# MCP Inspection Live Verification

date:
version:
command:
credential profile: local/credentials/credential-profile.json
result:
tools/list with credentials:
tools/list without credentials:
response shapes:
secret scan:
operator notes:
```
