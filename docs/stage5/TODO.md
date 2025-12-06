# Stage5 やることリスト（REST Only / bitFlyer 縦スライス）

- REST専用化/Realtime除去: `IRealtime*` 系インターフェース・DTO・実装・テスト（例: BitflyerRealtimeClient, Ticks等）を削除し、互換レイヤは作らない。REST-onlyでビルドが通ることを完了条件に含める。
- フォルダ再編: ContractsのDTOを Market/Trading/Account/Margin/ExchangeInfo/Common で階層化。adapter/Bitflyer を Http（Public/Private/Signer/Models）/Adapters/Trading/Market/Account/Margin/ExchangeInfo/RawApi に分離し、責務を物理的に揃える。
- ExchangeInfoベースライン: product_code一覧・ティックサイズ・最小数量の取得とキャッシュ方針を確定（起動時取得＋TTL=10分を初期値とし、実測で最適化）。Symbol対応表を確定。
- Trading実装: `/sendchildorder` で MARKET/LIMIT/STOP + TIF/Side/数量・価格バリデーション、STOPの trigger_price 必須、未サポートTIFは送信禁止。キャンセル（単体/全件）と約定確認ポーリング（1s間隔/最大30回=30sタイムアウトを初期値とし、実測で調整）の終端条件を実装。
- Account/Margin実装: `/getbalance` `/getcollateral` `/getpositions` `/getchildorders` `/getexecutions` を抽象APIへ統合し、口座サマリ取得フローを確立（数量単位・桁数・LONG/SHORT判定を明示）。
- Market実装: Ticker/Board/Executionsを抽象モデルへ正規化（必要ならCandles追加）、日時はUTC、Symbolは `BTC_JPY` 形式に統一。
- Raw API整理: 抽象化しにくいエンドポイントを一覧化し Raw に載せる（例: `/gethealth`, `/getboardstate`, product_codeごとの制限/設定取得系、取引所固有ステータス）。最終リストを確定し、Raw経由でもエラーマッピングは抽象例外に寄せる。
- エラーハンドリング: bitFlyerエラーコード/メッセージと抽象例外の対応表（認証・残高不足・パラメータ不正・レートリミット・その他）を作成し、再試行可否の基準を定義。
- テスト: マッピング単体（MARKET/LIMIT/STOP、BUY/SELL、端数・桁の境界値）、エラー単体（認証失敗/残高不足/パラメータ不正/レートリミットのモック）、結合（BTC/JPYで残高→LIMIT新規→ポーリング約定確認→決済→履歴取得が成功）を用意しグリーン確認。ポーリング/TTL調整はテスト結果に基づき値を確定する。
- ドキュメント: Stage5スコープ/ゴール/やらないこと、QuickStart（RESTのみで指値→約定確認→決済→履歴）を更新し、Realtime廃止の移行メモ（互換層なし）を明記。移行ガイドに「削除対象インターフェース/クラス一覧」「代替手順（RESTのみの利用例）」「バージョン影響」を含める。

- 計画・見積り補助: 推奨順序に対して所要時間/依存関係を洗い出し、実行計画として明文化する（各タスクの初期見積りと依存をドキュメント化）。

推奨順序:
1. REST専用化（Realtime除去）→ビルド確認
2. フォルダ再編（Contracts DTO階層化・Bitflyer責務分離）
3. ExchangeInfoベースライン確立
4. Trading実装（MARKET→LIMIT→STOP→キャンセル→ポーリング）
5. Account/Margin実装
6. Market実装
7. Raw API整理
8. エラーハンドリング充実
9. テストとドキュメント仕上げ
