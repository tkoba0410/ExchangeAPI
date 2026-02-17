# REVIEW-MERGE-CHECK（2026-02-17 / stage9）

本レビューは `docs/process/process.md` 7.2（Merge 前必須）に基づく最終確認である。

対象: branch `stage9`

---

# 7.2 Checklist

## A. 層と境界（TopSpec）

- [x] 層ジャンプなし（`tests/Composition.Tests/Guard/LayerBoundaryGuardTests.cs`）
- [x] Wire が JSON を解釈しない（`src/Exchanges/*/Wire` に `System.Text.Json` / `JsonDocument` 参照なし）
- [x] `string` の境界漏れなし（`LayerBoundaryGuardTests` / Contracts I/F 点検）
- [x] Raw は lossless 前提を維持（既存 Raw/Normalized 分離テスト群で回帰なし）
- [x] Raw DTO / RawJson の公開面漏れなし（`LayerBoundaryGuardTests`）
- [x] 単一実装内で複数取引所 Raw/Normalized 混在なし（既存境界テスト群で回帰なし）

## A2. 非層カテゴリ（TopSpec）

- [x] `src/Application` は `Contracts.*` 非参照（検索確認）
- [x] Facade Request/Interface の直接流入に重大回帰なし（既存構成で回帰なし）
- [x] Composition 逆流の重大回帰なし（`tests/Composition.Tests` 通過）

## B. 契約（Contracts）

- [x] 公開 API は Call-only（`src/Contracts/Facade/Interfaces/*.cs` 点検）
- [x] Contracts に取引所名混入なし（`src/Contracts` 検索確認）
- [x] DTO/Nullable 規範の重大逸脱なし（`BatchError` / `ICandlesticksApi` 分離後の契約同期済み）

## C. Inventory（事実一覧）

- [x] 外部 API 変更は inventory 反映済み（`docs/inventory/endpoints-contracts.md` 更新済み）
- [x] Official Reference が明示されている（各 inventory 冒頭/列定義を確認）
- [x] `PresentIn` 列/語彙整合（`tests/Docs.Inventory.Tests` 通過）
- [x] `PresentIn` と Raw/Normalized `CallAsync` 整合（inventory 一貫性テスト通過）
- [x] `EndpointId` と alias 分離整合（inventory 一貫性テスト通過）

## D. 例外（Decisions）

- [x] 未登録の例外を新規導入していない

---

# Verification

- `dotnet build ExchangeApi.slnx -c Release` : Passed
- `dotnet test tests/Docs.Inventory.Tests/Docs.Inventory.Tests.csproj -c Release` : Passed
- `dotnet test tests/Common.Tests/Common.Tests.csproj -c Release` : Passed
- `dotnet test tests/Composition.Tests/Composition.Tests.csproj -c Release` : Passed
- `dotnet test tests/Exchanges/Bitflyer/Adapter.Tests/Exchange.Bitflyer.Adapter.Tests.csproj -c Release` : Passed
- `dotnet test tests/Exchanges/Bittrade/Adapter.Tests/Exchange.Bittrade.Adapter.Tests.csproj -c Release` : Passed

---

# Findings

## Must

なし

## Should

なし

## Nit

なし

---

# Conclusion

7.2 必須項目の最終確認は完了。Merge 判定は「可」。
