using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Shared.ViewModels
{
  public class AjaxUpdateResponseViewModel
  {
    public string Error { get; set; }
    public bool Success { get; set; }

    public AjaxUpdateResponseViewModel(bool success, string error = null) {
      Success = success;
      Error = error;
    }
  }
}
