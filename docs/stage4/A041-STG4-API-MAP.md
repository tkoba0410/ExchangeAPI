# A041-STG4-API-MAP 抽象 API と bitFlyer 入口（参考）

Stage4 は抽象 API を凍結するステージであり、実装は Stage5 以降。bitFlyer を例に、6 区分ごとに抽象の対応範囲と実装ステージを示す。

## 0. 分類（6 区分）
- REST: Market / Trading / Account / Margin
- WS: Realtime
- Other: ExchangeInfo（スケルトン）

## 1. 抽象インターフェース対応表（参考マッピング）
| 区分 | 抽象 IF / メソッド | プロトコル | 抽象範囲 | bitFlyer 参考 API | Stage4 スコープ | 実装ステージ |
| --- | --- | --- | --- | --- | --- | --- |
| Market | IMarketDataApi.GetTicker | REST | スナップショット | GET `/v1/getticker` | 抽象定義 | Stage5+ |
| Market | IMarketDataApi.GetOrderBook | REST | スナップショット | GET `/v1/getboard` | 抽象定義 | Stage5+ |
| Market | IMarketDataApi.GetMarketExecutions | REST | スナップショット（市場約定） | GET `/v1/getexecutions` | 抽象定義 | Stage5+ |
| Market | IMarketDataApi.GetCandlesticks | REST | OHLCV | （bitFlyer 非対応、bittrade を初期ターゲット） | 抽象定義 | Stage5+ |
| Trading | ITradingApi.SendOrder | REST | MARKET/LIMIT/STOP/STOP_LIMIT | POST `/v1/me/sendchildorder` | 抽象定義 | Stage5+ |
| Trading | ITradingApi.CancelOrder | REST | 単一キャンセル | POST `/v1/me/cancelchildorder` | 抽象定義 | Stage5+ |
| Trading | ITradingApi.GetOpenOrders | REST | Open/Active | GET `/v1/me/getchildorders` | 抽象定義 | Stage5+ |
| Account | IAccountApi.GetBalances | REST | 現物残高 | GET `/v1/me/getbalance` | 抽象定義 | Stage5+ |
| Account | IAccountApi.GetAccountExecutions | REST | 口座約定履歴 | GET `/v1/me/getexecutions` | 抽象定義 | Stage5+ |
| Margin | IMarginAccountApi.GetOpenPositions | REST | 建玉一覧 | GET `/v1/me/getpositions` | 抽象定義 | Stage5+ |
| Margin | IMarginAccountApi.GetCollateral | REST | 証拠金サマリ | GET `/v1/me/getcollateral` | 抽象定義 | Stage5+ |
| Realtime | IRealtimeMarketDataApi.SubscribeTicker | WS | 配信購読 | WS `ticker` | 抽象定義 | Stage5+ |
| Realtime | IRealtimeMarketDataApi.SubscribeOrderBook | WS | 配信購読 | WS `board` | 抽象定義 | Stage5+ |
| Realtime | IRealtimeMarketDataApi.SubscribeExecutions | WS | 配信購読 | WS `executions` | 抽象定義 | Stage5+ |
| ExchangeInfo | IExchangeInfoApi | REST | 市場/機能情報の入口 | （将来決定） | 抽象定義 | Stage5+ |

## 2. ExchangeInfo の返却例（スケルトン）
- Markets: 現状スケルトン（例: `[{ Symbol: "BTC/JPY", ProductCode: "BTC_JPY", Type: Spot, MinSize, PriceIncrement, SizeIncrement }]`）。実装時に対象市場を反映する。
- Features: `{ SupportsWebSocket, SupportsMargin, SupportsStopOrder, SupportsParentOrder, SupportsCandlestick, SupportsOrderBookDelta, SupportsRealtimeExecutions, SupportsWithdraw }` などの機能フラグ（板差分/約定WS対応の有無を含める）。bitFlyer は Candlestick/OrderBookDelta/RealtimeExecutions は false。
- RateLimits（任意）: `{ RequestsPerMinute, OrdersPerMinute }` のような概略値

## 3. Raw API の扱い（Stage4 抽象外）
- 親注文（sendparentorder/cancelparentorder/getparentorders/getparentorder）、入出金（withdraw/getdeposits/getwithdrawals）、履歴系（balancehistory/collateralhistory）や権限確認（getpermissions）などは Raw で扱う。
- WS の再接続・バックプレッシャー・品質制御も実装ステージで扱う。

## 4. スコープの考え方
- Stage4: 抽象インターフェースとドメイン型の確定のみ（REST/WS を含む）
- Stage5+: bitFlyer など取引所へのマッピング実装と運用ロジックを実装

備考: 本表に記載の API はすべて「抽象定義のみが Stage4 スコープ」であり、実装は Stage5 以降で行う。
