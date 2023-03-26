using Blotter.Models;
using Blotter.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PriceSupplier;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.ComponentModel;

namespace Blotter.ViewModels
{

    public class BlotterRowViewModel : INotifyPropertyChanged
    {
        public BlotterRowViewModel(BlotterRow row)
        {
            CurrencyPair = row.CurrencyPair;
            Price = row.Price;


        }
        public BlotterRowViewModel(FxPairPrice fxPairPrice)
        {
            CurrencyPair = fxPairPrice.CurrencyPair;
            Price = fxPairPrice.Price;

        }
        private string _currencyPair;
        private decimal? _price;
        private string _error;

        public string CurrencyPair
        {
            get => _currencyPair;
            set
            {
                _currencyPair = value;
                OnPropertyChanged(nameof(CurrencyPair));
            }
        }

        public decimal? Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public string Error { get => _error; set { _error = value; OnPropertyChanged(nameof(Error)); } }

        public bool IsNotValid { get { return !string.IsNullOrEmpty(Error); } }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class BlotterRowViewModelFactory
    {
        private static readonly IValidator<string> FxPairValidator = new FxPairValidator();

        /// IN A REAL APP USE AUTOMAP TO MAP A MODEL TO VIEW MODEL AND USE VALIDATOR
        public static BlotterRowViewModel Create(BlotterRow row)
        {
            var vm = new BlotterRowViewModel(row);

            if (!FxPairValidator.TryValidate(row.CurrencyPair, out IEnumerable<string> errors))
            {
                 vm.Error = string.Join(",",errors).TrimEnd(',');               
            }
            if ( !PriceSourceCache.AvailableCurrencyPairs.ContainsKey(row.CurrencyPair))
            {
                vm.Error = $"The currency pair {vm.CurrencyPair} is not supported by the price source cache";
            }
            return vm;
        }
    }
    public class BlotterViewModel : IBlotterViewModel
    {
        private BlotterRow[] DefaultBlotterRows = new[] { new BlotterRow("EURUSD"),
                new BlotterRow("GBPUSD"),
                new BlotterRow("USDJPY")};

        private readonly IPriceSourceCache _priceSourceCache;
        private TimeSpan _uiUpdateInterval;
        private CompositeDisposable _compositeSusbsriptions;

        public BlotterViewModel(IPriceSourceCache priceSourceCache, long uiUpdateInterval = 50)
        {            
            BlotterViewModelRows = new ObservableCollection<BlotterRowViewModel>(DefaultBlotterRows.Select(BlotterRowViewModelFactory.Create));
            _priceSourceCache = priceSourceCache;
            _uiUpdateInterval = TimeSpan.FromMilliseconds(50);
            _compositeSusbsriptions = new CompositeDisposable();
            SubscribeToFxPriceCache(PriceSourceCache.AvailableCurrencyPairs.Keys);
        }
        private void SubscribeToFxPriceCache(IEnumerable<string> currencyPairs)
        {
            var subscriptions = new Dictionary<string, IDisposable>();

            // Subscribe to each currency pair
            foreach (var ccyPair in currencyPairs)
            {                
                var subscription = _priceSourceCache?.Subscribe(ccyPair)
                   .Select(update => update.Price)
                   .DistinctUntilChanged()
                   .Throttle(_uiUpdateInterval)
                   .ObserveOn(SynchronizationContext.Current)
                   .Subscribe(price =>
                   {
                       Console.WriteLine($"{ccyPair}: {price}");
                       var row = BlotterViewModelRows.FirstOrDefault(r => r.CurrencyPair == ccyPair);
                       if (row == null)
                       {
                           BlotterViewModelRows.Add(new BlotterRowViewModel(new FxPairPrice()));
                       }
                       else
                       {
                           row.Price = price;
                       }
                   });

                _compositeSusbsriptions.Add(subscription);
                
            }
        }
        public ObservableCollection<BlotterRowViewModel> BlotterViewModelRows { get; private set; }
        public long UiUpdateInterval { get => _uiUpdateInterval.Milliseconds; set => _uiUpdateInterval = new TimeSpan(value); }
    }


}
