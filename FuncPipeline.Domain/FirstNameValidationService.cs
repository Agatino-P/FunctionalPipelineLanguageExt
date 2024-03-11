using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncPipeline.Domain;
internal class FirstNameValidationService : IFirstNameValidationService
{
    public bool IsValid(string firstName)
        => string.IsNullOrEmpty(firstName);

}
