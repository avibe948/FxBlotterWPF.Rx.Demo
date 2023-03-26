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
  
}
