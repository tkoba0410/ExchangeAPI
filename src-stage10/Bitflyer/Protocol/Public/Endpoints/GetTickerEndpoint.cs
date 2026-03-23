using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api
{
    public partial interface IBitflyerPublicProtocolApi
    {
        Task<Call<WireCallSpec, WireResponse>> GetTickerAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPublicProtocolApi
    {
        public Task<Call<WireCallSpec, WireResponse>> GetTickerAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default) =>
            _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTickerWireSpec.Build(productCode),
                cancellationToken);
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints
{
    internal static class GetTickerWireSpec
    {
        internal const string Method = "GET";
        internal const string Path = ProtocolPaths.GetTicker;
        internal const string QueryProductCode = ProtocolQueryKeys.ProductCode;

        public static WireCallSpec Build(string? productCode) =>
            ProtocolCallSpecBuilder.Get(
                EndpointIds.GetTicker,
                Path,
                ProtocolCallSpecBuilder.BuildQuery((QueryProductCode, productCode)));
    }
}
