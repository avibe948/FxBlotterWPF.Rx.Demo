using System;

namespace PriceSupplier
{
    public interface IPriceSource : IObservable<FxPairPrice>
    {
       
    }
  
}
