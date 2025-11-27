# A030-STG4-ARCL Stage4 レイヤ構成（Private 横展開 + 注文強化）

## 1. 依存方向
```
Abstractions  ←  Infrastructure  ←  Bitflyer (API/Adapter)  ←  Factory
```

## 2. レイヤ別の役割
- **Abstractions**: Domain モデル（Position/Execution/Collateral/OrderRequest 拡張）と抽象インターフェース（Trading/Account）
- **Infrastructure**: RestClient/Signer の拡張（キャンセル系・E2 エラー・レートリミットフック）
- **Bitflyer API**: Private GET/POST DTO と HTTP 呼び出し実装
- **Bitflyer Adapter**: DTO ⇄ Domain マッピング、E2 エラー分類の適用
- **Factory**: bitFlyer 向け DI 構築（GET/POST 全てを一括登録）

## 3. コンポーネント構成（Stage4 範囲）
```
ExchangeApi.Abstractions
  ├─ Domain: Position / Execution / Collateral
  ├─ Domain: OrderRequest (price / minute_to_expire / time_in_force / trigger_price)
  └─ Interfaces: IExchangeTradingClient (PlaceOrderAsync, Cancel..., GetPositions..., GetExecutions..., GetCollateral)

ExchangeApi.Infrastructure
  ├─ IRequestSigner（キャンセル系・拡張パラメータ対応）
  ├─ IRestClient (GET/POST 拡張, E2 エラー処理)
  ├─ ExchangeApiException (+ exchange code/category)
  └─ RateLimit hook (インターフェース/設定のみ)

ExchangeApi.Bitflyer.PrivateApi
  ├─ DTO: Positions / Executions / Collateral
  ├─ DTO: CancelChildOrder / CancelAllChildOrders
  └─ BitflyerPrivateApi (GET/POST 拡張)

ExchangeApi.Bitflyer.Adapter
  ├─ Mapping: DTO → Domain (positions/executions/collateral)
  ├─ Mapping: Domain → DTO (orders/cancel)
  └─ E2 Error mapping (bitFlyer code → typed exception)

ExchangeApi.Factory
  └─ BitflyerClientFactory (GET/POST/Trading を全登録、設定を集約)
```

## 4. データフローの特徴
- GET 系：RestClient.GET → PrivateApi DTO → Adapter で Domain に変換
- POST 系：Domain → Adapter DTO → PrivateApi.POST → DTO → Adapter で Domain/Result に変換
- エラー：RestClient/Adapter で HTTP + bitFlyer code を分類し、E2 例外として抽象層へ伝播
