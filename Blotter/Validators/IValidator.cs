using System.Collections.Generic;

namespace Blotter.Validators
{
    public interface IValidator<T>
    {
        bool TryValidate(T obj, out IEnumerable<string> errors);
    }


}
