# Stage7 TODO (Bittrade 対応含む)

- Bittrade ExchangeInfo: 手数料/メンテ情報の取得と JSON 初期値作成。必要に応じて Factory から JSON 切替の導線を追記。
- Bittrade エラー分類拡張: 公式エラーコード表に基づき ExchangeErrorCategory へ詳細マッピング（ステータス以外のコードを網羅する）。
- Bittrade 履歴系: AccountExecutions/履歴/ポジションは REST 非対応のため NotSupported と明記済み。仕様変更時に再検討。
- Bittrade サンプル/Docs: `docs/Adapter/Bittrade` に手数料/メンテ/エラー分類/履歴の扱いを追記する（サンプル追加済み）。
- ポリシー/署名/Factory の差し替え手順を docs に反映（ExchangeInfo JSON 切替含む）。
