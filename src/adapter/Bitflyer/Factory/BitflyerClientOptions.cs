using System.Net.Http;
using ExchangeApi.Transport.Logging;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer.Factory;

/// <summary>
/// bitFlyer クライアントの構成オプション。
/// </summary>
public sealed class BitflyerClientOptions
{
    public HttpClient? HttpClient { get; init; }
    public IHttpPolicy? Policy { get; init; }
    public HttpPolicyOptions? PolicyOptions { get; init; }
    public IRestClientLogger? Logger { get; init; }
    public IRestCallObserver? Observer { get; init; }
    public IExchangeErrorClassifier? ErrorClassifier { get; init; }
}

public static class BitflyerClientOptionsExtensions
{
    /// <summary>
    /// 観測性に関するオプションをまとめて設定するヘルパ。
    /// </summary>
    public static BitflyerClientOptions WithObservability(
        this BitflyerClientOptions options,
        IRestCallObserver observer,
        IRestClientLogger? logger = null)
    {
        return new BitflyerClientOptions
        {
            HttpClient = options.HttpClient,
            Policy = options.Policy,
            PolicyOptions = options.PolicyOptions,
            Logger = logger ?? options.Logger,
            Observer = observer,
            ErrorClassifier = options.ErrorClassifier
        };
    }
}
