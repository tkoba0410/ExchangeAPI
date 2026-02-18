# REVIEW-SUMMARY-20260218-stage9

対象: リポジトリ全体（branch: `stage9`）  
基準: `docs/process/review-framework.md` / `docs/process/codex-review-runbook.md` / 各軸テンプレート  
実施日: 2026-02-18

---

## 1. 軸別判定サマリ

| 軸 | OK | 要修正 | NG | Fatal |
| --- | --- | --- | --- | --- |
| Boundary | 0 | 0 | 0 | 0 |
| Consistency | 0 | 0 | 0 | 0 |
| Contracts | 0 | 0 | 1 | 0 |
| Reliability | 0 | 0 | 0 | 0 |
| Security | 0 | 0 | 2 | 2 |
| DX | 0 | 0 | 1 | 1 |
| Change | 0 | 0 | 0 | 0 |
| Docs（補助監査） | 0 | 0 | 0 | 0 |

注記: `DX` の NG は `Security` と同一根因（secret露出）で重複計上。

---

## 2. 指摘一覧（重大順）

`[NG] Request URI をそのままログ出力し、署名付きクエリの露出経路を作っている - src/Transport/Observability/StructuredRestClientLogger.cs:21 - F4 - REVIEW-SECURITY（secret非露出）`

`[NG] 署名情報をクエリへ付与しており、上記ロガー経由で機密情報露出に直結する - src/Exchanges/Bittrade/Adapter/Internal/RequestSigner.cs:52 - F4 - REVIEW-SECURITY（署名/Canonicalize整合・secret非露出）`

`[NG] Contracts 層で取引所名を含む識別子を生成する API が公開されている - src/Contracts/Facade/Operations/OperationComponent.cs:7 - NonFatal - docs/normative/contracts/contracts.md 3章（取引所非依存）`

`[NG] secret非露出の観点で DX も不合格（Securityと同根因） - src/Transport/Observability/StructuredRestClientLogger.cs:34 - F4 - REVIEW-DX（secret非露出）`

---

## 3. 最優先 NG Top10（Fatal優先）

1. `StructuredRestClientLogger` の `uri` 生出力による機密露出経路（F4） - `src/Transport/Observability/StructuredRestClientLogger.cs:21`
2. `Bittrade RequestSigner` の署名クエリ付与とログ露出の結合（F4） - `src/Exchanges/Bittrade/Adapter/Internal/RequestSigner.cs:52`
3. `OperationComponent.WithExchange` による Contracts 取引所依存の導入（NonFatal） - `src/Contracts/Facade/Operations/OperationComponent.cs:7`
4. （同根因）DX 観点での secret 非露出違反（F4） - `src/Transport/Observability/StructuredRestClientLogger.cs:34`

---

## 4. 未解消件数

- 未解消 NG 総件数（重複除外）: **3**
- 未解消 Fatal 件数（重複除外）: **2**

---

## 5. 例外記録要否

- F4 の 2 件は原則修正対象。修正せず維持する場合は `docs/process/exceptions.md` への記録が必要。
- Contracts 非依存違反（NonFatal）を意図維持する場合も `docs/process/exceptions.md` への記録が必要。

---

## 6. 補足検証

- `dotnet test --nologo` 実行: **全テスト成功（失敗 0）**
