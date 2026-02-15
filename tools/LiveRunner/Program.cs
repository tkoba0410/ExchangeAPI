using System.Net;
using System.Net.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Observability;

var productCodeText = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]) ? args[0] : "BTC_JPY";
var timeoutMs = 10000;
if (args.Length > 1 && int.TryParse(args[1], out var parsedTimeout) && parsedTimeout > 0)
{
    timeoutMs = parsedTimeout;
}

var productCode = ProductCode.ParseOrThrowNormalized(productCodeText);
var observer = new ConsoleRestCallObserver(Console.WriteLine);
var options = new ClientOptions
{
    Observer = observer,
};

var normalized = ClientFactory.CreateNormalized(options);
using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs));
var call = await normalized.GetTickerCallAsync(productCode, cts.Token);

if (call.Result is CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Ok ok)
{
    var ticker = ok.Response;
    Console.WriteLine($"OUTCOME=Pass endpoint=GetTicker layer=Normalized productCode={ticker.ProductCode.Value} last={ticker.LastTradedPrice} ts={ticker.Timestamp:O}");
    return 0;
}

var err = (CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Err)call.Result;
Console.WriteLine($"OUTCOME=Fail endpoint=GetTicker layer=Normalized kind={err.Error.Kind} httpStatus={err.Error.HttpStatus} message={err.Error.Message}");
return 1;

internal sealed class ConsoleRestCallObserver(Action<string> write) : IRestCallObserver
{
    private readonly Action<string> _write = write ?? throw new ArgumentNullException(nameof(write));

    public void OnRequest(RestCallContext context)
    {
        _write($"REQ method={context.Method} uri={context.Request.RequestUri}");
    }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
    {
        _write($"RES method={context.Method} uri={context.Request.RequestUri} status={(int)response.StatusCode} elapsedMs={duration.TotalMilliseconds:F0} bodyBytes={content.Length}");
    }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
    {
        _write($"ERR method={context.Method} uri={context.Request.RequestUri} status={(statusCode.HasValue ? ((int)statusCode.Value).ToString() : "n/a")} elapsedMs={duration.TotalMilliseconds:F0} ex={exception.GetType().Name}");
    }
}
