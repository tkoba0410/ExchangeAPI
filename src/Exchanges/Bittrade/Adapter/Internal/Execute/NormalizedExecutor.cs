using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.Adapter.Internal;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Execute;

internal static class NormalizedExecutor
{
    public static Task<Call<TReq, TOk>> ExecuteMapCallAsync<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        string component,
        Func<CancellationToken, Task<Call<TNormReq, TNormRes>>> executeNormalized,
        Func<TNormRes, TOk> mapper,
        CancellationToken cancellationToken = default,
        Func<DateTimeOffset, Exception, Call<TReq, TOk>?>? mapException = null) =>
        AdapterCallExecutor.ExecuteMapCallAsync(
            request,
            component,
            executeNormalized,
            mapper,
            cancellationToken,
            mapException);
}
