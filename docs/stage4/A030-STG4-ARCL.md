# A030-STG4-ARCL Stage4 レイヤ構成（REST+WS 抽象確定）

## 1. 依存方向
```
Abstractions (6 modules)
     ↓
Infrastructure / Adapter（実装は Stage5 以降）
     ↓
Exchange-specific API / Transport
     ↓
Factory
```

## 2. レイヤ別の役割
- **Abstractions**: Market / Trading / Account / Margin / Realtime / ExchangeInfo の 6 区分で薄いインターフェースと最小ドメイン型を提供する。Stage4 の主成果。
- **Infrastructure/Adapter**: 取引所固有 DTO ↔ Domain の変換、HTTP/WS 呼び出し、エラー分類フックを担当。Stage5 以降で実装。
- **Exchange API (bitFlyer など)**: 実際の REST/WS エンドポイント呼び出し。Stage4 では参照のみ。
- **Factory**: 取引所実装を組み立てる入口。Stage5 以降で拡張。

## 3. コンポーネント構成（Stage4 範囲）
```
ExchangeApi.Abstractions
  ├─ Domain: Ticker / OrderBook / Execution / OrderRequest / OrderResult / OpenOrder
  ├─ Domain: Position / Collateral（Margin 用・最小）
  ├─ REST Interfaces:
  │    IMarketDataApi (GetTicker / GetOrderBook / GetExecutions)
  │    ITradingApi (SendOrder / CancelOrder / GetOpenOrders)
  │    IAccountApi (GetBalances)
  │    IMarginAccountApi : IAccountApi (GetOpenPositions / GetCollateral)
  ├─ WS Interface:
  │    IRealtimeMarketDataApi (SubscribeTicker / SubscribeOrderBook / SubscribeExecutions)
  └─ ExchangeInfo Interface:
       IExchangeInfoApi (スケルトン入口)
```

## 4. データフローの特徴（設計ガイド）
- REST: Client → Abstractions → Adapter (Stage5+) → Transport → Exchange API → Adapter → Domain
- WS: Client → Abstractions → WS Adapter (Stage5+) → WS Stream → Domain events
- Raw: 抽象化できない機能は Raw API として Adapter/Factory 側に逃がす（Stage4 では設計対象外）
