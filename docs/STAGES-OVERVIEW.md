# Stages Overview

Stage1〜将来ステージのロードマップ。進行状況に応じて改訂する。

## ステージ一覧
- **Stage1**: Public GET（/v1/getticker）で Ticker 取得。REST/Transport 分離と統一 DTO/抽象クライアントを確立。
- **Stage2**: Private GET 初期縦スライス（/v1/me/getbalance）。認証付き GET、署名、E1（HTTPベース）エラーの基盤整備。
- **Stage3**: Private POST 初期縦スライス（/v1/me/sendchildorder, MARKET）。署名付き POST、ドメイン⇄DTO マッピング、トレード API テンプレート確立。
- **Stage4**: 抽象 API の確定（REST-only へ集約）。Market/Trading/Account/Margin/ExchangeInfo/Raw の区分でインターフェースと最小ドメイン型を固定。
- **Stage5**: Stage4 抽象 REST を bitFlyer で実装・検証（REST-only）。LIMIT/STOP/キャンセル/ポジション・証拠金取得、エラー/ポーリングを含むトレードフローを通す。WS/Realtime は廃止。
- **Stage6**: REST-only 信頼性・運用強化。Timeout/Retry/RateLimit/CircuitBreaker の安全デフォルト、E2/E3 エラー分類、観測性フック（ログ/メトリクス/トレース）、Factory オプション拡張、Fault Injection と劣化環境E2E（TestFactory+モックで代表フロー確認）を整備。
- **Stage7**: 複数取引所対応の実証と DX 仕上げ。追加取引所で縦スライスを実装し、抽象 API/エラー分類/ポリシーの汎用性を検証。API リファレンスやサンプル/クックブック拡充。
- **Stage8**: 拡張機能や高頻度ワークロード向け最適化（必要に応じて定義）。運用自動化、追加の信頼性パターン、配布/リリース体制の強化を含む。

## 進め方の目安
- 縦方向（最小縦スライス）を先に深め、横展開は後段でまとめる。
- 各ステージは完了条件（DoD）を満たした時点で次へ進む。DoD はドキュメント/コード/テスト/動作確認を含む。
- ドキュメントは Stage ごとに A0xx 形式を基本とし、利用者向け資料（QuickStart/Entry Guide/Overview）を段階的に補強。
- ブレイキング変更はステージ境界で整理し、README/Overview/リリースノートに反映。

## ステージ移行ルール
- **完了判定**: DoD が揃い、主要ユースケースが縦スライスで通ること。
- **品質ゲート**: 単体・結合テスト緑、手動チェックリスト完了、ビルド/リンター通過。
- **ブレイキング変更の明示**: 互換性影響はリリースノートに列挙し、ドキュメントを更新。
- **持ち越し管理**: 後送りタスクは「次ステージ持ち越しリスト」に記載し、開始時に再優先付け。
- **タグ/リリース**: ステージ完了でタグ付与し、最小限のリリースノートを残す。

※ 本書はドラフト。ステージ進行に応じて更新する。
