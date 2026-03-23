using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Wire.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Wire.Private.Api
{
    public partial interface IBitflyerPrivateWireApi
    {
        Task<Call<WireCallSpec, WireResponse>> CancelChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateWireApi
    {
        public Task<Call<WireCallSpec, WireResponse>> CancelChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bodyJson);

            return _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Wire.Private.Endpoints.CancelChildOrderWireSpec.Build(bodyJson),
                cancellationToken);
        }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Wire.Private.Endpoints
{
    internal static class CancelChildOrderWireSpec
    {
        internal const string Method = "POST";
        internal const string Path = WirePaths.CancelChildOrder;

        public static WireCallSpec Build(string bodyJson) =>
            WireCallSpecBuilder.Post(
                EndpointIds.CancelChildOrder,
                Path,
                bodyJson);
    }
}
