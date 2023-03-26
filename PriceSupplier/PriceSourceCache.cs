using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PriceSupplier
{
    #region Documentation
    /*
    The purpose of the PriceSourceCache is to provide a cache of currency pair prices that can be subscribed 
    to by other parts of the application. It uses the "BehaviorSubject" class to store the current price
     of each currency pair, and it creates a new "IPriceSource" object for each new currency pair that is 
     subscribed to. The "Subscribe" function returns an observable stream of "FxPairPrice" objects, 
     which can be used to monitor changes in the price of a particular currency pair. 
     The cache is designed to be disposed of when it is no longer needed, which will dispose of all the
      "BehaviorSubject" objects and free up any resources they were using. 
      Overall, the PriceSourceCache provides a 
    convenient way to manage and monitor currency pair prices in a reactive and efficient manner.
    This code defines a class called "PriceSourceCache" that implements the "IPriceSourceCache" interface.
     It contains a dictionary called "_cache" that maps currency pairs to "BehaviorSubject" objects. 
     It also has a function called "Subscribe" that takes a currency pair as input and returns an observable 
     stream of "FxPairPrice" objects.
      If the currency pair is not already in the cache, it creates a new "BehaviorSubject" object and 
      subscribes it to a new "IPriceSource" object created by calling the "_priceSourceCreator" function.
       The default initial price for the currency pair is obtained from the "AvailableCurrencyPairs" dictionary.
        The "Dispose" function disposes all the "BehaviorSubject" objects in the cache.
    */
    #endregion
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
