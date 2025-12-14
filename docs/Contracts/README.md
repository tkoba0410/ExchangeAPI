# Contracts Docs

Contracts プロジェクトの責務と主な DTO/インターフェースをまとめます。依存方向は `Contracts → Transport → Adapter.Bitflyer → Factory` で一方向。

## 役割
- 抽象インターフェース: `IMarketDataApi`, `ITradingApi`, `IAccountApi`, `IMarginAccountApi`, `IExchangeInfoApi`, `IApiCredentialProvider`
- 共通 DTO: Market/Trading/Account/ExchangeInfo/Errors。挙動は Transport/Adapter に委譲し、ここでは形だけを定義する。
- エラー分類: `ExchangeErrorCategory` で取引所固有エラーを正規化し、上位ポリシー判定に使う。

## 各ドキュメント
- `order-request.md`: 注文 DTO の必須組み合わせとバリデーションヒント。
- `exchange-info.md`: 取引所メタ情報とシンボル刻み値の扱い（`ExchangeMarketInfo` を正とし、SymbolMeta は廃止）。
- `exchange-info-json.md`: ExchangeInfo を JSON で外部化するためのスキーマ案とサンプル。
- `credentials.md`: API キーの権限種別/有効期限の表現と運用例。
