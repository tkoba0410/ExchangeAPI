using System;
using System.Collections.Generic;
using System.Net.Http;
using ExchangeApi.Transport.Observability;

namespace ExchangeApi.Tests.Common.Tests.Transport.Logging;

public class StructuredRestClientLoggerTests
{
    [Fact]
    public void LogRequest_MasksSensitiveQueryValues()
    {
        var lines = new List<string>();
        var logger = new StructuredRestClientLogger(lines.Add);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api-cloud.bittrade.co.jp/v1/order?symbol=btc_jpy&AccessKeyId=abc123&Signature=sig-value&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=2026-01-01T00:00:00&order_id=12345&foo=bar");

        logger.LogRequest(request);

        var line = Assert.Single(lines);
        Assert.Contains("symbol=btc_jpy", line);
        Assert.Contains("AccessKeyId=***", line);
        Assert.Contains("Signature=***", line);
        Assert.Contains("SignatureMethod=***", line);
        Assert.Contains("SignatureVersion=***", line);
        Assert.Contains("Timestamp=***", line);
        Assert.Contains("foo=***", line);
        Assert.Matches(".*order_id=oidp_v1_[A-Z2-7]{16}.*", line);
        Assert.DoesNotContain("abc123", line);
        Assert.DoesNotContain("sig-value", line);
        Assert.DoesNotContain("order_id=12345", line);
    }

    [Fact]
    public void LogError_MasksAccountAndMessageIdentifiers()
    {
        var lines = new List<string>();
        var logger = new StructuredRestClientLogger(lines.Add);
        var exception = new InvalidOperationException("failed");

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api-cloud.bittrade.co.jp/v1/order/openOrders?account-id=10001&message_id=abcdef&status=open");

        logger.LogError(exception, request);

        var line = Assert.Single(lines);
        Assert.Contains("account-id=***", line);
        Assert.Contains("message_id=***", line);
        Assert.Contains("status=open", line);
        Assert.DoesNotContain("10001", line);
        Assert.DoesNotContain("abcdef", line);
    }

    [Fact]
    public void LogRequest_UsesPathOnlyWhenQueryIsAbsent()
    {
        var lines = new List<string>();
        var logger = new StructuredRestClientLogger(lines.Add);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.bitflyer.com/v1/me/getbalance");

        logger.LogRequest(request);

        var line = Assert.Single(lines);
        Assert.Contains("uri=https://api.bitflyer.com/v1/me/getbalance", line);
    }
}
