using System;
using System.Net.Http;
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Infrastructure.Protocol;
using ExchangeApi.Infrastructure.Time;
using ExchangeApi.Infrastructure.Transport;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer 向けの IExchangeClient を組み立てるファクトリ。
/// HttpClient → HttpTransport → BitflyerSigningTransport → RestClient →
/// BitflyerPublicApi / BitflyerPrivateApi → BitflyerExchangeClient
/// という依存グラフをここで構築する。
/// </summary>
public static class BitflyerClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// bitFlyer 用の IExchangeClient を作成する。
    /// </summary>
    /// <param name="apiKey">bitFlyer API キー。</param>
    /// <param name="apiSecret">bitFlyer API シークレット。</param>
    public static IExchangeClient Create(string apiKey, string apiSecret)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(apiSecret));
        }

        // HttpClient は base URI のみ設定して Transport に渡す。
        var httpClient = new HttpClient
        {
            BaseAddress = BitflyerApiBaseUri,
        };

        // 生の HTTP トランスポート
        IHttpTransport baseTransport = new HttpTransport(httpClient, disposeHttpClient: true);

        // 既存: clockはそのまま
        IExchangeClock clock = new SystemClock();

        // signerを作る
        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);

        // signing transportにsignerを渡す
        IHttpTransport signingTransport = new BitflyerSigningTransport(baseTransport, signer);

        // 署名付き REST クライアント（Public/Private 共通で利用）
        IRestClient restClient = new RestClient(BitflyerApiBaseUri, signingTransport);

        // Raw API（Public / Private）
        var publicApi = new BitflyerPublicApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);

        // Adapter（IExchangeClient 実装）
        return new BitflyerExchangeClient(publicApi, privateApi);
    }
}
