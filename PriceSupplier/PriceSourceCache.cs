using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PriceSupplier
{
    public class PriceSourceCache : IPriceSourceCache
    {
        private readonly Dictionary<string, BehaviorSubject<FxPairPrice>> _cache = new Dictionary<string, BehaviorSubject<FxPairPrice>>();
        private readonly Func<string, decimal, IPriceSource> _priceSourceCreator;
        public static readonly Dictionary<string, decimal> AvailableCurrencyPairs = new Dictionary<string, decimal>
        {
                {"AUDUSD", 0.66m},
                {"EURUSD", 1.07m},
                {"GBPUSD", 1.21m},
                {"USDCAD", 1.38m},
                {"USDCHF", 0.91m},
                {"USDJPY", 133m},
                {"USDNOK", 10.6m},
                {"USDNZD", 0.62m},
                {"USDSEK", 10.71m}
        };
        
        public PriceSourceCache(Func<string,decimal, IPriceSource> priceSourceCreator)
        {
            _priceSourceCreator = priceSourceCreator;
        }
        

     
        public IObservable<FxPairPrice> Subscribe(string currencyPair)
        {
            if (!_cache.TryGetValue(currencyPair, out BehaviorSubject<FxPairPrice> priceSubject))
            {
               AvailableCurrencyPairs.TryGetValue(currencyPair, out decimal defaultInitialPrice);
                var priceSource = _priceSourceCreator(currencyPair, defaultInitialPrice);
                priceSubject = new BehaviorSubject<FxPairPrice>(new FxPairPrice(currencyPair, defaultInitialPrice));
                priceSource.Subscribe(priceSubject);
                _cache[currencyPair] = priceSubject;
            }

            return priceSubject.AsObservable();
        }

        public void Dispose()
        {
            foreach (var subject in _cache.Values)
            {
                subject.Dispose();
            }

            _cache.Clear();
        }
    }
  
}
