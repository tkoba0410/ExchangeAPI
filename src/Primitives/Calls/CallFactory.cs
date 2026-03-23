namespace ExchangeApi.Primitives.Calls;

public static class CallFactory
{
    public static Call<TRequest, TResponse> Success<TRequest, TResponse>(
        TRequest request,
        TResponse response,
        CallMeta meta)
    {
        return new Call<TRequest, TResponse>
        {
            Request = request,
            Response = response,
            IsSuccess = true,
            Error = null,
            Meta = meta,
        };
    }

    public static Call<TRequest, TResponse> Failure<TRequest, TResponse>(
        TRequest request,
        CallError error,
        CallMeta meta)
    {
        return new Call<TRequest, TResponse>
        {
            Request = request,
            Response = default,
            IsSuccess = false,
            Error = error,
            Meta = meta,
        };
    }
}
