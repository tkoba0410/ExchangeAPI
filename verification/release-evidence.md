# Release Evidence Runbook

位置づけ: v2.2.0 release evidence runbook

本 runbook は release verification の証跡を `local/evidence/` に残す場合の標準手順を定義する。
default では evidence / log は作らない。

## 1. Directory Layout

```text
local/evidence/<phase>/<yyyymmdd>-<label>/
  runtime/
    artifacts/
    logs/
  notes/
```

phase:

- `static`
- `verification`
- `local-live`
- `test-operation`

v2.2.0 release verification の例:

```text
local/evidence/verification/<yyyymmdd>-v2.2.0-release/
local/evidence/local-live/<yyyymmdd>-v2.2.0-mcp-inspection/
```

## 2. Secret-Free Rule

credentials、API key、API secret、signature、Authorization header は evidence、log、result、exception、stdout、stderr に含めない。

コピー禁止:

- raw credential profile
- age secret key
- decrypted credentials JSON
- protocol raw log with auth headers

保存可能:

- sanitized summary JSON
- test result summary
- package / asset file list
- SHA-256 checksum
- operator notes

## 3. Script Integration

`ExchangeApi.Optional.Logging` の evidence helper を使う場合も、接続対象は scripts / verification に限定する。

禁止:

- library public API を増やす
- CLI option を追加する
- live test の default path で必須接続する
- opt-in なしで evidence / log を作る

## 4. Suggested Checks

release verification 後、必要に応じて次を確認する。

```bash
rg -n "apiKey|apiSecret|signature|Authorization|ACCESS-KEY|ACCESS-SIGN|X-Bitflyer-Access-Key|X-Bitflyer-Access-Sign" local/evidence/
```

検出された場合:

- 対象 evidence を release 証跡として採用しない
- secret を含む可能性がある artifact を削除する
- redaction または保存対象を修正して再実行する
