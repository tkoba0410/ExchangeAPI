# STAGE10-LIVE-EVIDENCE

本書は、Stage10 第1段階の bitFlyer live test 実行結果を保存するための証跡テンプレートである。  
履歴ログは `docs/process/reviews/STAGE10-LIVE-EVIDENCE-<date>.md` として保存する。

---

## 0. 対象

* 実施日:
* 対象ブランチ:
* 対象 commit:
* 実施者:
* 対象 exchange:
* 対象層:
* 対象 endpoint:

---

## 1. 実行前提

* 資格情報ソース:
  * direct env / age credential store
* 口座:
  * 専用テスト口座 / account id
* 対象 market:
  * `Symbol`:
  * `ProductCode`:
* POST 有効化:
  * `EXCHANGEAPI_BITFLYER_LIVE_ALLOW_POST=0|1`
* POST 条件:
  * `Side`:
  * `Size`:
  * `Price`:

---

## 2. 実行コマンド

```bash
dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --nologo --verbosity minimal
```

必要に応じて、以下のようにフィルタ実行を併記する。

```bash
dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PublicGet"
dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PrivateGet"
dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PrivatePost"
dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Layer=Normalized"
```

---

## 3. 実行結果サマリ

* 総結果:
  * passed:
  * failed:
  * skipped:
* 実行時間:
* 未取消 `ACTIVE` 注文:
  * なし / あり

| Flow | Layer | Result | Notes |
| --- | --- | --- | --- |
| PublicGet | Wire |  |  |
| PublicGet | Raw |  |  |
| PublicGet | Normalized |  |  |
| PrivateGet | Wire |  |  |
| PrivateGet | Raw |  |  |
| PrivateGet | Normalized |  |  |
| PrivatePost | Wire |  |  |
| PrivatePost | Raw |  |  |
| PrivatePost | Normalized |  |  |

---

## 4. Lifecycle 結果

* `SendChildOrder`:
  * acceptance id 取得可否:
* `GetChildOrders`:
  * 可視化確認:
* `CancelChildOrder`:
  * 取消成功:
* `GetChildOrders` 再確認:
  * `ACTIVE` 非表示確認:

---

## 5. 発見事項

| Severity | Area | Fact | Impact | Status |
| --- | --- | --- | --- | --- |
|  |  |  |  |  |

---

## 6. 証跡

* 実行ログ:
  * 自動ログ root:
  * `run.json`:
  * `events.jsonl`:
  * サニタイズ方針:
    * auth 系: mask
    * order/account 系 identifier: pseudonymize
    * private balance/collateral 系数値: mask
* 関連 test project:
  * `tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj`
* 関連 stage 文書:
  * `stage10.md`
* 関連運用文書:
  * `docs/process/templates/README.md`
  * `docs/process/templates/bitflyer-live.env.template`

---

## 7. 裁定材料

* DoD 判定:
  * 満たす / 未充足あり
* 未解消 blocker:
  * なし / あり
* 次アクション:
  * Stage10 第1段階 close 判定へ進む / 追加修正 / 再実行
