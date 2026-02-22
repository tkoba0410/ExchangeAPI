# Process（運用ルール）

本書は、開発および文書整備の進め方を示す **参考文書**である。
設計規範・層責務・公開範囲の正本は  
**TopSpec（docs/normative/topspec.md）** とする。  
公開 API 契約の正本は **docs/normative/contracts/contracts.md** とする。
429 / Timeout / Partial Failure の契約正本は **docs/normative/contracts/resilience.md** とする。
`<exchange>` 物理構成（機械可読）の正本は
**docs/normative/layout/exchange-module-shape.json** とする。

なお、`docs/normative/contracts/*` は外部公開向けの **契約文書（公開安定 API の説明）** であり、
設計規範ではないが、**公開 API 契約としては Normative** である。

本書に記載された手順や判断は、TopSpec および
`docs/normative/layout/exchange-module-shape.json` に反しない範囲でのみ有効とする。

本書の目的は、運用上の迷いを減らし、**文書を増やさずに揺らぎを止める**ことである。

---

## 1. 文書運用の基本方針

- 文書は情報共有のためではなく、**判断余地（揺らぎ）を消すため**に書く。
- 迷った場合は **「書かない」** を選ぶ。
- 「説明（Why/How）」が必要になったときは、まず **削れるか / テストに寄せられるか** を検討する。

---

## 2. 文書の種類と役割（固定）

本リポジトリの文書は、次の種類に限定する（新カテゴリの増設は禁止）。

### 2.1 Normative（規範）
- `docs/normative/topspec.md`：技術仕様・設計規範の正本（層・境界・禁止事項）
- `docs/normative/layout/exchange-module-shape.json`：`<exchange>` 物理構成の機械可読正本（required/optional/forbidden）
- `docs/normative/naming-rules.md`：命名・語彙・DTO 接尾辞などの補助規範
- `docs/normative/contracts/contracts.md`：公開 API 契約の正本（Contract 層）
- `docs/normative/contracts/resilience.md`：429 / Timeout / Partial Failure 契約の正本
- `docs/normative/governance.md`：設計判断の裁定ルールの正本

### 2.2 Inventory（事実一覧）
- `docs/inventory/endpoints-<exchange>.md`：取引所別インベントリ
- `docs/inventory/endpoints-contracts.md`：Contracts 採用/対応関係の SSOT

※ Inventory に仕様本文（公式APIの写経）を書いてはならない。

#### 2.2.1 公式仕様の正本（MUST）
- 公式 API 仕様の正本は、各取引所の公式 API ドキュメント（公開 URL）とする。
- Inventory の CanonicalSourceUrl は、公式 API ドキュメント上の到達可能な URL を根拠として一致検証できなければならない。

### 2.3 Decisions（例外台帳）
- `docs/process/exceptions.md`：原則からの逸脱（差分と理由）

### 2.4 Process（運用）
- `docs/process/process.md`：文書化ルール・レビュー手順・例外運用
- `docs/process/review-framework.md`：レビュー体系（L1/L2/L3）と品質軸
- `docs/process/codex-review-runbook.md`：Codex 実行手順
- `docs/stage_9_close_policy.md`：Stage9 クローズ方針（終了判定・ループ抑止）
- `docs/process/reviews/templates/*.md`：軸別レビューの判定テンプレート
- `docs/process/reviews/README.md`：レビュー監査ログと語彙Lint範囲の運用ポリシー
- `docs/process/reviews/REVIEW-*.md` / `docs/process/reviews/STAGE*-*.md`：実施済みレビューの監査ログ（履歴）

#### 2.4.1 Governance（補足）
`docs/normative/governance.md` は「裁定ルールの正本（Normative）」であり、Process ではない。
Process 文書では Governance の内容を再定義せず、運用手順のみを記載する。
運用文書と乖離した場合の裁定は、`docs/normative/governance.md` の正本優先順に従う。

### 2.5 Reference（参考）
- `docs/reference/navigation.md`：用語ナビゲーション（非規範）
- `docs/reference/utilities.md`：補助的な運用メモ（非規範）
- `docs/reference/reviews/*`：過去レビューの判例・参考資料（Informative）
- `stage9.md`：Stage9 完了判定チェック（非規範）
- `docs/archive/document-plan.md`：履歴資料（Archive）
- `docs/archive/references/*`：過去検討・監査ログ（Informative）

