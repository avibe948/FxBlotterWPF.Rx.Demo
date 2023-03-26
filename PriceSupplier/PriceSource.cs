using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace PriceSupplier
{
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
  
}
