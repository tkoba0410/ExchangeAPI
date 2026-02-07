# 命名監査レポート（2026-02-07）

対象:
- `src/Contracts`
- `src/Exchanges`
- `tests`（命名確認に関わる参照のみ）

基準:
- `docs/checklists/naming.md`

---

## 要修正（差分）

1. `Symbol` と `Market` の混在（Contracts Request）
- 状況: 同一意味（通貨ペア）に `Symbol` と `Market` が混在
- 根拠:
  - `src/Contracts/Facade/Requests/TickerRequest.cs:5` `Symbol Symbol`
  - `src/Contracts/Facade/Requests/BoardRequest.cs:5` `Symbol Symbol`
  - `src/Contracts/Facade/Requests/OrdersRequest.cs:6` `Symbol Market`
  - `src/Contracts/Facade/Requests/ExecutionsPrivateRequest.cs:6` `Symbol Market`
  - `src/Contracts/Facade/Extensions/PrivateApiExtensions.cs:37` `Symbol market`
- 影響: 利用者視点の引数語彙が不統一
- 推奨: Contracts 層は `Symbol` に統一（`Market` は内部文脈名に限定）

2. `OrderId` 意味のフィールドが `Id` になっている（Bittrade Normalized）
- 状況: 実体が `OrderId` なのにフィールド名が `Id`
- 根拠:
  - `src/Exchanges/Bittrade/Normalized/Public/Dtos/ExecutionNormalized.cs:11` `OrderId Id`
  - `src/Exchanges/Bittrade/Adapter/Internal/Mappers/MarketMapper.cs:48` `OrderId: normalized.Id`
- 影響: `Id` が何の識別子か不明瞭
- 推奨: `OrderId` へ改名

3. Kline 時刻キーのフィールド名が `Id`（意味不一致）
- 状況: Unix 時刻を保持する値が `Id` 命名
- 根拠:
  - `src/Exchanges/Bittrade/Normalized/Public/Dtos/KlineNormalized.cs:6` `FreeText Id`
  - `src/Exchanges/Bittrade/Adapter/Internal/Mappers/MarketMapper.cs:68` `ParseUnixTimestamp(kline.Id)`
- 影響: 読み手が識別子と誤認しやすい
- 推奨: `Timestamp` または `OpenTimeUnix` など意味名に変更

---

## 許容（EndpointId 由来）

1. `GetCurrencys` の綴り
- 状況: 一般英語では `Currencies` だが、EndpointId は `GetCurrencys`
- 根拠:
  - `docs/inventory/endpoints-bittrade.md:80` `GetCurrencys`
  - `src/Exchanges/Bittrade/Wire/Constants/EndpointIds.cs:7` `GetCurrencys`
- 判断: EndpointId 由来として許容（仕様準拠）

---

## 備考

- 本レポートは「高確度で機械的に判定可能な差分」に限定した。
- 追加で厳密化する場合は、Roslyn Analyzer による命名規約の自動検査導入を推奨。
