# Stage5 仕様メモ（REST Only / bitFlyer 縦スライス）

## 1. Trading 仕様
- エンドポイント: `/sendchildorder`（新規）, `/cancelchildorder`（単体キャンセル）, `/cancelallchildorders`（全キャンセル）。
- サポート注文種別: MARKET / LIMIT / STOP。
- TimeInForce: 使う場合は抽象 `TimeInForce` と bitFlyerパラメータの対応を固定（例: FOK→`FOK`, IOC→`IOC`）。bitFlyer未サポートは送信禁止または例外。
- Side: BUY/SELL を抽象 `OrderSide` と一意にマッピング。
- 数量・価格: 数量単位と最小数量を ExchangeInfo で取得し、送信前バリデーション。価格は JPY 建てで小数桁を tick size で正規化。
- STOP: `trigger_price` を必須、LIMIT/STOP の価格必須、MARKET は価格なし。
- 約定確認（ポーリング）: 初期値は 1s 間隔・最大30回=30s タイムアウト。キャンセル後の終端条件を定義。実測により以下で調整:
  - 充足条件: 80%以上のケースでタイムアウトしない間隔・回数
  - レートリミット: 429 が一定回数以上出る場合は間隔を伸ばす/回数を絞る
  - SLA化: 調整後の値をSPECとコードに反映し固定する

## 2. エラーハンドリング
- 対応表: bitFlyerエラーコード/メッセージと抽象例外をマッピング。
  - 認証エラー: 認証系例外。
  - 残高不足: 残高不足例外。
  - パラメータ不正: バリデーション例外。
  - レート制限: レートリミット例外。
  - その他: 汎用 ExchangeApiException。
- 再試行方針: 再試行可能/不可を分類（例: レート制限→一定リトライ可、残高不足→不可）。

## 3. DTO 正規化 / マッピング
- Market: Ticker/Board/Executions（必要ならCandles）の抽象DTOを固定し、bitFlyerレスポンスからの正規化ルールを明文化。
- Trading: OrderRequest/OrderResult/OpenOrder などのフィールド対応を定義（order_id/child_order_acceptance_id などのIDの扱い含む）。
- Account/Margin: Balance/Collateral/Position/Order履歴/Execution履歴のフィールド対応を定義。数値の単位・桁数・ポジション方向（LONG/SHORT）の決め方を明示。
- 共通: 日時はUTCに正規化、Symbol表記（例: `BTC_JPY`）を固定。

## 4. ExchangeInfo
- product_code一覧の取得元（bitFlyer API）とキャッシュ方針（起動時取得＋一定期間キャッシュなど）。
- ティックサイズ/最小注文数量の算出方法（API提供がない場合のハードコード可否や設定ファイル化）。
- シンボルと抽象DTO（Symbol）との対応を表で管理。
- キャッシュ方針: 起動時取得＋TTL=10分を初期値とし、実測で最適化。更新基準は「TTL内の値が実際と乖離したら短縮」「レートリミットが増えたら延長」。確定した値をSPECとコードに反映。

## 5. テスト方針
- マッピング単体テスト: DTO→Domain、Domain→bitFlyerリクエストの代表ケースを列挙（MARKET/LIMIT/STOP、正/負方向、桁数など）。
- エラー単体テスト: 代表的なエラーレスポンスをモックし、例外種別のマッピングを検証。
- 結合テスト: 代表フローを固定（例: 残高取得→LIMIT新規→約定確認→決済→履歴取得）。使う注文種類・シンボルと成功条件（例: status=completed, ポジション解消済み）を明記。

## 6. Realtime 除去の影響範囲（REST専用化）
- インターフェース: `IRealtime*` 系を削除対象とし、互換レイヤは設けない。
- DTO/モデル: Realtime/WS専用DTO（Ticks等）は Contracts から除去。
- 実装: `BitflyerRealtimeClient` 等の Realtime 実装・テストを除去。
- ビルド影響: REST専用後にビルドが通ることを完了条件に含める。

## 7. Raw API 取り扱い
- 抽象化しづらいもの（例: `/gethealth`, `/getboardstate`, 取引所固有の設定取得）は Raw API に載せる。
- Raw に載せる基準: 抽象 DTO との整合が取れない、または単一取引所にしか存在しない機能。
- Raw 経由でもエラーハンドリングは抽象例外に寄せる。
- Raw エンドポイントリスト: `/gethealth`, `/getboardstate`, product_code ごとの制限/設定取得系、取引所固有ステータス。最終リストを SPEC で確定し、実装とドキュメントで同期。

## 8. 優先度/実行順（計画のためのガイド）
1. REST専用化: Realtime除去とビルド修正。
2. フォルダ再編: Contracts DTO 階層化と Bitflyer 配下の責務分離。
3. ExchangeInfo ベースライン: product_code/ティックサイズ/最小数量を取得できる状態にする（以後のバリデーションに使用）。
4. Trading 実装（MARKET→LIMIT→STOP→キャンセル→ポーリング）。
5. Account/Margin 実装（残高・証拠金・建玉・履歴）。
6. Market 実装（Ticker/Board/Executions 正規化）。
7. Raw API 整理（抽象外のものを確定）。
8. エラーハンドリングマッピングの充実。
9. テスト（単体→結合）とドキュメント更新。

## 9. Trading 詳細設定（デフォルト推奨値）
- ポーリング間隔: 1〜2秒。最大試行: 30回（~30〜60秒でタイムアウト）。
- キャンセル後の終端条件: キャンセル要求成功 → その注文の状態が `canceled` / 未約定を確認して完了。
- TimeInForce: 未サポート値はリクエスト前にバリデーションでブロック。
- 調整基準: ポーリング/TTL など運用値はテスト結果とレートリミット状況で見直し、確定値をSPEC/コードに反映して固定。

## 10. テストシナリオ例（明文化）
- マッピング単体: MARKET/LIMIT/STOP で Side=BUY/SELL、数量端数や桁数が丸め/エラーになる境界値。
- エラー: 認証失敗、残高不足、パラメータ不正、レートリミットの各レスポンスをモックして例外種別を確認。
- 結合: BTC/JPY で (1) 残高取得 → LIMIT 新規（少量） → ポーリングで約定確認 → 決済 → 履歴取得 が成功。

## 11. 計画・見積り記載要件
- 各タスクに初期見積り（人時/人日）と依存関係を付与し、計画ドキュメントに記載。
- 優先順に沿ってクリティカルパスを明示し、順序変更の可否を判断可能にする。

## 12. Realtime 廃止の移行ガイド項目
- 削除対象インターフェース/クラス/DTO の一覧（`IRealtime*`, `BitflyerRealtimeClient`, `RealtimeTicks` 等）。
- 代替手順: RESTのみの利用例と置き換え方法（サンプルコード）。
- バージョン影響: 非互換バージョンとして明記し、適用方法を案内。
