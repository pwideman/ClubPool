using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpArch.Core.CommonValidator;

using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Matches.ViewModels
{
  public class EditMatchResponseViewModel : AjaxUpdateResponseViewModel
  {
    public IEnumerable<ValidationResultViewModel> ValidationResults { get; set; }

    public EditMatchResponseViewModel() : base() {
      ValidationResults = new List<ValidationResultViewModel>();
    }

    public EditMatchResponseViewModel(bool success)
      : this() {
      Success = success;
    }

    public EditMatchResponseViewModel(bool success, string message)
      : this(success) {
      Message = message;
    }

    public EditMatchResponseViewModel(bool success, string message, IEnumerable<IValidationResult> validationResults)
      : this(success, message) {
      var results = new List<ValidationResultViewModel>();
      foreach (var result in validationResults) {
        results.Add(new ValidationResultViewModel(result));
      }
      ValidationResults = results;
    }
  }

  public class ValidationResultViewModel
  {
    public object AttemptedValue { get; set; }
    public string Message { get; set; }
    public string PropertyName { get; set; }

    public ValidationResultViewModel(IValidationResult result) {
      AttemptedValue = result.AttemptedValue;
      Message = result.Message;
      PropertyName = result.PropertyName;
    }
  }

}
