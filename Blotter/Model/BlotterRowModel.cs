using System.ComponentModel;
using System.Windows;
using PriceSupplier;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blotter.Models
{

    public class BlotterRow
    {
        public BlotterRow(string ccypair, decimal? price=null)
        {
            CurrencyPair = ccypair;
            if(price!=null)
               Price = price.Value;
        }
      
        public string CurrencyPair { get; private set; }
        public decimal Price { get; set; }
    
    }
     

}
