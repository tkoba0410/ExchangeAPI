using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.NotSupported;

internal static class BitflyerNotSupportedNormalizedCalls
{
    private const string Layer = "Normalized";
    private const string Component = "Bitflyer.NotSupported";

    public static Call<TReq, TOk> Create<TReq, TOk>(TReq request, string feature) =>
        NotSupportedCall.Create<TReq, TOk>(Layer, Component, request, feature);
}
