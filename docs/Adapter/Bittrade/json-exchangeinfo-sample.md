# Bittrade ExchangeInfo JSON 切替サンプル

Bittrade は手数料/メンテを API から取得できないため、JSON を用いて ExchangeInfo を上書きする例です。`configs/exchangeinfo/bittrade.json` に手数料 0/null を設定し、Factory で `JsonExchangeInfoApi` を使う想定。

```csharp
using ExchangeApi.Factory.ExchangeInfo;
using ExchangeApi.Adapter.Bittrade.Factory;

// JSON ファイルを優先し、取れない場合は BittradeExchangeInfoApi の API 値をフォールバックする例
var jsonPaths = new[] { "configs/exchangeinfo/bittrade.json" };
var jsonApi = new JsonExchangeInfoApi(jsonPaths);
var apiFallback = BittradeClientFactory.CreateExchangeInfo();

ExchangeInfo info;
try
{
    info = await jsonApi.GetExchangeInfoAsync();
}
catch
{
    info = await apiFallback.GetExchangeInfoAsync();
}
```

注意:
- fee/maintenance は API 非提供のため JSON で手動設定が必要です。
- JSON の `source` に公式ルールページ URL を記載しています。値の更新は手動で行ってください。
