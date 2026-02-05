using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.NotSupported;

internal static class BittradeNotSupportedNormalizedCalls
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.NotSupported";

    public static Call<TReq, TOk> Create<TReq, TOk>(TReq request, string feature) =>
        NotSupportedCall.Create<TReq, TOk>(Layer, Component, request, feature);
}
