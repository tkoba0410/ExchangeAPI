# Naming / DTO / Layer Debt Audit (Phase A/B/C)

> Informative only: 本文書は監査ログであり、設計判断の正本ではない。

Date: 2026-02-13
Branch: `feature/stage8`

## Scope
- Naming debt (legacy names / inappropriate prefixes / layer vocabulary bleed)
- DTO debt (unused / duplicate / boundary bleed)
- Layer responsibility bleed (Contracts dependency direction)

## A. Naming Debt

### A-1. Legacy renamed symbols (must be zero)
Command:
- `rg -n "RawMergedTick|RawDepthTick|RawTradeTick|RawTradeEntry" -S src tests docs`

Result:
- **0 hits**

Assessment:
- Legacy names from prior rename are already cleaned.

### A-2. `Get*` prefix scan
Command:
- `rg -n "\bGet[A-Z]\w*" src tests -S`

Result:
- Large number of hits, mostly endpoint-derived method/type names (e.g., `GetTicker`, `GetOrders...`).

Assessment:
- This scan includes many **intentional EndpointId-derived names** and is not directly actionable as-is.
- No targeted rename was applied in this sprint to avoid breaking established EndpointId traceability.
- Policy decision: EndpointId 由来の `Get*` 命名は許容し、命名違反として扱わない。

### A-3. Layer vocabulary bleed scan (`Wire|Json|Http|Dto`)
Command:
- `rg -n "\bWire\b|\bJson\b|\bHttp\b|\bDto\b" src/Contracts src/Exchanges/*/Normalized -S`

Result summary:
- `Contracts`: no exchange-layer vocabulary bleed in public boundary signatures.
- `Normalized`: `Json` references exist in some files for payload handling.

Assessment:
- `Contracts` side is acceptable.
- `Normalized` `Json` references are existing design choices; no minimal safe rename/removal identified in this sprint without broader refactor.
- Policy decision: `Json` 参照は実装内部では許容。ただし公開境界型名への `Json/Wire/Http` 混入は禁止する。

## B. DTO Debt

### B-1. Unused DTO candidates
Quick reference count indicated one clear dead DTO:
- `ExecutionAccount` appears only in:
  - declaration (`src/Contracts/Common/Dtos/ExecutionAccount.cs`)
  - one utility extension method (`src/Utilities/Extensions/SideSizeExtensions.cs`)

Decision:
- Remove `ExecutionAccount` and its unused extension method as dead code.

### B-2. Intra-layer duplicate DTOs
- No clear same-layer duplicate payload pair was found that could be merged safely in this sprint with minimal diff.

### B-3. DTO layer crossing
- No direct evidence of exchange-specific DTO exposure from Contracts public interfaces.

## C. Layer Responsibility / Dependency Direction

### C-1. Contracts reverse dependency risk
- Existing guard checked Raw/Wire/Json exposure but did not explicitly block `ExchangeApi.Exchanges.*` namespace exposure from Contracts signatures.

Decision:
- Add a guard rule to fail if Contracts public signatures expose any `ExchangeApi.Exchanges.*` type.

## Planned changes from this audit
1. Remove dead DTO `ExecutionAccount` and unused extension.
2. Add lightweight guard lock for Contracts -> Exchanges namespace exposure.

## Closure

- `ExecutionAccount` と未使用 extension は削除済み。
- Contracts 公開シグネチャに `ExchangeApi.Exchanges.*` が露出したら失敗するガードを追加済み。
- A-2/A-3 は方針決定でクローズ（追加改名は不要）。
