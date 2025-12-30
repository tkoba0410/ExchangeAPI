using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public
{
    /// <summary>
    /// bitFlyer Public REST API への Wire アクセスインターフェース。
    /// </summary>
    internal interface IBitflyerPublicApi : IBitflyerWireMarketDataApi, IBitflyerWireExchangeInfoApi
    {
    }
}
