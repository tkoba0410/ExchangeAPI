# Exchange Parity Policy（取引所間の統一/例外運用）

本書は **取引所実装間**（例：Bitflyer / Bittrade）において、
「どこまで統一し、どこから差異を許容するか」を運用上の方針として記録する。

本書は **非規範（Reference）** である。技術規範の正本は `docs/topspec.md`、
EndpointId の正本は各 `docs/inventory/endpoints-*.md` とする。

## 1. 目的

- 取引所間の差異を「仕様差（不可避）」と「実装差（回避可能）」に分離する
- 実装差（回避可能）については、レビュー観点・改修方針を固定する
- 参照実装を Bitflyer に保ちつつ、他取引所は段階的に追随可能にする

## 2. 統一するもの（MUST）

### 2.1 物理配置（骨格）

- Wire / Raw / Normalized / Adapter の各層で、**Public / Private / Internal** を基本骨格として揃える
- **意味分類（MarketData / Trading / Account など）でフォルダ分割しない**（Public/Private 以外で分けない）
- 生成・組み立て用途の `Factory` は **Internal 配下に置く**（例：`.../Internal/Factory`）
- `Internal` は各層の直下に置く（例：`.../Normalized/Internal`）。Public/Private 配下には置かない

### 2.2 namespace

- namespace は物理配置に一致させる（例：`ExchangeApi.Exchanges.<Exchange>.<Layer>.Public.*`）

### 2.3 EndpointId とメソッド名

- inventory の `EndpointId` を正として、Wire/Raw/Normalized は原則 **`<EndpointId>CallAsync`** を 1:1 対応させる
- 対応対象は inventory の `PresentIn` により決定する（`None` は未実装として扱う）

## 3. 統一しないもの（MAY）

- 取引所固有の機能差（例：片方に存在しない endpoint 群）
- 内部実装の詳細（DTO の内部表現、マッパの分割、最適化手法など）
- Public 側の “Facade/Api” といった命名・構造パターン（統一する場合は別途タスク化）
- `Properties` など **ビルドメタ情報の配置**は例外として許容する

## 3.1 揺らぎ禁止（MUST NOT）

- `Internal` の配置階層を取引所ごとに変えること
- Raw API の入口配置（例：`Raw/Api` の有無）を取引所ごとに変えること
- Factory の配置を `Internal` 以外に置くこと

## 4. Bitflyer を参照実装とする範囲（SHOULD）

- 新規に層構造/命名/例外処理パターンを導入する際は、原則として Bitflyer の実装を参照する
- ただし Bitflyer の現状が TopSpec に反する場合は TopSpec を優先する

## 5. 差異の記録ルール

- **事実（Fact）**：inventory の `Note` で記録してよい（例：duplicate candidate、obsolete candidate）
- **運用方針/設計判断**：inventory に書かず、本書（または設計メモ）に記録する

## 6. 変更時チェックリスト

1. inventory を更新（EndpointId / PresentIn）
2. Wire: `Endpoints/` と定数（Path/Query/Traits 等）を追加
3. Raw: inventory と 1:1 の `*CallAsync` を追加
4. Normalized: inventory と 1:1 の `*CallAsync` を追加（取引所内正規化）
5. Adapter: Contracts への適合のみ（取引所固有 endpoint を公開しない）
