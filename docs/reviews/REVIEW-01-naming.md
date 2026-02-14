# REVIEW-01: 命名統一レビュー（対象: 統一感 / 将来拡張時の保守性）

> 本レビューは **命名の統一性のみ** を対象とし、実装変更提案は行わない。  
> 観点は「将来の取引所追加・Endpoint追加時の事故防止」と「保守コスト最小化」。

---

## 評価サマリ

- 現状は `EndpointId` 由来命名を中心に整っているが、
  - `EndpointId` 方針差（Bitflyer / Bittrade）は、取引所差異許容の新ルールで運用可能な状態になった。
  - API 境界 DTO 直結は進み、`Result` 語の責務混在は主要箇所で解消した。
  - `*Normalized` 接尾辞は「原則不使用・衝突時のみ許可」に更新済み。
- 実装面の主要な命名不整合は収束し、残件は文書整合（レビュー本文・例外台帳の追従）が中心。

---

## P0（最優先: 先にルール固定しないと今後の追加で再発し続ける）

### P0-1. `EndpointId` 規則の基準文書が取引所間で実質分岐している

- Bittrade inventory は `Get / Post` prefix を許容。  
  - `docs/inventory/endpoints-bittrade.md` の EndpointId ルールに明記。
- Bitflyer inventory は末尾で「HTTP Method prefix を採用しない」旨を記載しつつ、実表は `Get*` 系中心、さらに `Markets` など重複候補も同列に記載。  
  - `docs/inventory/endpoints-bitflyer.md` Public テーブルに `GetMarkets` と `Markets` が併存（`duplicate candidate`）。

**リスク**
- 新規取引所追加時、`EndpointId` 命名の初期判断が担当者依存になる。
- Raw/Normalized/Tests の自動生成・自動検証の規則化が困難になり、命名差分レビューが都度必要になる。

**対応状況**
- 方針: `docs/naming-rules.md` に「取引所内一貫 + 取引所間差異許容」を明文化済み。
- inventory: alias は `Aliases` セクションへ分離済み（主表には正規 EndpointId のみ記載）。
- 判定: **対応済み**（運用ルール化完了）。

---

## P1（高優先: 放置すると同義語が蓄積して運用コスト増）

### P1-1. 同一Layer・同一概念で `*Normalized` 接尾辞の有無が揺れている

