using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core.CommonValidator;
using Microsoft.Practices.ServiceLocation;

namespace ClubPool.Core
{
  public class ValidatableEntityDto : EntityDto, IValidatable
  {
    protected IValidator Validator {
      get { return ServiceLocator.Current.GetInstance<IValidator>(); }
    }

    public virtual bool IsValid() {
      return Validator.IsValid(this);
    }

    public virtual ICollection<IValidationResult> ValidationResults() {
      return Validator.ValidationResultsFor(this);
    }
  }
}
