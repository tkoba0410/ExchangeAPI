# REVIEW-02: メソッド引数の順番・形の統一性レビュー

対象: ExchangeAPI リポジトリ（将来の取引所追加・endpoint追加時の保守性/事故防止観点）

## 結論（要約）
- **Facade 層（`Contracts.Facade`）は比較的統一されている**。`Request DTO + CancellationToken` の形が中心で、引数順も安定。
- 一方で **Adapter/Normalized 層で「DTO受け」と「プリミティブ受け」の混在** が見られ、同じ操作でも層ごとに設計が揺れている。
- また **CancellationToken の命名 (`ct` / `cancellationToken`) が混在**、**プリミティブ（`int` / `decimal`）の意味づけ不足**、**string 境界の扱いルール未明確** が今後の拡張時の事故要因になる。

---

## 統一ルール案（引数設計）

### 1) 公開境界（Facade / ExchangeClient 実装）
- 原則: **`<OperationRequest request, CancellationToken cancellationToken = default>` の2引数のみ**。
- 例外: 引数なし操作のみ `CancellationToken` 単独許可（ただし request 空 record の採用を推奨）。

### 2) 内部境界（Adapter / Normalized）
- 原則: **DTO受けに統一**（内部で分解して下位に渡す）。
- 同一操作において `request` 版と `primitive` 版を併存させない。

### 3) DTOフィールド順序ルール
- フィールド順序は以下に固定:
  1. 識別子（Symbol / OrderKey / ProductCode など）
  2. 期間（Period / Start-End）
  3. ページング（Limit / Cursor / Size）
  4. オプション（Filter / Flags）
- メソッド引数も同順序を維持し、**最後に `CancellationToken`**。

### 4) 型ルール（プリミティブ抑制）
- `string` / `int` / `decimal` を業務意味を持つ引数として直接使わない。
- 可能な限り **ValueObject / enum / record** に昇格。
- `string` は原則「外部I/O境界（Raw HTTP/JSON）」でのみ使用し、境界通過時に VO へ変換。

### 5) record / sealed class / ValueObject 使い分け
- **request/response**: `sealed record`（不変・比較しやすい）。
- **振る舞い主体サービス**: `sealed class`。
- **業務値（ID/Code/Side/Type/Price/Size）**: ValueObject または enum。

---

## 逸脱一覧（P0 / P1 / P2）

## P0（重大: 将来拡張時に高確率で事故化）
- 該当なし（現時点で即時障害級の引数順崩れは限定的）。

## P1（高: 拡張時に不整合・実装漏れを誘発）

1. **同一関心で DTO 受けとプリミティブ受けが混在**
   - 例: Normalized 公開IFは request DTO を受けるが、委譲先 private 実装では `Symbol, OrderKey` などに分解して受ける経路がある。
   - 影響: endpoint 追加時に、DTO拡張項目が途中層で落ちる/順序ミスする温床。

2. **Request を受けるのに実質利用しないメソッドが存在**
   - `GetOrdersCallAsync(GetOrdersRequest request, ...)` が request を使わず下位へ委譲。
   - 影響: 将来 `GetOrdersRequest` にフィールド追加しても反映漏れが起きやすい。

3. **ページング情報の扱いが不統一（Cursor が黙殺される経路）**
   - `ExecutionsPrivateRequest`/`OrdersRequest` は `Cursor` を持つが、Adapter 側の呼び出しは limit 中心で cursor を実質使わない経路がある。
   - 影響: 取引所追加時に「対応しているつもり」のページング不具合が入りやすい。

## P2（中: 可読性・規約運用コストに影響）

1. **CancellationToken 命名揺れ（`ct` / `cancellationToken`）**
   - 影響: 検索性/レビュー効率低下。規約チェック自動化もしづらい。

2. **ドメイン値にプリミティブが残る箇所**
   - 例: `int Direct`, `int? Status`, `int Type`, `decimal Amount` 等。
   - 影響: 値域制約が型で表現されず、endpoint 追加時の不正値混入リスク上昇。

3. **string の内部流通ルールが曖昧**
   - 例: `accountId` を string で受けて内部で `FreeText` 化する経路。
   - 影響: 境界が曖昧になり、将来の入力検証責務が分散しやすい。

---

## 推奨アクション（ルール化のみ）

1. **引数規約を ADR 化**
   - 「Facade=必ずRequest DTO」「内部境界も原則DTO」「最後は `CancellationToken cancellationToken` 固定」を明文化。

2. **静的チェック導入（命名/順序）**
   - Roslyn Analyzer もしくはテストで、`CancellationToken` 名称・末尾配置・DTO受け原則を検査。

3. **プリミティブ昇格バックログ化**
   - `Direct` / `Status` / `Type` / `Amount` などを対象に VO/enum 化優先度を定義。

4. **ページング対応方針を統一**
   - `Cursor` 非対応 endpoint は DTO から除外するか、`NotSupported` を明示して誤用を防止。

