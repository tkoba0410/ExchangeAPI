# Local Evidence

本ディレクトリは、実行結果、artifact、log、手動確認メモを置く local-only evidence 領域である。

標準構成:

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

`local/evidence/` 配下の run directory は repository の正本ではない。  
必要な場合だけローカルに残し、credentials、署名値、API key / secret を含めてはならない。
