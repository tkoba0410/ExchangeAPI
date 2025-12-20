using System.Linq;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeRequestSigner_Tests
{
    [Fact]
    public async Task SignAsync_AppendsRequiredSignatureQuery()
    {
        var signer = new BittradeRequestSigner("access", "secret");
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api-cloud.bittrade.co.jp/v1/order/orders?symbol=btcjpy");

        await signer.SignAsync(request);

        var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);
        Assert.Equal("btcjpy", query["symbol"]);
        Assert.Equal("access", query["AccessKeyId"]);
        Assert.Equal("HmacSHA256", query["SignatureMethod"]);
        Assert.Equal("2", query["SignatureVersion"]);
        Assert.False(string.IsNullOrWhiteSpace(query["Timestamp"]));
        Assert.False(string.IsNullOrWhiteSpace(query["Signature"]));
    }
}
