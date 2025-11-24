using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer.Models;
using ExchangeApi.Infrastructure.Protocol;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer 公開 REST API の実装。
/// Transport/Protocol 層に依存し、取引所固有のロジックのみを提供する。
/// </summary>
public sealed class BitflyerPublicApi : IBitflyerPublicApi
{
    private readonly IRestClient _restClient;

    /// <summary>
    /// bitFlyer API のベース URL。
    /// </summary>
    private static readonly Uri BaseUri = new("https://api.bitflyer.com/");

    public BitflyerPublicApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    /// <summary>
    /// 生の ticker JSON を取得し、<see cref="BitflyerTickerRaw"/> にデシリアライズして返す。
    /// </summary>
    public Task<BitflyerTickerRaw> GetTickerRawAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        const string path = "/v1/ticker";

        IReadOnlyDictionary<string, string?> query =
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["product_code"] = productCode,
            };

        return _restClient.GetAsync<BitflyerTickerRaw>(
            path,
            query,
            cancellationToken);
    }

}
