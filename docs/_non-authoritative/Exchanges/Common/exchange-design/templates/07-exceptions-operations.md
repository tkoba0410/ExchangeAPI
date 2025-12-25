# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# 07 Exceptions / Operation / Error Mapping Template

目的：
- 例外には **必ず** `ExchangeCode` と `Operation` を入れる（Adapter責務）
- Operation は一意な文字列（定数）で集中管理
- 取引所のエラー（code/message/HTTP/status wrapper）を分類する

## Operation 定義（取引所ごと）

```csharp
internal static class Operations
{
    internal static class MarketData
    {
        public const string GetTicker = "Bitflyer.MarketData.GetTicker";
        public const string GetBoard  = "Bitflyer.MarketData.GetBoard";
    }

    internal static class Trading
    {
        public const string PlaceOrder = "Bitflyer.Trading.PlaceOrder";
    }
}
```

## Enrich（Adapterで必須）

```csharp
catch (ExchangeApiException ex)
{
    throw ex.Enrich(
        exchange: ExchangeCode.Bitflyer,
        operation: Operations.Trading.PlaceOrder
    );
}
```

## ErrorMapper（取引所→共通カテゴリ）

例：bittrade の `error_code` / HTTP / wrapper status を集約する。

```csharp
internal static class BittradeErrorMapper
{
    public static ExchangeErrorCategory Map(string? code, int? httpStatus)
    {
        if (httpStatus == 401 || httpStatus == 403) return ExchangeErrorCategory.AuthError;
        if (httpStatus == 429) return ExchangeErrorCategory.RateLimited;

        return ExchangeErrorCategory.ExchangeError;
    }
}
```

## 例外責務の分離（重要）

- Raw：鏡像（判定しない）
- Wire：正規化（判定・抽出）
- Adapter：共通化（Enrich して上げる）