---

#### 2.x 例外の例外（記録ルール）

- entry point 以外で throw を残す場合は、必ず `docs/process/exceptions.md` に理由を記録する。

---

## 3. 配置ルール
- `README.md` 以外の文書は、すべて `docs/` 配下に置く。
- `docs/` 配下のリンクは相対リンクのみを使用する。
- ルート直下に新たな設計・運用文書を追加してはならない。
- 例外として、Stage 文書（`stage*.md`）はルート直下への配置を許可する。
- Stage 文書（`stage*.md`）は **初回リリース前の暫定文書** とし、`v1.0.0` 到達時に `docs/archive/` へ移動する。
- リリース後の変更追跡は `docs/process/revision-history.md` を正本とする。
- 機密値（APIキー/シークレット/秘密鍵）を含む平文ファイルは `docs/` を含むリポジトリ管理下に配置してはならない。
- 設定ファイルが必要な機能は、実ファイルではなくテンプレート（`*.template.*` / `*.example.*`）のみを管理対象とする。

---

## 4. 書いてよいこと / 書いてはいけないこと

### 4.1 書いてよいこと

- MUST / MUST NOT による規範（TopSpec）
- 境界や責務の固定（層・依存方向・型の in/out）
- API の存在一覧（method / path / 参照元リンク）
- 例外（逸脱）とその理由（Decisions）
- レビュー時のチェック観点（本書）

### 4.2 書いてはいけないこと

- 公式ドキュメントの転記（API仕様本文の写経）
- 実装手順・コード解説・チュートリアル（ただし `README.md` の「最初の1コール（Quickstart）」は例外として許容）
- テスト手順書・CI説明
- 「現時点では〜」の未確定メモ（判断を固定できないため）

補足:

- Quickstart は「利用開始の導線」に限定し、規範本文（MUST/MUST NOT）の写経や長文解説は行わない（SSOTリンクへ寄せる）

---

## 5. 更新ルール（いつ、どれを更新するか）

- 規範（設計判断・境界・契約）を変更した場合：
  → `docs/normative/topspec.md`
- 物理構成の方針・拘束（required/optional/forbidden）を変更した場合：
  → `docs/normative/topspec.md` と `docs/normative/layout/exchange-module-shape.json`
- 外部 API を追加・削除・差し替えた場合：
  → `docs/inventory/endpoints-<exchange>.md`
- 原則からの逸脱が必要になった場合：
  → `docs/process/exceptions.md`（差分と理由を記録）
- PR を作成する場合：
  → 本書の「レビュー・チェックリスト」を自己確認する
- リリースに伴う変更履歴を記録する場合：
  → `docs/process/revision-history.md`
- ファイルベース設定（資格情報/ローカル設定/鍵パス等）を追加・変更した場合：
  → `docs/process/templates/` のテンプレート群を同時更新する（テンプレート未整備の仕様追加は禁止）

---

## 6. 例外運用（Decisions の規律）

### 6.1 例外の記録先
- 規範に従えない場合、必ず `docs/process/exceptions.md` に **差分と理由**を記録する。

### 6.2 例外として認められる条件（固定）
Decisions に記録できるのは、次のいずれかのみ。

1. 公式 API 仕様が原因で回避不能
2. 後方互換性維持のために必要
3. セキュリティ / 性能 / 法令上の不可避

※「便利だから」「実装が楽だから」「説明したいから」を理由とする例外は不可。

---

### 6.3 裁定が必要な場合の運用

inventory の Note に記載された状況（重複候補・旧版・非機能・version 並立等）により、
実装対象が自明でないと判断された場合のみ、
governance に従って裁定を行う。

裁定の結果は、inventory の `PresentIn` により必ず確定されなければならない。

* 採用する endpoint の `PresentIn` を確定する
* 非採用（どの層にも提供しない）とする endpoint の `PresentIn` を `None` として明示する（未指定のままにしない）

裁定を要しない場合、判断を追加してはならない。

---

## 7. レビュー・チェックリスト（PR 最終判断装置）

