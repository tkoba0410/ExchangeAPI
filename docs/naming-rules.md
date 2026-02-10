# Naming Rules

本書は ExchangeAPI における命名規則を定義する。
TopSpec の原則を補完し、命名判断を機械的に再現可能にすることを目的とする。

## 1. 位置づけ

- 本書は命名規則の集約先とする。
- TopSpec と矛盾する場合は TopSpec を優先する。
- チェックリスト (`docs/checklists/*.md`) は本書を参照する。

## 2. EndpointId 運用

- EndpointId の正本は各取引所 inventory とする。
- 新規取引所の初期命名方針の策定は裁定者が行う。
- ただし、初期方針は機械的ルールとして文書化されていなければならない。
- 初期方針決定後は、当該取引所 inventory の EndpointId ルールを唯一基準として運用し、個別 EndpointId の解釈・命名判断に裁定者判断を介在させない。
- inventory 主表には正規 EndpointId のみを記載する。
- 旧呼称・別名・重複候補は `Aliases` または別表に分離する。

## 3. 未実装 API の記録

- 公式に存在するが実装しない API は、inventory 主表とは別表で管理する。
- 別表には少なくとも以下を記載する。
  - EndpointId
  - Method
  - Path
  - 未実装理由

## 4. 層別命名

- Wire: `Endpoint` / `Path` / `Query` / `Spec`
- Raw: `Raw` / `Json` / `Request` / `Response`
- Normalized: 取引所内意味語彙
- Contracts: 取引所非依存語彙

## 5. 層接尾辞 (`Wire`/`Raw`/`Normalized`/`Adapter`) の扱い

- `*Wire` / `*Raw` / `*Normalized` / `*Adapter` は通常付与しない。
- 同一コンパイル単位で型衝突または曖昧参照が発生する場合に限り、接尾辞付与を許可する。
- 接尾辞を付与した場合は、衝突元と解消理由を記録する。

## 6. API 境界 DTO の直結ルール

- 各層の API 境界 DTO（`RequestType` / `ResponseType`）は、対応する inventory の型定義に直結させる。
- `Contracts` 層は `docs/inventory/endpoints-contracts.md` の `RequestType` / `ResponseType` を正本とする。
- `Raw` / `Normalized` 層は、各取引所 inventory（`docs/inventory/endpoints-*.md`）の `RequestType` / `ResponseType` を正本とする。
- inventory に未定義の API 境界 DTO 名を新設してはならない。必要な場合は先に inventory を更新する。
- API 境界の `Call<TRequest, TOk>` における `TOk` は原則 non-null とする。
- `該当なし` / `空結果` / `0件` は、`ResponseType` 内の機構（例: `Found` / `Item` / `Items`）で表現する。
- `nullable` は API 境界 DTO そのもの（`TOk?`）ではなく、`ResponseType` 内部の表現として扱う。
- 配列応答は `ResponseType` を endpoint 直結名で維持し、DTO 内の `Items`（`IReadOnlyList<TItem>`）で表現する。
- 0..1 応答は `ResponseType` を endpoint 直結名で維持し、DTO 内の `Found` + `Item`（`TItem?`）で表現する。
- 単一オブジェクト応答（非配列・非nullable）は `ResponseType` をフラットに定義する。
- 上記ケースでの `Item` 単一プロパティによるラップは原則禁止とする。
- `Item` は `Items`（配列）または `Found + Item`（0..1）を表現する場合に限定して使用する。
- 要素 DTO 名は `Response` を付けず `*Item` を用いる。
- `Nullable` は型名・プロパティ名に含めない（nullable は型注釈で表す）。
- API 境界での `using XxxResponse = ...` による alias は段階的に廃止し、最終的に endpoint 直結の実体 DTO に統一する。
- 後方互換性維持や段階移行で単一 `Item` ラップを残す場合は、`docs/exceptions.md` に理由・影響範囲・解消条件を記録する。
- `Result` は `CallResult<T>`（呼出結果コンテナ）に限定して使用する。
- API 境界 DTO / 内部 DTO に新規 `*Result` 命名を導入してはならない。
- 内部結果モデルは `Outcome` を優先し、やむを得ず逸脱する場合は `docs/exceptions.md` に理由・影響範囲・解消条件を記録する。

### 6.1 例外

- `Internal` / `Composite`（例: `ExchangeInfo`）は endpoint 直結の対象外とする。
- 例外を採用する場合は、`docs/exceptions.md` に以下を記録する。
  - 対象 API
  - 非直結とする理由
  - 影響範囲
  - 解消条件

## 7. 例外運用

- 命名例外は `docs/exceptions.md` に登録する。
- 登録時は以下を必須とする。
  - 例外内容
  - 影響範囲
  - 採用理由
  - 解消条件

## 8. 外部仕様由来 typo の扱い

- typo を検出した場合は、まず正本（各取引所 inventory の EndpointId / RequestType / ResponseType）を修正する。
- typo を既知のまま残した名称を、新規の API 境界 DTO や Contracts 公開境界（RequestType / ResponseType / ContractApiId）に導入してはならない。
- 正本修正に伴い影響が出る場合は、`docs/inventory/endpoints-contracts.md` と関連実装を同一変更で追従させる。

## 9. CancellationToken 命名規約

- `CancellationToken` 型の引数名は `cancellationToken` のみ許可する。
- `CancellationToken` 引数はメソッド引数の末尾に配置する。
- 既定値は原則 `= default` を付与する。
- `ct` は新規追加を禁止する。
- 既存 `public`/`protected` API の改名は named argument 互換性に配慮し、メジャー更新時に実施する。

## 10. Normalized 以降の型ルール

- Raw 層を除き、業務意味を持つ値を `string` / `int` / `long` / `decimal` で受け渡してはならない。
- Normalized / Adapter / Contracts では、業務値は ValueObject または enum で表現する。
- `string` は外部 I/O 境界（HTTP/JSON, 設定入力）でのみ許可し、境界通過時に VO/enum へ変換する。
- 変換責務は Raw -> Normalized 入口に集約し、下流層で再変換しない。
- `DateTimeOffset` / `bool` / `CancellationToken` / コレクションは本規約の対象外とする。
- 例外が必要な場合は `docs/exceptions.md` に理由・影響範囲・解消条件を記録する。
