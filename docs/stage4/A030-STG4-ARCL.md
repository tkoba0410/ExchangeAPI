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
ExchangeApi.Contracts
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

## 5. プロジェクト/フォルダ構成の指針（提案）
- `core/`: 抽象インターフェース（REST/WS/Info）、共通 DTO/Errors を置く（取引所非依存に徹する）
- `transport/`: HTTP/WS クライアント、RestClient/Signer、シリアライズ、時刻/シンボル変換など取引所非依存の I/O 基盤
- `adapter.bitflyer/`: bitFlyer 固有の REST/WS 実装と DTO/マッピング（必要に応じて rest/realtime でサブフォルダ分割）
- `factory/`: DI 組み立て。機能ごとの登録（Market/Trading/Margin/Realtime/ExchangeInfo）とフルセット登録を切り替えられるようにする
ポイント: 取引所固有のコードは adapter 側に閉じ込め、transport は共通基盤だけを置く。抽象と実装が混ざらないよう物理境界と依存方向を固定する。
