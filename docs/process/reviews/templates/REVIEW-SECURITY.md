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

### Canonicalize整合

* 判定基準: canonicalize規則（正規化/連結/エンコード）が仕様と一致
* OK条件: 全要求で同一canonicalize規則が適用される
* NG条件: エンドポイントごとにcanonicalize実装が分岐・不一致
* 不合格例: 特定endpointのみ未エンコード値で署名計算する
* 該当Fatal: F4（署名不整合）
* 修正方針: canonicalize処理を共通化し、仕様準拠テストを追加する

### secret非露出

* 判定基準: APIキー/署名素材がログや例外に出ない
* OK条件: secretはマスクされ、例外・ログに生値が出力されない
* OK条件: ロガー差し替え時も共通のサニタイズ経路で secret 非露出が担保される
* OK条件: `exception.Message` を生出力せず、追跡は `error_ref` 等の疑似識別子で行う
* NG条件: secretまたは署名素材がログ/例外に露出
* NG条件: 特定ロガー実装にのみ秘匿化が存在し、差し替えで露出し得る
* NG条件: `exception.Message` をログ行や OTel status description にそのまま出力する
* 不合格例: 例外メッセージにAPIキー断片を含む
* 該当Fatal: F4
* 修正方針: ログ出力点のマスキング統一と禁止パターン検査を追加

### nonce/timestamp安全

* 判定基準: nonce/timestampの生成と検証が再送・時刻ずれに耐える
* OK条件: 一意性・単調性・有効期限が仕様に沿って管理される
* NG条件: 再利用可能なnonceや無検証timestampを許容する
* 不合格例: 同一nonceを並行要求で再利用して拒否される
* 該当Fatal: F4（再送攻撃/失効不備）
* 修正方針: nonce管理を単一責務へ集約し、時刻許容差を明示実装する

---

## 3. CI自動化候補

* 署名順序テスト
* ログ出力検査
* ロガー差し替え時の秘匿化回帰テスト
* `exception.Message` 生出力禁止検査（log/trace）

---

## 4. 関連Normative / 判例

* docs/normative/topspec.md
* docs/normative/contracts/contracts.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
