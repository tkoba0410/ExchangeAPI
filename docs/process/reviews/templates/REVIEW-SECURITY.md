# REVIEW-SECURITY

本レビューは Security（署名 / 認証 / 秘密情報）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* 対象層（Wire / Raw / Adapter / Security境界）:

---

## 1. 判定サマリ

| 観点                | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| ----------------- | -- | ----------- | ----- | -- |
| 署名順序整合            |    |             |       |    |
| Canonicalize整合    |    |             |       |    |
| secret非露出         |    |             |       |    |
| nonce/timestamp安全 |    |             |       |    |

---

## 2. 観点詳細

### 署名順序整合

* 判定基準: 署名対象データが仕様と一致
* OK条件: canonicalize手順・連結順・対象フィールドが仕様と一致
* NG条件: 署名対象順序や対象データが仕様から逸脱
* 不合格例: クエリ順序未整列のまま署名を生成
* 該当Fatal: F4（Security重大違反）
* 修正方針: 署名生成処理を仕様準拠で再実装し、順序固定テストを追加

### secret非露出

* 判定基準: APIキー/署名素材がログや例外に出ない
* OK条件: secretはマスクされ、例外・ログに生値が出力されない
* NG条件: secretまたは署名素材がログ/例外に露出
* 不合格例: 例外メッセージにAPIキー断片を含む
* 該当Fatal: F4
* 修正方針: ログ出力点のマスキング統一と禁止パターン検査を追加

---

## 3. CI自動化候補

* 署名順序テスト
* ログ出力検査

---

## 4. 関連Normative / 判例

* docs/normative/topspec.md
* docs/normative/contracts/contracts.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
