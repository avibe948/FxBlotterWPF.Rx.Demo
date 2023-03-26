using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Blotter.Validators
{
    /// <summary>
    /// Validates FxPair string to match the pattern CCY1/CCY2  , in a real app with will use FluentValidator Nuget app.
    /// </summary>
    public class FxPairValidator : IValidator<string>
    {
        public bool TryValidate(string ccyPair, out IEnumerable<string> errors)
        {
            errors = Enumerable.Empty<string>();
            if (string.IsNullOrEmpty(ccyPair))
            {
                errors = new List<string>(1) { "the currency pair must not be null" };
                return false;
            }
            Regex regex = new Regex(@"^[A-Z]{3}[A-Z]{3}$");

            var isMatched =  regex.IsMatch(ccyPair);
            if(!isMatched)
            {
                errors = new List<string>(1) { $"the currency pair {ccyPair} does not match the regEx pattern CCY1/CCY2 , i.e GBP/USD" };
                return false;
            }
            return true;
        }
    }


}
