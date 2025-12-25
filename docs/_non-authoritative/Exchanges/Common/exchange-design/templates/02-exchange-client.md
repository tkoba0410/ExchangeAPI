# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# 02 ExchangeClient Implementation Template (Common + Raw/Wire Gate + Capability)

ポイント：
- client 自体は Raw/Wire API を **implements しない**
- Raw/Wire は **ゲート経由で取り出す**（C1：未対応は例外）
- Capability は `client.As<TCapability>()` で短く呼べる

## 公開インターフェイス（Common 正面玄関）

```csharp
public interface IExchangeClient
{
    ExchangeCode ExchangeCode { get; }

    ITradingApi Trading { get; }
    IAccountApi Account { get; }
    IMarketDataApi MarketData { get; }
    IExchangeInfoApi ExchangeInfo { get; }
}
```

## internal ゲート IF

```csharp
internal interface IHasRawAccess
{
    bool TryGetRaw<T>(out T raw) where T : class;
}

internal interface IHasWireAccess
{
    bool TryGetWire<T>(out T wire) where T : class;
}
```

## 利用者向け最短導線（拡張メソッド、C1）

```csharp
public sealed class ExchangeFeatureNotSupportedException : NotSupportedException
{
    public ExchangeFeatureNotSupportedException(string message) : base(message) { }
}

public static class ExchangeClientExtensions
{
    public static T Raw<T>(this IExchangeClient client) where T : class
    {
        if (client is IHasRawAccess ra && ra.TryGetRaw<T>(out var raw))
            return raw;

        throw new ExchangeFeatureNotSupportedException(
            $"Raw API {typeof(T).Name} is not available for {client.ExchangeCode}.");
    }

    public static T Wire<T>(this IExchangeClient client) where T : class
    {
        if (client is IHasWireAccess wa && wa.TryGetWire<T>(out var wire))
            return wire;

        throw new ExchangeFeatureNotSupportedException(
            $"Wire API {typeof(T).Name} is not available for {client.ExchangeCode}.");
    }

    public static T As<T>(this IExchangeClient client) where T : class
    {
        if (client is T t) return t;

        throw new ExchangeFeatureNotSupportedException(
            $"{typeof(T).Name} is not supported by {client.ExchangeCode}.");
    }
}
```

## 取引所ごとの client 実体（internal）

```csharp
internal sealed class BitflyerExchangeClient :
    IExchangeClient,
    IHasRawAccess,
    IHasWireAccess
{
    public ExchangeCode ExchangeCode => ExchangeCode.Bitflyer;

    public ITradingApi Trading { get; }
    public IAccountApi Account { get; }
    public IMarketDataApi MarketData { get; }
    public IExchangeInfoApi ExchangeInfo { get; }

    private readonly object _rawBundle;
    private readonly object _wireBundle;

    public BitflyerExchangeClient(
        ITradingApi trading,
        IAccountApi account,
        IMarketDataApi marketData,
        IExchangeInfoApi exchangeInfo,
        object rawBundle,
        object wireBundle)
    {
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        _rawBundle = rawBundle ?? throw new ArgumentNullException(nameof(rawBundle));
        _wireBundle = wireBundle ?? throw new ArgumentNullException(nameof(wireBundle));
    }

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T;
        return raw is not null;
    }

    public bool TryGetWire<T>(out T wire) where T : class
    {
        wire = _wireBundle as T;
        return wire is not null;
    }
}
```

## 利用コード例（見た目の比較）

```csharp
// Common（普通の利用者）
var t = await client.MarketData.GetTickerAsync(symbol);

// Raw（玄人）
var raw = client.Raw<IBitflyerRawApi>();
var board = await raw.MarketData.GetBoardAsync("BTC_JPY");

// Wire（正規化）
var wire = client.Wire<IBitflyerWireApi>();
var wt = await wire.MarketData.GetTickerAsync("BTC_JPY");
```
