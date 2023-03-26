using Blotter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PriceSupplier;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Collections.Specialized;

using Blotter.Validators;

namespace Blotter.ViewModels
{

    #region documentation
    /*
    This code defines a view model for a blotter, which is a table that displays real-time prices 
    for various currency pairs. The view model subscribes to a price source cache and updates 
    the table with the latest prices for each currency pair. 
    It also allows the user to add or remove rows from the table and change the currency pairs displayed
     in each row. The code uses various classes and interfaces from the Blotter.
     Models and PriceSupplier namespaces, as well as several .NET libraries for reactive programming 
     and collections.*/
    #endregion 
    public class BlotterViewModel : IBlotterViewModel
    {
        private static readonly IValidator<string> FxPairValidator = new FxPairValidator();

        private BlotterRow[] DefaultBlotterRows = new[] { new BlotterRow("EURUSD"),
                new BlotterRow("GBPUSD"),
                new BlotterRow("USDJPY") };

        private readonly IPriceSourceCache _priceSourceCache;
        private TimeSpan _uiUpdateInterval;
        private CompositeDisposable _compositeSusbsriptions;

        public BlotterViewModel(IPriceSourceCache priceSourceCache, long uiUpdateInterval = 50)
        {
            var emptyBlooterRows = Enumerable.Range(0, 9).Select(_=> BlotterRowViewModelFactory.CreateEmptyBlotterRow()); //create 9 empty rows just as before;
            BlotterViewModelRows = new ObservableCollection<BlotterRowViewModel>(DefaultBlotterRows.Select(BlotterRowViewModelFactory.Create).Union(emptyBlooterRows));
            
            _priceSourceCache = priceSourceCache;
            _uiUpdateInterval = TimeSpan.FromMilliseconds(50);
            _compositeSusbsriptions = new CompositeDisposable();
            SubscribeToFxPriceCache(BlotterViewModelRows.Where(blotterVmRow=>!blotterVmRow.IsNotValid).Select(b=>b.CurrencyPair));
            BlotterViewModelRows.CollectionChanged += BlotterViewModelRows_CollectionChanged;
            foreach ( var blotterRowVm in BlotterViewModelRows)
            {
                blotterRowVm.CurrencyPairChanged += BlotterRowViewModel_CurrencyPairChanged;
            }
        }
        private void SubscribeToFxPriceCache(IEnumerable<string> currencyPairs)
        {         
            var subscriptions = new Dictionary<string, IDisposable>();

            // Subscribe to each currency pair
            foreach (var ccyPair in currencyPairs.Where(ccy => ccy!=null))
            {                
                var subscription = _priceSourceCache?.Subscribe(ccyPair)
                   .Select(update => update.Price)
                   .DistinctUntilChanged()
                   .Throttle(_uiUpdateInterval)
                   .ObserveOn(SynchronizationContext.Current)
                   .Subscribe(price =>
                   {
                       var rows = BlotterViewModelRows.Where(r => r.CurrencyPair == ccyPair);
                       if (! rows.Any())
                       {
                           BlotterViewModelRows.Add(new BlotterRowViewModel(new FxPairPrice().ToBlotterRow()));
                       }
                       else
                       {
                           foreach (var row in rows)
                           { 
                                row.Price = price;
                           }
                       }
                   });

                _compositeSusbsriptions.Add(subscription);
                
            }
        }
        public ObservableCollection<BlotterRowViewModel> BlotterViewModelRows { get; private set; }
        public long UiUpdateInterval { get => _uiUpdateInterval.Milliseconds; set => _uiUpdateInterval = new TimeSpan(value); }

     
        private void BlotterViewModelRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.OfType<BlotterRowViewModel>())
                {
                    newItem.CurrencyPairChanged += BlotterRowViewModel_CurrencyPairChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems.OfType<BlotterRowViewModel>())
                {
                    oldItem.CurrencyPairChanged -= BlotterRowViewModel_CurrencyPairChanged;
                }
            }
        }

        private void BlotterRowViewModel_CurrencyPairChanged(object sender, EventArgs e)
        {
            var blotterRowViewModel = (BlotterRowViewModel)sender;
            
            var currencyPair = blotterRowViewModel.CurrencyPair;

            SubscribeToFxPriceCache(new[] { currencyPair });
        }
    }
   }
