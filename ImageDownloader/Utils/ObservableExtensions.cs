using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ImageDownloader.Utils
{
    public static class ObservableExtensions
    {
        public static IObservable<Unit> IgnoreValue<T>(this IObservable<T> source)
        {
            return source.Select(x => Unit.Default);
        }
    }
}
