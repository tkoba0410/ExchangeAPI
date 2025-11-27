# A050-STG4-IMPL Stage4 実装ノート（Private 横展開 + 注文強化）

## 1. 抽象/ドメイン
- `OrderRequest` を拡張（price, trigger_price, minute_to_expire, time_in_force）し、バリデーションを追加する
- `Position` / `Execution` / `Collateral` のドメインモデルを追加し、将来の他取引所でも使える最小共通項に寄せる
- エラー型に exchange code/category を追加（E2）。HTTP ステータス + bitFlyer code を保持。
- 注文バリデーションの組み合わせルール（ドラフト）  
  - MARKET: 必須=product_code, side, size / 禁止=price, trigger_price / minute_to_expire・time_in_force は任意（未指定=デフォルト）  
  - LIMIT: 必須=product_code, side, size, price / trigger_price 禁止 / minute_to_expire/time_in_force 任意  
  - STOP: 必須=product_code, side, size, trigger_price / price 任意（未指定→STOP 成行、指定→STOP_LIMIT）/ minute_to_expire/time_in_force 任意  
  - 共通: price>0, trigger_price>0, size>0。未指定フィールドは送信しない。デフォルト time_in_force は GTC 前提。

## 2. Infrastructure
- `IRestClient`/`RestClient`: cancelchildorder/cancelallchildorders のクエリ/POST混在を扱えるようにする
- `IRequestSigner`: クエリとボディの両方を考慮した署名を再確認
- レートリミット: ハンドラ/デコレータの挿入口を用意（実装は簡易カウンタまたは未実装でも可）
- ExchangeApiException: エラーコード/カテゴリを持たせ、Adapter で利用できる形にする
- RateLimit フック例:  
  - Transport/RestClient の前後にデコレータを差し込み、429/Retry-After を検知して待機/失敗を選択できるようにする  
  - 設定例: `RateLimit: { Enabled, MaxRequestsPerSec, BackoffBaseMs, MaxBackoffMs }`

## 3. Bitflyer Private API
- GET DTO: positions/executions/collateral を追加
- POST DTO: cancelchildorder/cancelallchildorders を追加
- sendchildorder DTO: limit/stop/time_in_force/minute_to_expire を含むパラメータ対応
- PrivateApi 実装: 新 API を RestClient 経由で呼び出し、DTO を返す

## 4. Adapter
- Domain → DTO: OrderRequest を bitFlyer パラメータへ変換（不要な項目は送らない）
- DTO → Domain: positions/executions/collateral のマッピング
- エラー分類: HTTP + bitFlyer code を E2 にマッピングし、typed 例外を投げる

## 5. Factory/設定
- Factory で新 API/クライアントを一括登録し、設定（product_code, API keys, timeouts, rate-limit hooks）を受け取れるようにする
- 既存の Stage3 設定との後方互換を保つ（既存利用者の破壊的変更を避ける）

## 6. ドキュメント/DX
- Quick Start を更新（LIMIT/STOP/キャンセル/ポジション取得の最小例）
- README を利用者向け/開発者向けに再編（エントリールートを明示）
- Stage4 A0xx の内容と整合するようにサンプル/コード断片を揃える
