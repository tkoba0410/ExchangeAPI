# A050-STG4-IMPL Stage4 実装ノート（抽象設計フェーズ）

Stage4 は実装を進めるステージではなく、抽象インターフェースとドメインの整合性を固めるフェーズ。ここでは「何を残し、何を Stage5 に送るか」を明確化する。

## 1. 抽象/ドメイン
- `IMarketDataApi` / `ITradingApi` / `IAccountApi` / `IMarginAccountApi` / `IRealtimeMarketDataApi` / `IExchangeInfoApi` のメソッドセットを凍結する。
- ドメイン型は最小限に留める：Ticker / OrderBook / Execution / OrderRequest / OrderResult / OpenOrder / Position / Collateral。
- Margin は建玉・証拠金サマリに限定し、詳細な口座状態や履歴は Raw へ逃がす。
- OrderRequest は Stage3 の骨格を踏襲し、抽象メソッドのパラメータ要求と矛盾がないかを確認する（バリデーション詳細は Stage5 以降）。

## 2. Infrastructure / Adapter（設計メモ）
- REST と WS を分離し、Transport/Adapter を差し替え可能に保つ設計方針を記述する。
- E2 やレート制御などのフック位置を示し、実装は Stage5 以降に委ねる。
- Raw API（親注文/入出金など）をどこで受けるかの方針だけ残し、抽象への持ち込みを避ける。

## 3. Stage5 への受け渡し
- 抽象を実装する際に必要な最小限の契約（null 許容、例外方針、型の既定値）を記述する。
- WS 実装に求めるイベント形/購読解除の期待値を示し、詳細な QoS は後続で設計する。
- Factory 拡張の入口（登録ポイント）だけ明示し、実装コードは持ち込まない。

## 4. ドキュメント/DX
- Stage4 文書とサンプル記述を「抽象確定」基調に揃える（実装手順は書かない）。
- Quick Start/README の実装寄り説明は Stage5 以降で追加する前提にし、必要ならプレースホルダを置く。
