using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeApi.Shared.Transport.Observability;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Transport.Logging;

public class RestCallOpenTelemetryObserverTests
{
    [Fact]
    public async Task RecordsMetrics_OnResponse()
    {
        using var listener = new MeterListener();
        double? recorded = null;
        listener.InstrumentPublished += (instrument, l) =>
        {
            if (instrument.Meter.Name == "exchangeapi")
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
        {
            recorded = measurement;
        });
        listener.Start();

        var observer = new RestCallOpenTelemetryObserver("test-source");
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/api");
        var context = new RestCallContext(request);

        observer.OnRequest(context);
        observer.OnResponse(context, new HttpResponseMessage(HttpStatusCode.OK), "ok", TimeSpan.FromMilliseconds(100));

        // メトリクスの発行を待つ
        await Task.Delay(10);
        listener.Dispose();

        Assert.NotNull(recorded);
        Assert.True(recorded.Value > 0);
    }
}
