using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExchangeApi.Exchanges.Bitflyer.Wire.Public;

namespace ExchangeApi.Exchanges.Bitflyer.Wire
{
    /// <summary>
    /// bitFlyer Public REST API への Wire アクセスインターフェース。
    /// </summary>
    internal interface IBitflyerPublicApi : IBitflyerWireMarketDataApi, IBitflyerWireExchangeInfoApi
    {
    }
}
