using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Bootstrap;

internal static class BitflyerBootstrap
{
    internal static BitflyerProtocolBundle CreateProtocolBundle(BitflyerClientOptions? options)
    {
        var normalizedOptions = options ?? new BitflyerClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getTicker = new GetTickerProtocolEndpoint(transport);
        var publicApi = new BitflyerPublicProtocolApi(getTicker);

        if (normalizedOptions.Credentials is null)
        {
            return new BitflyerProtocolBundle
            {
                Public = publicApi,
                Private = null,
            };
        }

        var getBalance = new GetBalanceProtocolEndpoint(transport);
        var sendChildOrder = new SendChildOrderProtocolEndpoint(transport);
        var cancelChildOrder = new CancelChildOrderProtocolEndpoint(transport);
        var privateApi = new BitflyerPrivateProtocolApi(getBalance, sendChildOrder, cancelChildOrder);

        return new BitflyerProtocolBundle
        {
            Public = publicApi,
            Private = privateApi,
        };
    }

    internal static BitflyerNativeBundle CreateNativeBundle(BitflyerClientOptions? options)
    {
        var normalizedOptions = options ?? new BitflyerClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getTickerProtocol = new GetTickerProtocolEndpoint(transport);
        var publicProtocolApi = new BitflyerPublicProtocolApi(getTickerProtocol);
        var getTickerNative = new GetTickerNativeEndpoint(getTickerProtocol);
        var publicApi = new BitflyerPublicNativeApi(getTickerNative);

        if (normalizedOptions.Credentials is null)
        {
            return new BitflyerNativeBundle
            {
                Public = publicApi,
                Private = null,
                Protocol = new BitflyerProtocolBundle
                {
                    Public = publicProtocolApi,
                    Private = null,
                },
            };
        }

        var getBalanceProtocol = new GetBalanceProtocolEndpoint(transport);
        var sendChildOrderProtocol = new SendChildOrderProtocolEndpoint(transport);
        var cancelChildOrderProtocol = new CancelChildOrderProtocolEndpoint(transport);

        var privateProtocolApi = new BitflyerPrivateProtocolApi(
            getBalanceProtocol,
            sendChildOrderProtocol,
            cancelChildOrderProtocol);

        var getBalance = new GetBalanceNativeEndpoint(getBalanceProtocol);
        var sendChildOrder = new SendChildOrderNativeEndpoint(sendChildOrderProtocol);
        var cancelChildOrder = new CancelChildOrderNativeEndpoint(cancelChildOrderProtocol);
        var privateApi = new BitflyerPrivateNativeApi(getBalance, sendChildOrder, cancelChildOrder);

        return new BitflyerNativeBundle
        {
            Public = publicApi,
            Private = privateApi,
            Protocol = new BitflyerProtocolBundle
            {
                Public = publicProtocolApi,
                Private = privateProtocolApi,
            },
        };
    }

    private static IProtocolTransport CreateTransport(BitflyerClientOptions normalizedOptions)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = normalizedOptions.BaseUri,
        };

        IProtocolDebugLogger debugLogger = normalizedOptions.EnableProtocolDebugLogging
            ? new FileProtocolDebugLogger(normalizedOptions.ProtocolDebugLogDirectory)
            : new NoOpProtocolDebugLogger();

        return new BitflyerProtocolTransport(
            httpClient,
            debugLogger,
            normalizedOptions.Credentials?.ApiKey,
            normalizedOptions.Credentials?.ApiSecret);
    }
}
