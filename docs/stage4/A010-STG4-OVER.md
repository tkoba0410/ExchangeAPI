# A010-STG4-OVER Stage4 ゴール定義（抽象インターフェース整備・凍結）

## 1. Stage4 の目的
Stage3 で確立した縦スライス（MARKET 発注）を土台に、**取引ライフサイクルを最後まで見据えた抽象インターフェースとドメインモデルを揃える**ことを目的とする。実装の横展開は行わず、以降の縦深フェーズ（Stage5 以降）で具体実装できる状態をつくる。

## 2. スコープ（Stage4 でやること / やらないこと）

### 2.1 Stage4 でやること
- Private GET/POST を見据えた抽象インターフェースの追加・整理  
  - `Positions/Executions/Collateral/OpenOrders` 取得、`Cancel` 系、`ListOrders` 等のメソッド定義  
  - DTO/ドメインモデル（`Position`, `Execution`, `Collateral`, `OpenOrder`, `OrderRequest` 拡張）を Abstractions で揃える
- 注文モデルの拡張（LIMIT/STOP/STOP_LIMIT、`TimeInForce`, `MinuteToExpire`, `TriggerPrice` 等）を抽象層で定義し、型の互換性を確認する
- エラー分類の骨子（E2: 取引所固有コード分類の方針、再試行可否の表現など）を決め、例外型/enum を設計する
- RateLimit/Retry など将来フックのインターフェースだけを用意する（実装は Stage5 以降で可）
- ドキュメント整備：A010〜A070 を新方針に合わせて更新し、横展開は Stage5 以降に送ることを明記する

### 2.2 Stage4 でやらないこと
- bitFlyer での positions/executions/collateral/cancel の実装やテスト（縦深実装は Stage5 の範囲）
- 注文種別拡張の実装（Abstractions まで。送信ロジックは Stage5）
- 取引所固有コードの詳細マッピングやリトライ方針の実装
- WebSocket 系（Ticker/Board/Executions ストリーム）
- 複数取引所対応の実装
- 信頼性パターンの本格実装（サーキットブレーカ/高度なリトライ）
- DX 仕上げ（DocFX 生成/CLI などの最終形）

## 3. 完了条件（Definition of Done）
1. Abstractions/Interfaces が positions/executions/collateral/open-orders/cancel を含み、注文モデル拡張が型として揃っている
2. E2 エラー分類方針と例外/API 契約が定義され、ビルド時点で参照可能になっている
3. RateLimit/Retry など将来フックのインターフェースが用意され、呼び出し側が差し替え可能な構造になっている
4. 追加したメソッド/型のスタブ実装が存在し、Stage3 までの機能を壊さずにビルド/テストが通る
5. ドキュメント: Stage4 A010〜A070 が新方針に更新され、横展開が Stage5 以降に送られたことを明示している

## 4. Stage5 以降への接続
- Stage5: Stage4 で定義した抽象を bitFlyer で縦方向に実装し、取引ライフサイクルを通す
- Stage6: WebSocket/リアルタイム拡張に進む
- Stage7 以降: 信頼性パターン強化を経て、最後に複数取引所対応・DX 仕上げを行う
