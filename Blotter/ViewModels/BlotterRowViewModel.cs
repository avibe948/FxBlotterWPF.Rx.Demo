using Blotter.Models;
using Blotter.Validators;
using PriceSupplier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Blotter.ViewModels
{
    public static class BlotterModelExtensions
    {
        public static BlotterRow ToBlotterRow(this FxPairPrice fxPairPrice)
        {
            return new BlotterRow(fxPairPrice.CurrencyPair, fxPairPrice.Price);
        }
    }
    public class BlotterRowViewModel : INotifyPropertyChanged
    {
        
        private static readonly IValidator<string> FxPairValidator = new FxPairValidator();

        public BlotterRowViewModel(BlotterRow row)
        {
            CurrencyPair = row?.CurrencyPair;
            Price = row?.Price;
                       
        }
     
        private string _currencyPair;
        private decimal? _price;
        private string _error;

        public string CurrencyPair
        {
            get => _currencyPair;
            set
            {
                if (_currencyPair != value)
                {
                    _currencyPair = value.ToUpper();

                    OnPropertyChanged(nameof(CurrencyPair));
                    ValidateRowViewModel();
                }
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

        private bool ValidateRowViewModel()
        {
            if (!FxPairValidator.TryValidate(CurrencyPair, out IEnumerable<string> errors))
            {
                Error = string.Join(",", errors).TrimEnd(',');
            }
            if (!PriceSourceCache.AvailableCurrencyPairs.ContainsKey(CurrencyPair))
            {
                Error = $"The FX ccy pair {CurrencyPair} is not supported by the price source cache";
            }
            if(errors.Any())
            {
                Console.WriteLine(string.Join("\n", errors));
                Console.WriteLine("List of supported currency pairs:");
                Console.WriteLine(string.Join("\n", PriceSourceCache.AvailableCurrencyPairs.Keys.OrderBy(ccyp => ccyp)));
                return false;
            }
            Error = null;
            return true;
        }

        public EventHandler CurrencyPairChanged { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(CurrencyPair))
            {
                CurrencyPairChanged?.Invoke(this, EventArgs.Empty);
            }
        }

    }


}
