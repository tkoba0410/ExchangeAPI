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
    private readonly IRestClient _rest;

    /// <summary>
    /// bitFlyer API のベース URL。
    /// </summary>
    private static readonly Uri BaseUri = new("https://api.bitflyer.com/");

    public BitflyerPublicApi(IRestClient rest)
    {
        _rest = rest ?? throw new ArgumentNullException(nameof(rest));
    }

    /// <summary>
    /// 生の ticker JSON を取得し、<see cref="BitflyerTickerRaw"/> にデシリアライズして返す。
    /// </summary>
    public Task<BitflyerTickerRaw> GetTickerRawAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode must not be null or whitespace.", nameof(productCode));
        }

        // bitFlyer の ticker エンドポイント
        var relativePath = $"v1/ticker?product_code={productCode}";

        return _rest.GetAsync<BitflyerTickerRaw>(relativePath, cancellationToken);
    }
}
