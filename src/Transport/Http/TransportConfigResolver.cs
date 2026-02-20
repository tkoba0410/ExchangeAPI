using System;
using System.Net.Http;

namespace ExchangeApi.Transport.Http;

/// <summary>
/// <see cref="TransportConfig"/> から実行時の <see cref="IHttpTransport"/> を構築する。
/// </summary>
public static class TransportConfigResolver
{
    public static ResolvedTransport Resolve(Uri baseUri, TransportConfig config)
    {
        if (baseUri is null) throw new ArgumentNullException(nameof(baseUri));
        if (config is null) throw new ArgumentNullException(nameof(config));

        return config switch
        {
            TransportConfig.ExternalTransport externalTransport =>
                new ResolvedTransport(
                    externalTransport.Transport ?? throw new ArgumentException("Transport must not be null.", nameof(config)),
                    DisposeTransport: false),
            TransportConfig.ExternalHttpClient externalHttpClient =>
                externalHttpClient.HttpClient is null
                    ? throw new ArgumentException("HttpClient must not be null.", nameof(config))
                    : new ResolvedTransport(
                        new HttpTransport(externalHttpClient.HttpClient, disposeHttpClient: false),
                        DisposeTransport: true),
            TransportConfig.ManagedHttp managed => CreateManaged(baseUri, managed.Timeout),
            _ => throw new ArgumentOutOfRangeException(nameof(config), "Unsupported transport configuration.")
        };
    }

    private static ResolvedTransport CreateManaged(Uri baseUri, TimeSpan? timeout)
    {
        if (timeout is { } configuredTimeout && configuredTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
        }

        var httpClient = new HttpClient
        {
            BaseAddress = baseUri,
        };

        if (timeout is { } value)
        {
            httpClient.Timeout = value;
        }

        return new ResolvedTransport(
            new HttpTransport(httpClient, disposeHttpClient: true),
            DisposeTransport: true);
    }
}

public readonly record struct ResolvedTransport(
    IHttpTransport Transport,
    bool DisposeTransport);
