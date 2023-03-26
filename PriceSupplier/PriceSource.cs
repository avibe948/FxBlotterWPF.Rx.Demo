using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Subjects;

namespace PriceSupplier
{
    public struct FxPairPrice
    {
        public string CurrencyPair { get; }
        public decimal Price { get; }

        public FxPairPrice(string fxPair, decimal price)
        {
            CurrencyPair = fxPair;
            Price = price;
        }
    }

    public static class DisposableExtensions
    {
        public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
      
    }
    public class PriceSource : IPriceSource
    {
        private readonly Random _rng = new Random();
        private readonly int _rounding = 4;
        private readonly string _currencyPair;
        private readonly decimal _initialPrice;

        public PriceSource(string currencyPair, decimal initialPrice)
        {
            if (currencyPair.Equals("USDJPY", StringComparison.OrdinalIgnoreCase))
            {
                _rounding = 2;
            }

            _currencyPair = currencyPair;
            _initialPrice = initialPrice;
        }

        public IDisposable Subscribe(IObserver<FxPairPrice> observer)
        {
            var cancellation = new CancellationDisposable();
            var compositeDisposable = new CompositeDisposable();
            Observable.Create<FxPairPrice>(async (obs, cancel) =>
            {
                while (!cancel.IsCancellationRequested)
                {
                    var price = Math.Round(_initialPrice * (1 + ((decimal)_rng.NextDouble() - 0.5m) / 100), _rounding);
                    obs.OnNext(new FxPairPrice(_currencyPair, price));
                    await Task.Delay(_rng.Next(50, 2000), cancel);
                }

                obs.OnCompleted();
            })
            .Subscribe(observer)
            .AddTo(compositeDisposable);

            compositeDisposable.Add(cancellation);

            return compositeDisposable;
        }
    }

    public class PriceSourceCache : IPriceSourceCache
    {
        private readonly Dictionary<string, BehaviorSubject<FxPairPrice>> _cache = new Dictionary<string, BehaviorSubject<FxPairPrice>>();
        private readonly Func<string, decimal, IPriceSource> _priceSourceCreator;
        public static readonly Dictionary<string,decimal> AvailableCurrencyPairs = new Dictionary<string, decimal>
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
            InitializePriceSourceCache();
        }
        

        private void InitializePriceSourceCache()
        {
            foreach( KeyValuePair<string,decimal> kvp in AvailableCurrencyPairs)
            {
                if (!_cache.ContainsKey(kvp.Key))
                    _cache.Add(kvp.Key, new BehaviorSubject<FxPairPrice>(new FxPairPrice(kvp.Key, kvp.Value)));

            }
        }
        public IObservable<FxPairPrice> Subscribe(string currencyPair, decimal initialPriceDefault=100)
        {
            if (!_cache.TryGetValue(currencyPair, out BehaviorSubject<FxPairPrice> priceSubject))
            {
                var priceSource = _priceSourceCreator(currencyPair, initialPriceDefault);
                priceSubject = new BehaviorSubject<FxPairPrice>(new FxPairPrice(currencyPair, initialPriceDefault));
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
