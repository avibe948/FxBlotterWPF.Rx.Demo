using System;

namespace PriceSupplier
{
    public interface IPriceSourceCache : IDisposable
    {
        IObservable<FxPairPrice> Subscribe(string currencyPair);
       
    }
}