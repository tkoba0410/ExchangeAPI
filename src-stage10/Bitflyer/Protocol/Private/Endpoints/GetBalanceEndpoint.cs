using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api
{
    public partial interface IBitflyerPrivateProtocolApi
    {
        Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateProtocolApi
    {
        public Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default) =>
            _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalanceWireSpec.Build(),
                cancellationToken);
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints
{
    internal static class GetBalanceWireSpec
    {
        internal const string Method = "GET";
        internal const string Path = ProtocolPaths.GetBalance;

        public static WireCallSpec Build() =>
            ProtocolCallSpecBuilder.Get(
                EndpointIds.GetBalance,
                Path,
                query: null);
    }
}
