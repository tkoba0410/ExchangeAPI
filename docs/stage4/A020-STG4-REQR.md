# A020-STG4-REQR Stage4 要件定義（Private 横展開 + 注文強化）

## 1. 対象
- 取引所: bitFlyer
- API: Private GET（positions/executions/collateral）、Private POST（cancelchildorder/cancelallchildorders）、注文送信（LIMIT/STOP/time_in_force/minute_to_expire 対応）
- 抽象: IExchangeTradingClient / IExchangeClient の機能拡張

## 2. ユースケース（代表）
1) ポジション照会: 現在の建玉を取得し、サイズ/価格/建玉方向を確認できる  
2) 約定履歴取得: 直近の executions を取得し、fills を集計できる  
3) 証拠金照会: collateral を取得し、発注可能額を判断できる  
4) 注文キャンセル: child order を ID でキャンセル、または全キャンセル  
5) 新規注文（拡張）: LIMIT/STOP を time_in_force/minute_to_expire 付きで送信できる  
6) エラー診断: bitFlyer 固有コードで例外が分類され、リトライ可否が判断できる  

## 3. 機能要件
- 抽象モデル: Position/Execution/Collateral（仮称）を追加し、取引所固有 DTO からマッピングする
- 注文: OrderRequest に price/minute_to_expire/time_in_force/trigger_price（STOP 用）を拡張
- キャンセル: cancelchildorder/cancelallchildorders を抽象インターフェースに追加
- エラー: HTTP + bitFlyer コードを E2 で分類し、typed 例外を投げる
- 設定: time_in_force や product code、API キーなどを既存の Factory 経由で設定可能

## 4. 非機能要件
- 互換性: Stage3 までの MARKET 既存インターフェースは後方互換を維持
- テスト: DTO マッピング/注文生成/エラー分類/キャンセルの単体テストを追加
- 観測性: レートリミットやエラー分類のフックを提供（実装は簡易で可）

## 5. 除外（Stage4 ではやらない）
- WebSocket（Ticker/Board/Executions ストリーム）
- 複数取引所実装
- 高度な信頼性（サーキットブレーカ、指数バックオフの細かな調整）
- 完全なドキュメント生成パイプライン（DocFX 等）は骨子のみ
