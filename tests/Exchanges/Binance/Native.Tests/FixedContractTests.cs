using System.Reflection;
using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Tests.Exchanges.Binance.Native.Tests;

public sealed class FixedContractTests
{
    [Fact]
    public void Fixed_Request_And_Response_Dtos_KeepKnown_Shape()
    {
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.Symbol), typeof(string));
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.Interval), typeof(string));
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.StartTime), typeof(long?));
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.EndTime), typeof(long?));
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.TimeZone), typeof(string));
        AssertProperty(typeof(GetKlinesRequest), nameof(GetKlinesRequest.Limit), typeof(int?));

        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.OpenTime), typeof(long));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.OpenPrice), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.HighPrice), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.LowPrice), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.ClosePrice), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.Volume), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.CloseTime), typeof(long));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.QuoteAssetVolume), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.NumberOfTrades), typeof(int));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.TakerBuyBaseAssetVolume), typeof(decimal));
        AssertProperty(typeof(GetKlines.Item), nameof(GetKlines.Item.TakerBuyQuoteAssetVolume), typeof(decimal));

        AssertCallMethod(
            typeof(IBinancePublicNativeApi),
            nameof(IBinancePublicNativeApi.GetKlinesCallAsync),
            typeof(GetKlinesRequest),
            typeof(IReadOnlyList<GetKlines.Item>));
    }

    private static void AssertProperty(Type type, string propertyName, Type propertyType)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        Assert.Equal(propertyType, property!.PropertyType);
    }

    private static void AssertCallMethod(Type type, string methodName, Type requestType, Type responseType)
    {
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(method);

        var parameters = method!.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(requestType, parameters[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);

        var expectedCallType = typeof(Call<,>).MakeGenericType(requestType, responseType);
        var expectedReturnType = typeof(Task<>).MakeGenericType(expectedCallType);
        Assert.Equal(expectedReturnType, method.ReturnType);
    }
}
