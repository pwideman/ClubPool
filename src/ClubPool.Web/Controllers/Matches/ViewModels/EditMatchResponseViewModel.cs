using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

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

    public EditMatchResponseViewModel(bool success, string message, ModelStateDictionary modelState)
      : this(success, message) {
      var results = new List<ValidationResultViewModel>();
      foreach (var result in modelState.Where(kvp => kvp.Value.Errors.Count > 0)) {
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

    public ValidationResultViewModel(KeyValuePair<string, ModelState> result) {
      if (null != result.Value.Value) {
        AttemptedValue = result.Value.Value.AttemptedValue;
      }
      Message = result.Value.Errors.First().ErrorMessage;
      PropertyName = result.Key;
    }
  }

}
