# Stage7 TODO (Bittrade 対応含む)

- Bittrade ExchangeInfo: 手数料/メンテ情報の取得と JSON 初期値作成。必要に応じて Factory から JSON 切替の導線を追記。
- Bittrade エラー分類拡張: 公式エラーコード表に基づき ExchangeErrorCategory へ詳細マッピング。
- Bittrade 履歴系: AccountExecutions/履歴/ポジションが必要か確認し、対応 or NotSupported を明記。
- Bittrade サンプル/Docs: `docs/Adapter/Bittrade` に手数料/メンテ/エラー分類の扱いを追記する。
- ポリシー/署名/Factory の差し替え手順を docs に反映（ExchangeInfo JSON 切替含む）。
