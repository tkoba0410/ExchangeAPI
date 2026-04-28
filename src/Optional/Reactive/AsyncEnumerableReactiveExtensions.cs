using System.Reactive.Disposables;

namespace ExchangeApi.Optional.Reactive;

public static class AsyncEnumerableReactiveExtensions
{
    public static IObservable<T> ToObservable<T>(this IAsyncEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return new AsyncEnumerableObservable<T>(source);
    }

    private sealed class AsyncEnumerableObservable<T>(IAsyncEnumerable<T> source) : IObservable<T>
    {
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);

            var cancellation = new CancellationTokenSource();
            _ = RunAsync(source, observer, cancellation);

            return Disposable.Create(() =>
            {
                try
                {
                    cancellation.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // The enumeration may have already completed and disposed the token source.
                }
            });
        }

        private static async Task RunAsync(
            IAsyncEnumerable<T> source,
            IObserver<T> observer,
            CancellationTokenSource cancellation)
        {
            try
            {
                await foreach (var item in source.WithCancellation(cancellation.Token).ConfigureAwait(false))
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        break;
                    }

                    observer.OnNext(item);
                }

                if (!cancellation.IsCancellationRequested)
                {
                    observer.OnCompleted();
                }
            }
            catch (OperationCanceledException)
            {
                // Subscription disposal and source cancellation are terminal-notification neutral.
            }
            catch (Exception exception)
            {
                if (!cancellation.IsCancellationRequested)
                {
                    observer.OnError(exception);
                }
            }
            finally
            {
                cancellation.Dispose();
            }
        }
    }
}
