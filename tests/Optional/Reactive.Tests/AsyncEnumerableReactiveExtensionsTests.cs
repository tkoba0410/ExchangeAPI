using System.Runtime.CompilerServices;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Optional.Reactive;

namespace ExchangeApi.Tests.Optional.Reactive;

public sealed class AsyncEnumerableReactiveExtensionsTests
{
    [Fact]
    public async Task ToObservable_ForwardsItemsAndCompletion()
    {
        var observer = new TestObserver<int>();

        using var subscription = Numbers(1, 2, 3).ToObservable().Subscribe(observer);
        await observer.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal([1, 2, 3], observer.Items);
        Assert.True(observer.Completed);
        Assert.Null(observer.Error);
    }

    [Fact]
    public async Task ToObservable_ForwardsSourceExceptionAsOnError()
    {
        var observer = new TestObserver<int>();

        using var subscription = ThrowingSource().ToObservable().Subscribe(observer);
        await observer.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Empty(observer.Items);
        Assert.False(observer.Completed);
        Assert.IsType<InvalidOperationException>(observer.Error);
    }

    [Fact]
    public async Task ToObservable_DisposeCancelsEnumerationWithoutTerminalNotification()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var canceled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var observer = new TestObserver<int>();

        using var subscription = CancellableSource(started, canceled).ToObservable().Subscribe(observer);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        subscription.Dispose();
        await canceled.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Empty(observer.Items);
        Assert.False(observer.Completed);
        Assert.Null(observer.Error);
        Assert.False(observer.Terminal.Task.IsCompleted);
    }

    [Fact]
    public async Task ToObservable_ReEnumeratesSourceForEachSubscription()
    {
        var enumerationCount = 0;
        var observable = CountedSource(() => Interlocked.Increment(ref enumerationCount)).ToObservable();
        var first = new TestObserver<int>();
        var second = new TestObserver<int>();

        using var firstSubscription = observable.Subscribe(first);
        using var secondSubscription = observable.Subscribe(second);
        await first.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await second.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Equal(2, enumerationCount);
        Assert.Equal([1], first.Items);
        Assert.Equal([1], second.Items);
        Assert.True(first.Completed);
        Assert.True(second.Completed);
    }

    [Fact]
    public async Task ToObservable_ForwardsEnvelopeEventAsOnNext()
    {
        var observer = new TestObserver<BitflyerRealtimeStreamEvent<int>>();
        var diagnostic = new BitflyerRealtimeDiagnostic<int>
        {
            Channel = "channel",
            OccurredAt = DateTimeOffset.UtcNow,
            Diagnostic = new RealtimeDiagnosticEvent
            {
                EventType = RealtimeDiagnosticEventTypes.MessageRejected,
                ObservedAt = DateTimeOffset.UtcNow,
                Severity = RealtimeDiagnosticSeverities.Warning,
            },
        };

        using var subscription = Single(diagnostic).ToObservable().Subscribe(observer);
        await observer.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var item = Assert.Single(observer.Items);
        Assert.Same(diagnostic, item);
        Assert.True(observer.Completed);
        Assert.Null(observer.Error);
    }

    [Fact]
    public void ToObservable_NullSourceThrowsArgumentNullException()
    {
        IAsyncEnumerable<int>? source = null;

        Assert.Throws<ArgumentNullException>(() => source!.ToObservable());
    }

    [Fact]
    public async Task ToObservable_DoesNotCallItemToString()
    {
        var observer = new TestObserver<ThrowingToStringItem>();
        var item = new ThrowingToStringItem();

        using var subscription = Single(item).ToObservable().Subscribe(observer);
        await observer.Terminal.Task.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.Same(item, Assert.Single(observer.Items));
        Assert.True(observer.Completed);
        Assert.Null(observer.Error);
    }

    private static async IAsyncEnumerable<int> Numbers(params int[] values)
    {
        foreach (var value in values)
        {
            await Task.Yield();
            yield return value;
        }
    }

    private static async IAsyncEnumerable<T> Single<T>(T value)
    {
        await Task.Yield();
        yield return value;
    }

    private static async IAsyncEnumerable<int> ThrowingSource()
    {
        await Task.Yield();
        throw new InvalidOperationException("source failed");
        #pragma warning disable CS0162
        yield return 0;
        #pragma warning restore CS0162
    }

    private static async IAsyncEnumerable<int> CancellableSource(
        TaskCompletionSource started,
        TaskCompletionSource canceled,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        started.SetResult();

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
        finally
        {
            canceled.SetResult();
        }

        yield break;
    }

    private static async IAsyncEnumerable<int> CountedSource(Action onEnumerated)
    {
        onEnumerated();
        await Task.Yield();
        yield return 1;
    }

    private sealed class ThrowingToStringItem
    {
        public override string ToString()
        {
            throw new InvalidOperationException("ToString must not be called");
        }
    }

    private sealed class TestObserver<T> : IObserver<T>
    {
        public List<T> Items { get; } = [];

        public Exception? Error { get; private set; }

        public bool Completed { get; private set; }

        public TaskCompletionSource Terminal { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void OnCompleted()
        {
            Completed = true;
            Terminal.TrySetResult();
        }

        public void OnError(Exception error)
        {
            Error = error;
            Terminal.TrySetResult();
        }

        public void OnNext(T value)
        {
            Items.Add(value);
        }
    }
}
