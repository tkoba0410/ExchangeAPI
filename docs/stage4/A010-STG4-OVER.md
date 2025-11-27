# A010-STG4-OVER Stage4 ゴール定義（Private GET/POST 横展開 + 注文強化）

## 1. Stage4 の目的
Stage3 で確立した Private POST 縦スライス（MARKET 注文）を基盤に、**取引ライフサイクルを一通り回せる状態**に拡張する。具体的には：
- Private GET の横展開でポジション/約定/証拠金を取得できること
- Private POST のキャンセル系を実装し、注文ライフサイクルを完結できること
- 注文種別を MARKET から LIMIT/STOP/time_in_force/minute_to_expire に拡張すること
- bitFlyer 固有コードを扱う E2 エラー分類を導入すること
- 必要に応じてレートリミットのフックを設計すること

## 2. スコープ（Stage4 でやること / やらないこと）

### 2.1 Stage4 でやること
- Private GET 拡張：positions / executions / collateral
- Private POST 拡張：cancelchildorder / cancelallchildorders
- 注文モデル拡張：LIMIT / STOP / time_in_force / minute_to_expire
- エラー処理拡張：E2（bitFlyer 固有コード分類、リトライ可否の足場）
- レートリミット拡張ポイントの設計（実装は最小限で可）
- ドキュメント整備：A010〜A070 Stage4 版 + Quick Start/README 再編の着手

### 2.2 Stage4 でやらないこと
- WebSocket 系（Ticker/Board/Executions ストリーム）
- 複数取引所対応の実装（設計の前提確認のみ）
- 信頼性パターンの本格実装（サーキットブレーカ/高度なリトライ）
- DX 仕上げ（DocFX 生成/CLI などの最終形）※骨子のみ

## 3. 完了条件（Definition of Done）
1. Private GET: positions/executions/collateral が抽象モデルで取得可能
2. Private POST: cancelchildorder/cancelallchildorders が抽象経由で呼べる
3. 注文: LIMIT/STOP + time_in_force/minute_to_expire を送信できる
4. エラー: bitFlyer 固有コードを E2 で分類し、typed 例外にマッピング
5. テスト: GET/POST 拡張・マッピング・E2 エラー分類の代表テストが緑
6. ドキュメント: Stage4 A010〜A070 と Quick Start/README 再編のドラフトが存在

## 4. Stage5 以降への接続
- Stage5: 複数取引所対応で抽象化の汎用性を実証する
- Stage6: WebSocket/リアルタイム拡張に進む
- Stage7 以降: 信頼性パターン/運用/UX 強化を仕上げる
