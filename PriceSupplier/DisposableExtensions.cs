using System;
using System.Reactive.Disposables;

namespace PriceSupplier
{
    public static class DisposableExtensions
    {
        public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
      
    }
  
}
