using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Shared.Transport.Observability;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Shared.Transport.Http;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Transport;

public sealed class RestClientTests
{
    private sealed record TestDto(string Value);

    [Fact]
    public async Task BuildUri_MergesPathQueryAndArgumentQuery()
    {
        var rest = CreateRestClient(out var transport);

        var raw = await rest.GetRawAsync(
            "/api?a=1",
            new Dictionary<string, string?> { ["b"] = "2" });
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        AssertQueryEquals("/api?a=1&b=2", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_AllowsDuplicateKeyWithSameValue()
    {
        var rest = CreateRestClient(out var transport);

        var raw = await rest.GetRawAsync(
            "/api?a=1",
            new Dictionary<string, string?> { ["a"] = "1" });
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        AssertQueryEquals("/api?a=1", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_ThrowsOnDuplicateKeyWithDifferentValue()
    {
        var rest = CreateRestClient(out _);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            rest.GetRawAsync(
                "/api?a=1",
                new Dictionary<string, string?> { ["a"] = "2" }));
    }

    [Fact]
    public async Task BuildUri_IgnoresNullValues()
    {
        var rest = CreateRestClient(out var transport);

        var raw = await rest.GetRawAsync(
            "/api",
            new Dictionary<string, string?> { ["a"] = null, ["b"] = "x" });
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        AssertQueryEquals("/api?b=x", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_NoQueryIsHandled()
    {
        var rest = CreateRestClient(out var transport);

        var raw = await rest.GetRawAsync("/api");
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        AssertQueryEquals("/api", transport.LastRequest!.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task BuildUri_EncodesSpecialCharacters()
    {
        var rest = CreateRestClient(out var transport);

        var raw = await rest.GetRawAsync(
            "/api",
            new Dictionary<string, string?> { ["q"] = "a b&c" });
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        AssertQueryEquals("/api?q=a%20b%26c", transport.LastRequest!.RequestUri!.PathAndQuery);
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

        var raw = await rest.GetRawAsync("/api");
        Assert.Equal((int)HttpStatusCode.NoContent, raw.StatusCode);
        Assert.Equal(string.Empty, raw.Body);
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

        var raw = await rest.GetRawAsync("/api");
        Assert.Equal((int)HttpStatusCode.InternalServerError, raw.StatusCode);
        Assert.Equal("server error", raw.Body);
    }

    [Fact]
    public async Task GetAsync_HttpRequestException_IsWrappedWithStatusCode()
    {
        var transport = new FakeTransport
        {
            ResponseFactory = () => throw new HttpRequestException("boom", null, HttpStatusCode.BadGateway)
        };
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var ex = await Assert.ThrowsAsync<TransportException>(() => rest.SendRawAsync("GET", "/api"));

        Assert.Equal(HttpStatusCode.BadGateway, ex.StatusCode);
        Assert.IsType<HttpRequestException>(ex.InnerException);
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

        var raw = await rest.GetRawAsync("/api");
        Assert.Equal((int)HttpStatusCode.OK, raw.StatusCode);
        Assert.Equal(string.Empty, raw.Body);
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

        var raw = await rest.GetRawAsync("/api");
        Assert.Equal((int)HttpStatusCode.OK, raw.StatusCode);
        Assert.Equal("not json", raw.Body);
    }

    [Fact]
    public async Task PostAsync_SerializesBodyAndUsesPostMethod()
    {
        var transport = new FakeTransport();
        var rest = new RestClient(new Uri("https://example.com"), transport);

        var body = new TestDto("req");

        var raw = await rest.SendRawAsync("POST", "/api", bodyJson: JsonSerializer.Serialize(body));
        var result = JsonSerializer.Deserialize<TestDto>(raw.Body!);

        Assert.Equal("ok", result!.Value);
        Assert.Equal(HttpMethod.Post, transport.LastRequest!.Method);
        Assert.Contains("\"Value\":\"req\"", transport.LastRequestContent);
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

        var raw = await rest.SendRawAsync("POST", "/api", bodyJson: JsonSerializer.Serialize(new TestDto("x")));
        Assert.Equal((int)HttpStatusCode.BadRequest, raw.StatusCode);
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

        var raw = await rest.SendRawAsync("POST", "/api", bodyJson: JsonSerializer.Serialize(new TestDto("x")));
        Assert.Equal((int)HttpStatusCode.OK, raw.StatusCode);
        Assert.Equal("not json", raw.Body);
    }

    [Fact]
    public async Task Observer_Is_Notified_On_Request_Response_And_Error()
    {
        var observer = new RecordingObserver();
        var transport = new FakeTransport
        {
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"error_message\":\"bad\"}", Encoding.UTF8, "application/json")
            }
        };
        var rest = new RestClient(new Uri("https://example.com"), transport, observer: observer);

        var raw = await rest.GetRawAsync("/api");
        Assert.Equal((int)HttpStatusCode.BadRequest, raw.StatusCode);

        Assert.True(observer.RequestCalled);
        Assert.True(observer.ResponseCalled);
        Assert.False(observer.ErrorCalled);
        Assert.Equal(HttpStatusCode.BadRequest, observer.LastStatus);
        Assert.NotEqual(Guid.Empty, observer.LastRequestId);
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

    private sealed class RecordingObserver : IRestCallObserver
    {
        public bool RequestCalled { get; private set; }
        public bool ResponseCalled { get; private set; }
        public bool ErrorCalled { get; private set; }
        public Guid LastRequestId { get; private set; }
        public HttpStatusCode? LastStatus { get; private set; }

        public void OnRequest(RestCallContext context)
        {
            RequestCalled = true;
            LastRequestId = context.RequestId;
        }

        public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
        {
            ResponseCalled = true;
            LastStatus = response.StatusCode;
        }

        public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
        {
            ErrorCalled = true;
            LastStatus = statusCode;
        }
    }

}
