using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers.Attributes
{
  public abstract class BaseViewDataAttribute : ActionFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext filterContext) {
      var viewResult = filterContext.Result as ViewResult;
      if (null != viewResult) {
        var viewData = viewResult.ViewData;
        if (null != viewData) {
          var viewModel = GetViewModel(filterContext);
          viewData[viewModel.GetType().FullName] = viewModel;
        }
      }
      base.OnResultExecuting(filterContext);
    }

    protected abstract object GetViewModel(ResultExecutingContext filterContext);
  }
}
