using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Transport;
using Xunit;

namespace ExchangeApi.Transport.Tests;

public sealed class RestClientTests
{
    private sealed record TestDto(string Value);

    [Fact]
    public async Task BuildUri_MergesPathQueryAndArgumentQuery()
    {
        var rest = CreateRestClient(out var transport);

        var result = await rest.GetAsync<TestDto>(
            "/api?a=1",
            new Dictionary<string, string?> { ["b"] = "2" });

        Assert.Equal("ok", result.Value);
        AssertQueryEquals("/api?a=1&b=2", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_AllowsDuplicateKeyWithSameValue()
    {
        var rest = CreateRestClient(out var transport);

        var result = await rest.GetAsync<TestDto>(
            "/api?a=1",
            new Dictionary<string, string?> { ["a"] = "1" });

        Assert.Equal("ok", result.Value);
        AssertQueryEquals("/api?a=1", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_ThrowsOnDuplicateKeyWithDifferentValue()
    {
        var rest = CreateRestClient(out _);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            rest.GetAsync<TestDto>(
                "/api?a=1",
                new Dictionary<string, string?> { ["a"] = "2" }));
    }

    [Fact]
    public async Task BuildUri_IgnoresNullValues()
    {
        var rest = CreateRestClient(out var transport);

        var result = await rest.GetAsync<TestDto>(
            "/api",
            new Dictionary<string, string?> { ["a"] = null, ["b"] = "x" });

        Assert.Equal("ok", result.Value);
        AssertQueryEquals("/api?b=x", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_NoQueryIsHandled()
    {
        var rest = CreateRestClient(out var transport);

        var result = await rest.GetAsync<TestDto>("/api");

        Assert.Equal("ok", result.Value);
        AssertQueryEquals("/api", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetAsync_NoContentWithNullContent_DoesNotThrowNullReference()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = null
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        await Assert.ThrowsAsync<ExchangeApiException>(() => rest.GetAsync<TestDto>("/api"));
    }

    [Fact]
    public async Task GetAsync_HttpErrorStatus_IsWrappedWithStatusCode()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("server error", Encoding.UTF8, "text/plain")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => rest.GetAsync<TestDto>("/api"));

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
        Assert.Contains("/api", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_HttpRequestException_IsWrappedWithStatusCode()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => throw new HttpRequestException("boom", null, HttpStatusCode.BadGateway)
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => rest.GetAsync<TestDto>("/api"));

        Assert.Equal(HttpStatusCode.BadGateway, ex.StatusCode);
        Assert.IsType<HttpRequestException>(ex.InnerException);
        Assert.Contains("/api", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_EmptyBody_ThrowsExchangeApiException()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        await Assert.ThrowsAsync<ExchangeApiException>(() => rest.GetAsync<TestDto>("/api"));
    }

    [Fact]
    public async Task GetAsync_InvalidJson_ThrowsExchangeApiExceptionWithInnerJsonException()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json", Encoding.UTF8, "application/json")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() => rest.GetAsync<TestDto>("/api"));

        Assert.IsType<JsonException>(ex.InnerException);
    }

    [Fact]
    public async Task PostAsync_SerializesBodyAndUsesPostMethod()
    {
        var transport = new FakeTransport();
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var body = new TestDto("req");

        var result = await rest.PostAsync<TestDto, TestDto>("/api", body);

        Assert.Equal("ok", result.Value);
        Assert.Equal(HttpMethod.Post, transport.LastRequest!.Method);
        Assert.Contains("\"value\":\"req\"", transport.LastRequestContent);
    }

    [Fact]
    public async Task PostAsync_HttpErrorStatus_IsWrappedWithStatusCode()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad", Encoding.UTF8, "application/json")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            rest.PostAsync<TestDto, TestDto>("/api", new TestDto("x")));

        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
    }

    [Fact]
    public async Task PostAsync_InvalidJson_ThrowsExchangeApiException()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json", Encoding.UTF8, "application/json")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            rest.PostAsync<TestDto, TestDto>("/api", new TestDto("x")));

        Assert.IsType<JsonException>(ex.InnerException);
    }




    private static RestClient CreateRestClient(out FakeTransport transport)
    {
        transport = new FakeTransport();
        return new RestClient(new Uri("https://example.com"), transport);
    }

    private static void AssertQueryEquals(string expectedPathAndQuery, string actualPathAndQuery)
    {
        var expected = ParseQuery(expectedPathAndQuery);
        var actual = ParseQuery(actualPathAndQuery);
        Assert.Equal(expected, actual);
    }

    private static Dictionary<string, string> ParseQuery(string pathAndQuery)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var parts = pathAndQuery.Split('?', 2);
        if (parts.Length == 1) return result;

        foreach (var pair in parts[1].Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(kv[0]);
            var value = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
            result[key] = value;
        }

        return result;
    }

    private sealed class FakeTransport : IHttpTransport
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string LastRequestContent { get; private set; } = string.Empty;
        public Func<HttpResponseMessage>? ResponseFactory { get; set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            LastRequestContent = request.Content is null
                ? string.Empty
                : request.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            var response = ResponseFactory?.Invoke()
                ?? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"Value\":\"ok\"}", Encoding.UTF8, "application/json"),
                };
            return Task.FromResult(response);
        }
    }

}
