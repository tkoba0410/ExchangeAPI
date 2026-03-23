using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api
{
    public partial interface IBitflyerPrivateProtocolApi
    {
        Task<Call<WireCallSpec, WireResponse>> SendChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateProtocolApi
    {
        public Task<Call<WireCallSpec, WireResponse>> SendChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bodyJson);

            return _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.SendChildOrderWireSpec.Build(bodyJson),
                cancellationToken);
        }
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints
{
    internal static class SendChildOrderWireSpec
    {
        internal const string Method = "POST";
        internal const string Path = ProtocolPaths.SendChildOrder;

        public static WireCallSpec Build(string bodyJson) =>
            ProtocolCallSpecBuilder.Post(
                EndpointIds.SendChildOrder,
                Path,
                bodyJson);
    }
}