本チェックリストの目的は、実装の良し悪しの議論ではなく、
**規範（TopSpec）が守られているか**を機械的に確認することにある。

### 7.1 使い方
- PR 作成者は提出前に自己確認する。
- レビュー時は Yes / No で確認する。
- No がある場合、理由を明示する。
- 意図的な例外なら `docs/process/exceptions.md` に記録する（未記録の例外は禁止）。

---

### 7.2 必須（Merge 前に必ず確認）

#### A. 層と境界（TopSpec）
- [ ] 層の呼び出しが隣接層に限定され、層ジャンプがない
- [ ] Wire が JSON を解釈していない（text/bytes の意味段階を越えていない）
- [ ] `string` が Wire から下流へ漏れていない（in/out の型制約が守られている）
- [ ] Raw が lossless であり、意味確定（単位換算・時刻統一・解釈・デフォルト補完）をしていない
- [ ] Raw DTO / RawJson が公開面（Contracts）へ漏れていない
- [ ] 単一の API 呼び出し実装／単一インスタンス内部で、複数取引所の Raw / Normalized 型を混在させていない（TopSpec 3.4.4）

#### A2. 非層カテゴリ（TopSpec）
- [ ] `Application` は横断ユースケースの置き場であり、`Contracts.*` を参照していない
- [ ] Facade の Request/Interface を `Application` に直接流していない（Composition で変換している）
- [ ] `Composition` は組み立ての終端であり、実装都合が Contracts/層へ逆流していない

#### B. 契約（Contracts）
- [ ] 公開 API が Call-only（Response直返し禁止）
- [ ] Contracts に取引所名・取引所固有要素が混入していない
- [ ] DTO の命名、Nullable、Page/Cursor/Limit の規範を破っていない

#### C. Inventory（事実一覧）
- [ ] 外部 API の追加/変更がある場合、`docs/inventory/endpoints-<exchange>.md` に反映されている
- [ ] Official Reference（公式ドキュメントへの参照）が明示されている
- [ ] 取引所 endpoint inventory（`docs/inventory/endpoints-<exchange>.md`）に `PresentIn` 列が存在し、語彙が規定通り `{Wire, Raw, Normalized, Contracts, None}` のみである
- [ ] `PresentIn` に `Raw` を含む endpoint について、Raw 層に `<EndpointId>CallAsync` が存在する
- [ ] `PresentIn` に `Normalized` を含む endpoint について、Normalized 層に `<EndpointId>CallAsync` が存在する
- [ ] inventory の `EndpointId` 列に alias が混入していない（alias は `Aliases` 表にのみ記載されている）

#### D. 例外（Decisions）
- [ ] 原則からの逸脱がある場合、`docs/process/exceptions.md` に記録がある（未登録の例外は禁止）

#### E. 物理配置変更時の文書同期
- [ ] `<exchange>` 配下の物理配置を変更した場合、`docs/normative/topspec.md` と `docs/normative/layout/exchange-module-shape.json` を同一 PR で同期更新している
- [ ] `docs/normative/contracts/contracts.md` に物理配置の詳細を重複記載していないことを確認した
- [ ] 物理配置の変更を `tests/Common.Tests/Architecture/*LayoutParityTests.cs` へ反映し、`dotnet test` で検証している

#### F. 文書↔コード収束ゲート（レビューループ回避）
- [ ] 変更ごとに「収束アンカー（正本文書 1 つ + 主実装 1 箇所）」を明示している
- [ ] 乖離時の修正方向を `docs/normative/governance.md` の正本優先順で決定している
- [ ] 未解消論点を残していない（残す場合は `docs/process/exceptions.md` または `docs/process/CHANGE-*.md` に記録）
- [ ] 再レビューは前回 NG 指摘のみで実施し、新規指摘は重大回帰に限定している
- [ ] 同一論点が 3 回目でも未収束の場合、レビュー継続ではなく裁定へ切り替えている
- [ ] Stage9 終了判定では `docs/stage_9_close_policy.md` の必須条件と証跡要件を満たしている

---

## 8. Legacy（参考文書）

- 旧文書は互換・参照のため `docs/archive/references/` に保管する。
- `references` 配下は **Normative ではない**。
