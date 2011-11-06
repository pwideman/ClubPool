using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core.CommonValidator;
using SharpArch.Core;

namespace ClubPool.Web.Controllers
{
  // this class is heavily influenced by (and most of the code copied from) the ValidatableValueObject
  // in Who Can Help Me (http://whocanhelpme.codeplex.com/)
  public class ValidatableViewModel : ViewModelBase, IValidatable
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
