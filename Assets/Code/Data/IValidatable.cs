using System.Collections.Generic;

public interface IValidatable
{
    public void Validate(out ValidationResult result, out List<string> errorMessages);
}