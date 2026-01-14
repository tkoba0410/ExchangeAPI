using ExchangeApi.Contracts.Common.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.DomainCommon.Types;

public static class ExchangeCodeFormatter
{
    public static string ToCanonicalId(ExchangeCode exchange) =>
        exchange switch
        {
            ExchangeCode.Bitflyer => "bitflyer",
            ExchangeCode.Bittrade => "bittrade",
            ExchangeCode.Binance => "binance",
            ExchangeCode.Bitbank => "bitbank",
            ExchangeCode.Btcbox => "btcbox",
            ExchangeCode.Coincheck => "coincheck",
            ExchangeCode.Gmocoin => "gmocoin",
            ExchangeCode.Okcoin => "okcoin",
            ExchangeCode.Sandbox => "sandbox",
            _ => exchange.ToString().ToLowerInvariant()
        };
}
