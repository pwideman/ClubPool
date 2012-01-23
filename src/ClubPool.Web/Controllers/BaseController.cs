using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

using ClubPool.Web.Infrastructure;
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

    protected void InitializePagedListViewModel<TSource, TResult>(PagedListViewModelBase<TSource, TResult> model, IQueryable<TSource> source, int page, int pageSize, Func<TSource, TResult> converter) {
      model.Total = source.Count();
      model.TotalPages = (int)Math.Ceiling((double)model.Total / (double)pageSize);
      model.CurrentPage = Math.Min(page, model.TotalPages);
      var index = Math.Max(model.CurrentPage - 1, 0);
      model.Items = source.Page(index, pageSize).ToList().Select(i => converter(i));
      model.First = index * pageSize + 1;
      model.Last = model.First + model.Items.Count() - 1;
      if (model.TotalPages > 2) {
        var numPageLinksToShow = 5;
        var first = Math.Max(model.CurrentPage - numPageLinksToShow / 2, 1);
        var last = Math.Min(model.CurrentPage + numPageLinksToShow / 2, model.TotalPages);
        if (last - first < numPageLinksToShow - 1) {
          if (1 == first) {
            last += numPageLinksToShow - (last - first) - 1;
          }
          else {
            first -= numPageLinksToShow - (last - first) - 1;
          }
          last = Math.Min(last, model.TotalPages);
          first = Math.Max(first, 1);
        }
        model.FirstPageNumberLink = first;
        model.LastPageNumberLink = last;
      }
    }
  }
}
