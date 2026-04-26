# ExchangeAPI Verification

本ディレクトリは、manual / live verification の本体を置く場所である。

- endpoint ごとの契約固定や deterministic test は `tests/` に置く
- manual / live verification の runbook、scenario、replay input template は `verification/` に置く
- 実行結果、artifact、log、手動確認メモは `local/evidence/` に置く
- credentials、署名値、API key / secret を evidence に含めてはならない

詳細は [`docs/verification.md`](../docs/verification.md) を参照する。
