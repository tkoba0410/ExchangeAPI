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
- 要素 DTO 名は `Response` を付けず `*Item` を用いる。
- `Nullable` は型名・プロパティ名に含めない（nullable は型注釈で表す）。
- API 境界での `using XxxResponse = ...` による alias は段階的に廃止し、最終的に endpoint 直結の実体 DTO に統一する。
- `Result` 命名の採用可否は個別に判断し、混線リスクがある場合はレビューで明示する。
- `CallResult<T>`（呼出結果）と業務 DTO 名の `*Result` は責務が異なることを前提に扱う。

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
