using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Wire.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Wire.Public.Api
{
    public partial interface IBitflyerPublicWireApi
    {
        Task<Call<WireCallSpec, WireResponse>> GetTickerAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPublicWireApi
    {
        public Task<Call<WireCallSpec, WireResponse>> GetTickerAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default) =>
            _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Wire.Public.Endpoints.GetTickerWireSpec.Build(productCode),
                cancellationToken);
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Wire.Public.Endpoints
{
    internal static class GetTickerWireSpec
    {
        internal const string Method = "GET";
        internal const string Path = WirePaths.GetTicker;
        internal const string QueryProductCode = WireQueryKeys.ProductCode;

        public static WireCallSpec Build(string? productCode) =>
            WireCallSpecBuilder.Get(
                EndpointIds.GetTicker,
                Path,
                WireCallSpecBuilder.BuildQuery((QueryProductCode, productCode)));
    }
}
