namespace Unified.Client;

using Common.Core.Contracts.Contracts;
using Exchange.Bitflyer.Facade;
using Exchange.Bittrade.Facade;

/// <summary>
/// 複数取引所クライアントを束ねる薄いファサード。
/// Primary を切り替え可能にしつつ、個別のクライアントにもアクセスできる。
/// </summary>
public interface IUnifiedClient
{
    /// <summary>Primary（デフォルト）で使う取引所クライアント。</summary>
    IMarketDataApi PrimaryMarket { get; }
    ITradingApi PrimaryTrading { get; }
    IAccountApi PrimaryAccount { get; }
    IMarginAccountApi PrimaryMargin { get; }

    /// <summary>bitFlyer クライアント。</summary>
    BitflyerExchangeClient Bitflyer { get; }

    /// <summary>Bittrade クライアント。</summary>
    BittradeExchangeClient Bittrade { get; }
}
