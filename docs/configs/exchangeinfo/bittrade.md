# Bittrade ExchangeInfo 設定（人間向けメモ）

`configs/exchangeinfo/bittrade.json` と同じ内容を読みやすく記載しています。取引ルールは公式ページ（取引所 取引ルール）に基づく。

- Symbol: BTC/JPY (`productCode: btcjpy`)
- PriceIncrement: 0.01
- SizeIncrement: 0.00001
- MinSize: 0.00001 BTC
- MinNotional: 2 JPY（成行/指値とも最小約定金額）
- MaxSize: 10 BTC（成行・指値とも）
- Market 上限: 買い 10,000,000 JPY / 売り 5 BTC（JSON の `statusNote` に記載）
- MakerFeeRate / TakerFeeRate: 0（手数料無料と明記されているため 0 設定）。FeeCurrency/FeeType: null/Percentage（約定通貨で徴収想定）
- IsSupported: true（symbols API state=online）
- Maintenance: なし（不明の場合は null）

