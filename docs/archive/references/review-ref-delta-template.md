# REVIEW-REF-DELTA

本レビューは `docs/reference/*` の知見を用いて、実装・文書の品質を補助監査するテンプレートである。  
Merge Gate ではなく、改善提案と規範昇格判断のために使う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。  
Fatal 判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* 対象範囲（code / docs）:
* 参照した `docs/reference` 文書:
  * `docs/reference/reviews/REVIEW-01-naming.md`
  * `docs/reference/reviews/REVIEW-02-parameters.md`
  * `docs/reference/reviews/REVIEW-03-implementation.md`
  * `docs/reference/reviews/REVIEW-04-layering.md`
  * `docs/reference/reviews/REVIEW-05-cross-exchange.md`
  * `docs/reference/reviews/REVIEW-06-constants.md`
  * `docs/reference/reviews/REVIEW-07-boilerplate.md`
  * `docs/reference/reviews/user-experience-review.md`
  * `docs/reference/checklists/implementation.md`
  * `docs/reference/checklists/naming.md`
  * `docs/reference/navigation.md`
  * `docs/reference/utilities.md`

---

## 1. 判定サマリ

| 観点 | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- |
| 命名・語彙整合 |  |  |  |  |
| 引数設計整合 |  |  |  |  |
| 実装フロー整合 |  |  |  |  |
| レイヤ境界整合 |  |  |  |  |
| 取引所間パリティ |  |  |  |  |
| 定数/文字列表現統制 |  |  |  |  |
| ボイラー抑制・共通化 |  |  |  |  |
| DX導線/利用体験 |  |  |  |  |
| Reference運用健全性 |  |  |  |  |

---

## 2. 観点詳細

### 命名・語彙整合

* 判定基準: 層語彙・同義語・EndpointId由来命名が統一されている
* OK条件: 同一概念で命名揺れがなく、語彙の所在が一意
* NG条件: 同義語が混在し、層境界や責務の解釈が分岐
* 不合格例: 同一概念を `Ticker` / `Symbol` / `ProductCode` で無規律に併用
* 該当Fatal: F2（規範逸脱に波及する場合）
* 修正方針: 正本語彙へ統一し、旧語は移行マップで明示する

### 引数設計整合

* 判定基準: DTO/primitive、optional、引数順序が規則化されている
* OK条件: 公開境界シグネチャに一貫した順序・型ルールがある
* NG条件: APIごとに順序/optional表現が揺れ、誤用を誘発
* 不合格例: 同等操作で引数順が逆転し、CT命名も混在
* 該当Fatal: F3（契約破壊を伴う場合）
* 修正方針: DTO化方針と順序規約を固定し、逸脱を段階移行する

### 実装フロー整合

* 判定基準: 取得→判定→変換→正規化のフローが取引所間で同形
* OK条件: エラー判定位置・正規化方針・例外分類が統一
* NG条件: 同一責務が実装ごとに分岐し、運用判断が不安定
* 不合格例: 一方のみ payload business error 判定を欠く
* 該当Fatal: F1/F3（境界破壊・契約不整合の場合）
* 修正方針: 共通フローを先に固定し、差異は拡張点へ閉じ込める

### レイヤ境界整合

* 判定基準: 依存方向・責務分離・層ジャンプ禁止を満たす
* OK条件: 境界越境がなく、層ごとの責務が明確
* NG条件: 境界を跨ぐ直接依存や、層責務の逆流が存在
* 不合格例: Adapter都合が Contracts へ露出
* 該当Fatal: F1
* 修正方針: 層責務に再分配し、依存方向を正す

### 取引所間パリティ

* 判定基準: 揃えるべき差分と許容非対称が明文化されている
* OK条件: 追加取引所でも再現可能な標準形がある
* NG条件: 非対称の理由がなく、取引所追加時に再設計が必要
* 不合格例: 片系だけ別フローを採用し、根拠が未記載
* 該当Fatal: F1/F3（契約または境界を破壊する場合）
* 修正方針: 標準形・許容差・非採用理由をセットで記録する

### 定数/文字列表現統制

* 判定基準: マジックストリングを抑止し、語彙の置き場が一貫
* OK条件: 定数/enum/VO の配置方針が統一される
* NG条件: 語彙の重複定義や直書きが散在
* 不合格例: 署名キーや endpoint component の文字列直書き
* 該当Fatal: F2（正本語彙と衝突する場合）
* 修正方針: 共通化可能語彙を集約し、取引所固有は境界内に限定する

### ボイラー抑制・共通化

* 判定基準: 同形処理の反復が抑制され、共通化指針がある
* OK条件: 重複パターンが分類され、優先度付きで回収可能
* NG条件: 同形増殖が継続し、実装品質が取引所単位で分岐
* 不合格例: 例外変換や Call 骨格の手作業複製
* 該当Fatal: NonFatal（保守性劣化）
* 修正方針: 共通化候補を P0/P1/P2 で分割し段階適用する

### DX導線/利用体験

* 判定基準: 初回導入導線・エラー時次行動・参照順が明確
* OK条件: 利用者が最短で1成功できる導線が提示される
* NG条件: 読み順や次行動が不明で、運用依存が増える
* 不合格例: 失敗時に復旧フローが示されない
* 該当Fatal: NonFatal（運用品質）
* 修正方針: 最小導入導線・エラー判定フロー・利用目的別導線を整備する

### Reference運用健全性

* 判定基準: `docs/reference/*` が非規範として運用され、正本と衝突しない
* OK条件: 採用ルールは Normative/Process へ昇格済み、Reference は比較・検討に限定
* NG条件: Reference 側だけで実質ルール運用され、正本未反映
* 不合格例: Reference 文書に必須手順を残し、正本と乖離
* 該当Fatal: F2
* 修正方針: 規範本文を正本へ移管し、Reference 側は根拠と経緯に限定する

---

## 3. CI自動化候補

* 命名揺れ検査（語彙辞書ベース）
* 引数順序/optional表現の規約検査
* 取引所間フロー差分の回帰検査（パリティ）
* マジックストリング検査（定数化対象）
* docs/reference の Non-Normative 明示検査

---

## 4. 関連Normative / 判例

* `docs/normative/topspec.md`
* `docs/normative/governance.md`
* `docs/normative/contracts/contracts.md`
* `docs/process/process.md`（7.2）
* `docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md`
* `docs/reference/reviews/README.md`

---

## 5. 最終結論

* OK / 要修正 / NG

---

## 6. アクション

* Keep: Reference に据え置く
* Revise: Reference を修正して再監査
* Promote: 規範へ昇格（`docs/normative/*` / `docs/process/*`）
* Archive: `docs/archive/references/*` へ退避
