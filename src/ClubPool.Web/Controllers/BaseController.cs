using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers
{
  public abstract class BaseController : Controller
  {
    protected ActionResult ErrorView(string message) {
      TempData[GlobalViewDataProperty.PageErrorMessage] = message;
      return View("Error");
    }

    protected JsonResult AjaxUpdate() {
      return AjaxUpdate(true, null);
    }

    protected JsonResult AjaxUpdate(bool success) {
      return AjaxUpdate(success, null);
    }

    protected JsonResult AjaxUpdate(bool success, string message) {
      return Json(new AjaxUpdateResponseViewModel(success, message));
    }
  }
}
