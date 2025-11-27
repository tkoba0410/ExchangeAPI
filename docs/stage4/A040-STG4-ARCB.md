# A040-STG4-ARCB Stage4 API マッピング（bitFlyer Private 拡張）

## 1. Private GET → Domain
- `/v1/me/getpositions` → `Position` コレクション  
  - fields: product_code, side, size, price, pnl, open_date など → Domain へ正規化
- `/v1/me/getexecutions` → `Execution` コレクション  
  - fields: id, side, price, size, exec_date, child_order_acceptance_id など
- `/v1/me/getcollateral` → `Collateral`  
  - fields: collateral, open_position_pnl, require_collateral, keep_rate など

## 2. Private POST → Domain
- `/v1/me/cancelchildorder` → `CancelResult`（成功/失敗のみを扱う簡易型で可）  
  - request: product_code, child_order_acceptance_id（または order_id）  
- `/v1/me/cancelallchildorders` → `CancelResult`  
  - request: product_code

## 3. 注文送信（拡張）
- `/v1/me/sendchildorder` を LIMIT/STOP/time_in_force/minute_to_expire に対応  
  - Domain: `OrderRequest`  
    - side / order_type(MARKET/LIMIT/STOP) / price / trigger_price / minute_to_expire / time_in_force / size / product_code  
  - DTO: bitFlyer sendchildorder のパラメータへマッピング（不要フィールドは省略）

## 4. エラー分類（E2）
- HTTP ステータス + bitFlyer エラーコードを受け取り、カテゴリを判定する  
  - 例: `INSUFFICIENT_FUNDS`, `INVALID_ORDER`, `TIMEOUT`, `TOO_MANY_REQUESTS` など  
- 例外は ExchangeApiException を拡張し、`ExchangeErrorCode`（仮称）を保持して Adapter から抽象層へ伝搬
- 代表コード表（ドラフト）  
  - `INSUFFICIENT_FUNDS` → `Category: Balance` → Retry: No  
  - `INVALID_ORDER` → `Category: Request` → Retry: No（入力修正が必要）  
  - `TIMEOUT` → `Category: Network` → Retry: Yes（再試行候補）  
  - `TOO_MANY_REQUESTS` → `Category: RateLimit` → Retry: Yes（ヘッダの待機時間を尊重）  
  - `PRODUCT_NOT_FOUND` / `INVALID_PRODUCT` → `Category: Request` → Retry: No  
  - `SERVICE_UNAVAILABLE` → `Category: Server` → Retry: Yes（バックオフ）  
  - `INTERNAL_ERROR` → `Category: Server` → Retry: Yes（バックオフ）  
  - `AUTHENTICATION_ERROR` → `Category: Auth` → Retry: No（キー確認が必要）  
  ※正式なコード一覧は bitFlyer ドキュメントに合わせて精査し、マッピングテーブルとして実装する。

## 5. DTO/Domain の指針
- Domain は最小限の共通概念（価格/サイズ/日時/ID/サイド）に寄せ、取引所固有名は DTO 側に閉じ込める
- time_in_force は bitFlyer の `GTC/IOC/FOK` を列挙型として扱い、未対応値は明示エラーとする
- minute_to_expire は 0/未指定を許容（bitFlyer の仕様に合わせる）
