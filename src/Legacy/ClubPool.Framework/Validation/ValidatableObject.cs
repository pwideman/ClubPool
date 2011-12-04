using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core.CommonValidator;
using SharpArch.Core;

namespace ClubPool.Framework.Validation
{
  public class ValidatableObject : IValidatable
  {
    protected IValidator Validator {
      get { return SafeServiceLocator<IValidator>.GetService(); }
    }

    public virtual bool IsValid() {
      return Validator.IsValid(this);
    }

    public virtual ICollection<IValidationResult> ValidationResults() {
      return Validator.ValidationResultsFor(this);
    }
  }
}
