using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Web.Controllers.Shared.ViewModels
{
  public class AjaxUpdateResponseViewModel
  {
    public string Message { get; set; }
    public bool Success { get; set; }

    public AjaxUpdateResponseViewModel(bool success, string message = null) {
      Success = success;
      Message = message;
    }
  }
}
