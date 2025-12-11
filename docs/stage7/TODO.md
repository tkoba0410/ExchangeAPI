# Stage7 TODO (Bittrade 対応含む)

- Bittrade ExchangeInfo 取得: API (`common/symbols` 等) から刻み/最小数量/手数料通貨/種別/メンテを取得し、ExchangeMarketInfo に反映。初期 JSON を用意。
- Bittrade エラー分類拡張: 公式エラーコード表に基づき ExchangeErrorCategory へ詳細マッピング。
- Bittrade 履歴系: AccountExecutions/履歴/ポジションが必要か確認し、対応 or NotSupported を明記。
- Bittrade サンプル/Docs: `docs/Adapter/Bittrade` に利用方法と未対応範囲を追加（済みの概要に追記する）。
- ポリシー/署名/Factory の差し替え手順を docs に反映（ExchangeInfo JSON 切替含む）。
