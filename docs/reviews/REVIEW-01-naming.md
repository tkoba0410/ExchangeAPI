# REVIEW-01: 命名統一レビュー（対象: 統一感 / 将来拡張時の保守性）

> 本レビューは **命名の統一性のみ** を対象とし、実装変更提案は行わない。  
> 観点は「将来の取引所追加・Endpoint追加時の事故防止」と「保守コスト最小化」。

---

## 評価サマリ

- 現状は `EndpointId` 由来命名を中心に整っているが、
  - **同一概念の語彙揺れ**（例: `Result` 語の責務混在）
  - **Layer内での接尾辞ポリシー不一致**（`*Normalized` 有無）
  - **EndpointId規則の文書間不整合**（Bitflyer と Bittrade の方針差 + Bitflyer inventory 内の重複候補併記）
  が残る。
- 重大事故に直結する命名崩れ（型衝突や誤マッピング誘発）は少ないが、
  将来の拡張速度を落とす「命名意思決定の分岐点」が増えている。

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

**統一ルール案**
1. `EndpointId` は「**Method 非依存の意味名**」を正準とし、Method 衝突時のみ `Public/Private` または `ByXxx` で解消。  
2. 互換のため Method 付きIDが必要な場合は「Alias」としてのみ管理し、Inventory主表には載せない。  
3. `EndpointId` 規則は 1 文書（TopSpec相当）に一本化し、各取引所inventoryは参照のみ。

---

## P1（高優先: 放置すると同義語が蓄積して運用コスト増）

### P1-1. 同一Layer・同一概念で `*Normalized` 接尾辞の有無が揺れている

**具体箇所（現状）**
- Bitflyer: `OpenOrder`  
  - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/OpenOrder.cs`
- Bittrade: `OpenOrder`  
  - `src/Exchanges/Bittrade/Normalized/Private/Dtos/OpenOrder.cs`

- Bitflyer: `WithdrawResult`  
  - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/WithdrawResult.cs`
- Bittrade: `WithdrawResult`  
  - `src/Exchanges/Bittrade/Normalized/Private/Dtos/WithdrawResult.cs`

**リスク**
- 取引所横断で mapper / contract adapter を作る際に、
  型名だけで責務判定できず IDE 検索効率が落ちる。
- 「Normalized DTO には接尾辞を付けるか否か」の判断が都度発生。

**統一ルール案**
1. `*Normalized` は原則付与しない。
2. 同一コンパイル単位で型衝突または曖昧参照が発生する場合に限り `*Normalized` を許可する。
3. 付与時は衝突元と解消理由を記録する（必要に応じて `docs/exceptions.md`）。

---

### P1-2. Contracts層に EndpointId 直結語彙が混入し、語彙ポリシーが分岐している

**具体箇所**
- Contracts Request で通常語彙（`TickerRequest`, `BoardRequest`）と EndpointId 由来語彙（`GetCurrencysRequest`, `GetTimestampRequest`）が混在。  
  - `src/Contracts/Facade/Requests/TickerRequest.cs`  
  - `src/Contracts/Facade/Requests/GetCurrencysRequest.cs`  
  - `src/Contracts/Facade/Requests/GetTimestampRequest.cs`

**リスク**
- Contracts が「取引所非依存語彙」なのか「EndpointId鏡像」なのかが曖昧になり、
  新規 endpoint 追加時の命名判断が二択化する。

**統一ルール案**
1. Contracts層は **業務語彙（非EndpointId）固定**（例: `CurrenciesRequest`, `ServerTimeRequest`）。
2. EndpointId由来名は Raw/Normalized までに閉じ込め、Contracts へは mapper で吸収。

---

## P2（中優先: 事故率は低いがレビューコストを増やす）

### P2-1. `Currencys` の綴りを許容する範囲が明文化不足

**具体箇所**
- `GetCurrencys` 系が Raw/Normalized/Contracts まで浸透。  
  - 例: `src/Contracts/Facade/Requests/GetCurrencysRequest.cs`  
  - 例: `docs/inventory/endpoints-bittrade.md`

**リスク**
- typo由来命名が上位層へ漏れ、検索性と学習コストが継続的に悪化。

**統一ルール案**
1. typoを含む名称は **EndpointId（およびその直結型）に限定**。  
2. 上位層（Contracts / Application公開API）では正規英語へ正規化。

---

### P2-2. `Request / Response / Call / Result` の責務境界を型名だけで判定しにくい箇所がある

**具体箇所（代表）**
- Normalized Private DTO に `OrderResult` / `CancelResult` 等が存在し、
  Contracts の `*Response` と並んだ時に「API境界DTOか内部結果DTOか」が名前だけでは直感しづらい。  
  - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/OrderResult.cs`  
  - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/CancelResult.cs`  
  - `src/Exchanges/Bittrade/Normalized/Private/Dtos/OrderResult.cs`  
  - `src/Exchanges/Bittrade/Normalized/Private/Dtos/CancelResult.cs`

**統一ルール案**
1. API境界: `Request/Response`。
2. 呼出メタ: `Call<TReq, TOk>`（既存運用を維持）。
3. 内部変換結果: `*Outcome`（`*Result` との混線回避）を推奨。

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
   - 外部仕様由来の typo（例: `Currencys`）は EndpointId およびその直結 DTO まで許容する。
   - Contracts 公開境界へ露出する名称は正規英語（例: `Currencies`）を優先する。

5. **Cross-Exchange 命名整合ルール**
   - 同一 Layer・同一責務・同等フィールド契約の DTO は、可能な限り同名化する。
   - 互換性や段階移行で同名化できない場合は、`docs/exceptions.md` に理由・影響範囲・解消条件を登録する。

---

## 逸脱箇所一覧（抽出）

1. `EndpointId` 方針の不整合
   - `docs/inventory/endpoints-bitflyer.md`（`GetMarkets` と `Markets` の重複候補併記、prefix方針記述とのねじれ）
   - `docs/inventory/endpoints-bittrade.md`（`Get/Post` prefix 許容）

2. Normalized DTO 接尾辞揺れ（現時点では主要例は解消）
   - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/OpenOrder.cs`
   - `src/Exchanges/Bittrade/Normalized/Private/Dtos/OpenOrder.cs`
   - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/WithdrawResult.cs`
   - `src/Exchanges/Bittrade/Normalized/Private/Dtos/WithdrawResult.cs`

3. Contracts語彙の揺れ（業務語彙 vs EndpointId語彙）
   - `src/Contracts/Facade/Requests/TickerRequest.cs`
   - `src/Contracts/Facade/Requests/GetCurrencysRequest.cs`
   - `src/Contracts/Facade/Requests/GetTimestampRequest.cs`

4. typo語彙の上位層露出
   - `src/Contracts/Facade/Requests/GetCurrencysRequest.cs`
   - `docs/inventory/endpoints-bittrade.md`

5. `Result` 語の責務混在余地
   - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/OrderResult.cs`
   - `src/Exchanges/Bitflyer/Normalized/Private/Dtos/CancelResult.cs`
   - `src/Exchanges/Bittrade/Normalized/Private/Dtos/OrderResult.cs`
   - `src/Exchanges/Bittrade/Normalized/Private/Dtos/CancelResult.cs`

---

## 運用提案（最小）

- 新規 endpoint 追加時の PR テンプレートに以下 3 問を固定化:
  1. EndpointId は正準規則に一致しているか
  2. Contracts 名は取引所非依存語彙か
  3. Cross-Exchange 既存同義語と命名一致しているか
