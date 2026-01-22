# EndpointId 規範（common）

## 1. 目的

本書は、本リポジトリにおける **EndpointId** の意味と最小運用ルールを定義する。

- 設計・層責務・公開範囲・Call 抽象などの規範は **TopSpec（docs/topspec.md）** を正本とする。
- 公式 API 文書を仕様の正本とし、本リポジトリでは公式を置き換えない。
- 本書は EndpointId を「一覧（inventory）」と結びつけるための **最小の共通取り決め**のみを扱う。

---

## 2. EndpointId の定義

EndpointId は、**取引所ごとの API Endpoint を一意に識別するための識別子**である。

- EndpointId は **取引所内で一意**でなければならない。
- EndpointId は **文字列そのものではなく識別子（定数名 / enum 名 / 静的メンバ名）**として扱う。
- EndpointId は、仕様詳細（Request/Response、paging、error 等）を表現しない。

---

## 3. EndpointId の責務と非責務

### 3.1 責務

EndpointId の責務は、次に限定される。

- endpoint を識別する
- 公式 API 上の endpoint（Method/Path/Scope）と対応付ける

### 3.2 非責務（保証しない）

EndpointId は以下を保証しない。

- Request / Response の構造や型
- paging / cursor / limit 等の振る舞い
- Capability として提供されるか否か
- 上位 API（Facade / Application 等）の存在

これらは TopSpec に定義された各層および Contract（Capability/DTO）の責務である。

---

## 4. inventory との対応（一覧）

本リポジトリでは、取引所ごとの endpoint 一覧（inventory）を管理する。

- 公式 API 文書が正本
- inventory は **対応関係の一覧**であり、規範ではない

### 4.1 inventory に記載する最小項目

各 endpoint は少なくとも以下で列挙される。

- Scope（public/private）
- Method（HTTP Method）
- Path（公式 API の path）
- EndpointId（本リポジトリの識別子）

inventory の置き場所（推奨）：

- `docs/inventory/endpoints-<exchange>.md`

※ inventory には詳細仕様説明や設計判断を記載しない。設計規範は TopSpec を参照すること。

---

## 5. 表記の最小制約

EndpointId の表記は次の最小制約を満たす。

- PascalCase
- `/` を含めない（Path をそのまま埋め込まない）

取引所ごとの命名上の流儀（単語境界の細かさ、Method を含める/含めない等）は、
各取引所の補助資料（例：`docs/endpoint-id/endpointid-<exchange>.md`）に委ねる。
ただし、命名規範の正本は TopSpec を参照すること。

---

## 6. 追加・変更の運用原則

- 新しい endpoint を扱う場合、まず inventory を更新する。
- inventory に列挙されていない endpoint は、本リポジトリでは未定義として扱う。
- EndpointId の命名に迷いが生じた場合は、当該取引所の endpoint-id 補助資料に判断理由を記録してよい（MAY）。
  ただし、設計規範（層責務・公開範囲等）は TopSpec に従う。

---

## 7. 派生（参考）

EndpointId から API メソッド名や各層の命名を機械的に派生させることは可能である。
具体規則は TopSpec およびコード反映規約（該当文書）に委ねる。
