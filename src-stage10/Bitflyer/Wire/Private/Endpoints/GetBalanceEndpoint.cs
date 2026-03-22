using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Wire.Internal.Runtime;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Wire.Private.Api
{
    public partial interface IBitflyerPrivateWireApi
    {
        Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default);
    }

    public sealed partial class BitflyerPrivateWireApi
    {
        public Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default) =>
            _transport.SendAsync(
                ExchangeApi.Stage10.Bitflyer.Wire.Private.Endpoints.GetBalanceWireSpec.Build(),
                cancellationToken);
    }
}

namespace ExchangeApi.Stage10.Bitflyer.Wire.Private.Endpoints
{
    internal static class GetBalanceWireSpec
    {
        internal const string Method = "GET";
        internal const string Path = WirePaths.GetBalance;

        public static WireCallSpec Build() =>
            WireCallSpecBuilder.Get(
                EndpointIds.GetBalance,
                Path,
                query: null);
    }
}
