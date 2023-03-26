using Blotter.Models;
using Blotter.Validators;
using System.Collections.Generic;
using PriceSupplier;

namespace Blotter.ViewModels
{
    public static class BlotterRowViewModelFactory
    {

        /// IN A REAL APP USE AUTOMAP TO MAP A MODEL TO VIEW MODEL AND USE VALIDATOR
        public static BlotterRowViewModel Create(BlotterRow row)
        {
            return new BlotterRowViewModel(row);
            
        }
        public static BlotterRowViewModel CreateEmptyBlotterRow()
        {
            return Create(new BlotterRow(null));
        }



    }
}
