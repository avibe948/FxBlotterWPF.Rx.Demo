using Blotter.Models;
using PriceSupplier;

namespace Blotter.ViewModels
{
    public static class BlotterModelExtensions
    {
        public static BlotterRow ToBlotterRow(this FxPairPrice fxPairPrice)
        {
            return new BlotterRow(fxPairPrice.CurrencyPair, fxPairPrice.Price);
        }
    }


}