**具体箇所（更新後）**
- `OpenOrder` は Bitflyer / Bittrade で同名化済み。  
  - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/OpenOrder.cs`
  - `src/Exchanges/Bittrade/Normalized/Private/Dtos/OpenOrder.cs`
- `WithdrawResult` は内部 DTO から削除済み（API 境界 `...Response` へ統合）。

**リスク**
- 取引所横断で mapper / contract adapter を作る際に、
  型名だけで責務判定できず IDE 検索効率が落ちる。
- 「Normalized DTO には接尾辞を付けるか否か」の判断が都度発生。

**対応状況**
1. ルール: `docs/naming-rules.md` に反映済み。  
2. 実装: 主要不一致は解消済み。  
3. 判定: **方針更新済み / 実装概ね収束**。

---

### P1-2. Contracts層に EndpointId 直結語彙が混入し、語彙ポリシーが分岐している

**具体箇所**
- 現行 Contracts Request は業務語彙へ統一済み。  
  - `src/Contracts/Facade/Requests/TickerRequest.cs`  
  - `src/Contracts/Facade/Requests/BoardRequest.cs`

**リスク**
- Contracts が「取引所非依存語彙」なのか「EndpointId鏡像」なのかが曖昧になり、
  新規 endpoint 追加時の命名判断が二択化する。

**対応状況**
1. ルール: 業務語彙優先で明文化済み。  
2. 実装: `GetCurrenciesRequest` / `GetTimestampRequest` は Contracts から解消済み。  
3. 判定: **対応済み**。

---

## P2（中優先: 事故率は低いがレビューコストを増やす）

### P2-1. 外部仕様由来の `/currencys` 表記と内部命名の整合管理

**具体箇所**
- 外部 API path は `/v1/common/currencys` 表記を使用する。  
  - `docs/inventory/endpoints-bittrade.md`（Path 列）
- 内部命名は `GetCurrencies` / `GetCurrenciesRequest` / `GetCurrenciesResponse` へ統一済み。

**リスク**
- 外部 path typo と内部命名規則の境界が曖昧だと、将来追加時に追従範囲の判断が再発する。
- EndpointId/DTO と Path の修正責務を混同すると、互換性を壊す変更が混入する。

**対応状況**
1. ルール: typo 検出時は正本（inventory）を先に修正する方針へ更新済み。  
2. 実装: EndpointId / API 境界 DTO は `GetCurrencies*` へ修正済み。  
3. 判定: **対応済み**。

---

### P2-2. `Request / Response / Call / Result` の責務境界を型名だけで判定しにくい箇所がある

**具体箇所（更新後）**
- `OrderResult` / `CancelResult` / `WithdrawResult` / `RetailOrderResult` は API 境界 DTO 直結化の過程で削除済み。
- API 境界 DTO は `...Response` に集約され、`CallResult<T>` との責務分離が明確化された。

**対応状況**
1. API 境界 DTO 直結ルールは `docs/naming-rules.md` に反映済み。  
2. 主要実装は `...Response` 直結へ移行済み。  
3. 判定: **主要指摘は解消**。

---

## 命名「統一ルール案」（再策定 v2）

> 旧「提案版 v1」は不採用とする。  
> 理由: EndpointId を取引所横断で単一正準化する方針が、現行 TopSpec の「取引所固有ルール許容」と衝突するため。

1. **EndpointId 統一方針（取引所内一貫 + 取引所間差異許容）**
   - EndpointId は「取引所ごとの SoT（inventory）」を正本とし、取引所横断で同名統一は要求しない。
   - 新規取引所における初期命名方針（HTTP Method 語の採用/省略、単語境界粒度、衝突時の解消規則）は、裁定者が決定する。
   - 初期命名方針の決定後は、当該取引所 inventory の EndpointId ルールを唯一の基準として運用する。
   - 各 inventory の主表（EndpointId 列）には「正規 EndpointId」のみを記載する。
   - `duplicate candidate` / 旧呼称 / 別名は主表に置かず、`Aliases` セクションに分離する。

2. **Layer語彙固定（命名の責務境界）**
   - Wire: `Endpoint/Path/Query/Spec`
   - Raw: API境界DTOのみ `Request/Response`
   - Normalized: 層接尾辞は通常付与せず、衝突回避時のみ許可
   - Contracts: 公開 Facade 境界は取引所非依存の業務語彙を優先

3. **DTO接尾辞ルール**
   - `Request/Response` は API 境界 DTO のみに使用する。
   - 内部中間モデルは `Payload/Envelope/Entry/Item/Body/Encoded/Document` を使用する。
   - `Result` は曖昧性が高いため新規導入を抑制し、内部結果は `Outcome` 優先とする。

4. **typo/外部仕様追従ルール**
   - typo を検出した場合は、まず正本（inventory の EndpointId / RequestType / ResponseType）を修正する。
   - typo を既知のまま新規 API 境界 DTO や Contracts 公開境界へ導入しない。

5. **Cross-Exchange 命名整合ルール**
   - 同一 Layer・同一責務・同等フィールド契約の DTO は、可能な限り同名化する。
   - 互換性や段階移行で同名化できない場合は、`docs/exceptions.md` に理由・影響範囲・解消条件を登録する。

---

## 対応状況（2026-02-08）

1. P0-1 EndpointId 規則分岐
   - 状態: **対応済み**
   - 根拠: `docs/naming-rules.md` と各 inventory の `Aliases` 分離運用。

2. P1-1 `*Normalized` 接尾辞揺れ
   - 状態: **方針更新済み / 実装概ね収束**
   - 根拠: `OpenOrder` 同名化、`WithdrawResult` 系削除、ルール明文化。

3. P1-2 Contracts 語彙揺れ
   - 状態: **対応済み**
   - 根拠: EndpointId 直結語彙の Contracts 露出解消。

4. P2-1 typo 語彙上位露出
   - 状態: **対応済み**
   - 根拠: EndpointId / API 境界 DTO は `GetCurrencies*` へ統一し、外部 path `/currencys` のみ仕様追従として維持。

5. P2-2 `Result` 語の責務混在
   - 状態: **主要指摘は解消**
   - 根拠: API 境界 DTO を `...Response` へ統一し、内部 `*Result` DTO を整理。

---

## 運用提案（最小）

- 新規 endpoint 追加時の PR テンプレートに以下 3 問を固定化:
  1. EndpointId は正準規則に一致しているか
  2. Contracts 名は取引所非依存語彙か
  3. Cross-Exchange 既存同義語と命名一致しているか
