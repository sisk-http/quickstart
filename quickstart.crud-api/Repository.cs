using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api;

public class Repository<TModel> : IEnumerable<TModel> where TModel : notnull
{
    internal List<TModel> items = new List<TModel>();

    public void Add(TModel item)
    {
        List<ValidationResult> validatedResults = new List<ValidationResult>();
        ValidationContext validationContext = new ValidationContext(item);
        var isValidated = Validator.TryValidateObject(item, validationContext, validatedResults);

        if (!isValidated)
        {
            throw new ValidationException(validatedResults[0], null, item);
        }

        items.Add(item);
    }

    public bool Remove(TModel item)
    {
        return items.Remove(item);
    }

    public IEnumerator<TModel> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
}
