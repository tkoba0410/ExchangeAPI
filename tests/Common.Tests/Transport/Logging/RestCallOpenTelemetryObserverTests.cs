using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeApi.Transport.Observability;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Transport.Logging;

public class RestCallOpenTelemetryObserverTests
{
    [Fact]
    public void OnError_UsesErrorReferenceInActivityStatus()
    {
        Activity? stoppedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "test-source",
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => stoppedActivity = activity,
        };
        ActivitySource.AddActivityListener(listener);

        using var observer = new RestCallOpenTelemetryObserver("test-source");
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/private?apiKey=abc");
        var context = new RestCallContext(request);

        observer.OnRequest(context);
        observer.OnError(context, new InvalidOperationException("signature=secret"), TimeSpan.FromMilliseconds(50), HttpStatusCode.BadRequest);

        Assert.NotNull(stoppedActivity);
        Assert.Equal(ActivityStatusCode.Error, stoppedActivity!.Status);
        Assert.Matches("^error_ref=errp_v1_[0-9A-F]{16}$", stoppedActivity.StatusDescription ?? string.Empty);
        Assert.DoesNotContain("secret", stoppedActivity.StatusDescription ?? string.Empty);
    }

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
