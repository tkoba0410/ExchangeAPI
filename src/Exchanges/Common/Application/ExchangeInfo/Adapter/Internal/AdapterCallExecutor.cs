using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;

internal static class AdapterCallExecutor
{
    public static async Task<Call<TReq, TOk>> ExecuteMapCallAsync<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        string component,
        Func<CancellationToken, Task<Call<TNormReq, TNormRes>>> executeNormalized,
        Func<TNormRes, TOk> mapper,
        CancellationToken cancellationToken = default,
        Func<DateTimeOffset, Exception, Call<TReq, TOk>?>? mapException = null)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var normalizedCall = await executeNormalized(cancellationToken).ConfigureAwait(false);
            return AdapterCallMapper.MapCall(request, normalizedCall, component, mapper);
        }
        catch (Exception ex)
        {
            if (mapException is not null)
            {
                var mapped = mapException(startedAt, ex);
                if (mapped is not null)
                {
                    return mapped;
                }
            }

            return AdapterCallMapper.FromException<TReq, TOk>(request, startedAt, component, ex);
        }
    }
}
