using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceSupplier;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Reactive.Disposables;

namespace PriceSupplierTests
{
    [TestClass]
    public class PriceSupplierTests
    {
        [TestMethod]
        public void PriceSourceCache_Subscribe_ReturnsObservable()
        {
            // Arrange
            var cache = new PriceSourceCache((cp, ip) => new PriceSource(cp, ip));

            // Act
            var observable = cache.Subscribe("EURUSD");

            // Assert
            Assert.IsNotNull(observable);
        }

        [TestMethod]
        public void PriceSourceCache_Subscribe_ReturnsSameObservableForSameCurrencyPair()
        {
            // Arrange
            var cache = new PriceSourceCache((cp, ip) => new PriceSource(cp, ip));
            var currencyPair = "EURUSD";

            // Act
            var observable1 = cache.Subscribe(currencyPair);
            var observable2 = cache.Subscribe(currencyPair);

            // Assert
            var list1 = new List<FxPairPrice>();
            var list2 = new List<FxPairPrice>();
            using (observable1.Subscribe(new TestObserver<FxPairPrice>(list1)))
            using (observable2.Subscribe(new TestObserver<FxPairPrice>(list2)))
            {
                Assert.AreEqual(list1.Count, list2.Count);
                for (int i = 0; i < list1.Count; i++)
                {
                    Assert.AreEqual(list1[i], list2[i]);
                }
            }
        }

        [TestMethod]
        public void PriceSourceCache_Subscribe_ReturnsDifferentObservableForDifferentCurrencyPair()
        {
            // Arrange
            var cache = new PriceSourceCache((cp, ip) => new PriceSource(cp, ip));

            // Act
            var observable1 = cache.Subscribe("EURUSD");
            var observable2 = cache.Subscribe("USDJPY");

            // Assert
            Assert.AreNotSame(observable1, observable2);
        }

        [TestMethod]
        public void PriceSourceCache_Subscribe_ReturnsObservableWithCorrectCurrencyPair()
        {
            // Arrange
            var cache = new PriceSourceCache((cp, ip) => new PriceSource(cp, ip));
            var currencyPair = "EURUSD";

            // Act
            var observable = cache.Subscribe(currencyPair);

            // Assert
            Assert.AreEqual(currencyPair, observable.First().CurrencyPair);
        }

        [TestMethod]
        public void PriceSourceCache_Dispose_DisposesAllObservables()
        {
            // Arrange
            var cache = new PriceSourceCache((cp, ip) => new PriceSource(cp, ip));
            var currencyPairs = new List<string>(PriceSourceCache.AvailableCurrencyPairs.Keys);
            var disposables = new CompositeDisposable();
            var subscriptions = currencyPairs.Select(cp => cache.Subscribe(cp)).ToList();
            subscriptions.ForEach(s => disposables.Add(s.Subscribe(_ => { })));

            // Act
            cache.Dispose();
            disposables.Dispose();

            // Assert
            foreach (var subscription in subscriptions)
            {
                Assert.ThrowsException<ObjectDisposedException>(() => subscription.Subscribe(_ => { }));
            }
        }

        private class TestObserver<T> : IObserver<T>
        {
            private readonly List<T> _list;

            public TestObserver(List<T> list)
            {
                _list = list;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(T value)
            {
                _list.Add(value);
            }
        }
    }
}