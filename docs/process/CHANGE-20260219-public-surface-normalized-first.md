# CHANGE-20260219-public-surface-normalized-first

## Summary

公開面方針を明確化し、`Contract-only` の運用を見直して
`Normalized-first + Contracts minimal` を正式方針として文書整合した。

## What broke

- 方針文面の解釈が変わる（運用上、主利用面は Contracts ではなく Normalized）。
- 安定保証の境界は維持（Contracts のみ安定保証）。

## Why

- Contracts 層で取引所互換を全面担保するのは限界があり、
  取引所別 API 網羅を Normalized に寄せる方が無理のない仕様だから。

## Migration

1. Bot/高度利用の導線を `Normalized` 主体へ移す（`<Exchange>Factory.CreateClient(...)`）。
2. 取引所横断で共通化が必要な箇所のみ `Contracts` を使う。
   （`<Exchange>Factory.CreateContractClient(...)` を使用）
3. Contracts 依存の過剰共通化を削減する。

## Bot impact

- 取引所別機能を使う Bot は Normalized 利用が前提になる。
- Contracts のみで不足機能を扱っている Bot は、必要に応じて Normalized へ移行が必要。
