# 命名監査レポート（2026-02-07）

対象:
- `src/Contracts`
- `src/Exchanges`
- `tests`（命名確認に関わる参照のみ）

基準:
- `docs/reference/checklists/naming.md`

---

## 要修正（差分）

現時点で高確度の要修正項目は未検出。

## 解消済み（今回確認）

1. `Symbol` と `Market` の混在（Contracts Request）
- 状況: 解消済み（`Symbol` に統一）
- 確認:
  - `src/Contracts/Facade/Requests/OrdersRequest.cs:6` `Symbol Symbol`
  - `src/Contracts/Facade/Requests/ExecutionsPrivateRequest.cs:6` `Symbol Symbol`
  - `src/Contracts/Facade/Extensions/PrivateApiExtensions.cs:37` `Symbol symbol`

2. `OrderId` 意味のフィールドが `Id` になっている（Bittrade Normalized）
- 状況: 解消済み
- 確認:
  - `src/Exchanges/Bittrade/Normalized/Public/Dtos/ExecutionNormalized.cs:11` `OrderId OrderId`
  - `src/Exchanges/Bittrade/Adapter/Internal/Mappers/MarketMapper.cs:48` `OrderId: normalized.OrderId`

3. Kline 時刻キーのフィールド名が `Id`（意味不一致）
- 状況: 解消済み
- 確認:
  - `src/Exchanges/Bittrade/Normalized/Public/Dtos/KlineNormalized.cs:6` `FreeText OpenTimeUnix`
  - `src/Exchanges/Bittrade/Adapter/Internal/Mappers/MarketMapper.cs:68` `ParseUnixTimestamp(kline.OpenTimeUnix)`

---

## 許容（外部仕様追従）

1. Bittrade public path の `/currencys` 表記
- 状況: 外部仕様の path 表記は `/v1/common/currencys` だが、内部命名は `GetCurrencies*` に統一済み。
- 根拠:
  - `docs/inventory/endpoints-bittrade.md:80` `Path=/v1/common/currencys` / `EndpointId=GetCurrencies`
  - `src/Exchanges/Bittrade/Wire/Constants/Paths.cs:13` `CommonCurrenciesPath`
  - `src/Exchanges/Bittrade/Wire/Constants/EndpointIds.cs:7` `GetCurrencies`
- 判断: path は外部仕様に追従、内部命名は正規英語で管理。

---

## 備考

- 本レポートは「高確度で機械的に判定可能な差分」に限定した。
- 追加で厳密化する場合は、Roslyn Analyzer による命名規約の自動検査導入を推奨。
