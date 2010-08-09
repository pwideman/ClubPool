using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Machine.Specifications;

using ClubPool.Web.Controllers;

namespace ClubPool.MSpecTests
{
  public class ActionResultHelper<T> where T : ActionResult
  {
    public T Result;

    public ActionResultHelper(ActionResult result) {
      Result = result as T;
    }
  }

  public abstract class ViewResultBaseHelper<TResult, TModel> : ActionResultHelper<TResult> 
    where TResult : ViewResultBase
    where TModel : class
  {
    public TModel Model;
    public SidebarGadgetCollection SidebarGadgets;

    public ViewResultBaseHelper(ActionResult result)
      : base(result) {

      Model = Result.ViewData.Model as TModel;
      if (Result.ViewData.ContainsKey(GlobalViewDataProperty.SidebarGadgetCollection)) {
        SidebarGadgets = Result.ViewData[GlobalViewDataProperty.SidebarGadgetCollection] as SidebarGadgetCollection;
      }
    }
  }

  public class ViewResultHelper<T> : ViewResultBaseHelper<ViewResult, T> where T : class
  {
    public ViewResultHelper(ActionResult result)
      : base(result) {
    }
  }

  public class PartialViewResultHelper<T> : ViewResultBaseHelper<PartialViewResult, T> where T : class
  {
    public PartialViewResultHelper(ActionResult result)
      : base(result) {
    }
  }

  public class RedirectToRouteResultHelper : ActionResultHelper<RedirectToRouteResult>
  {
    public RedirectToRouteResultHelper(ActionResult result)
      : base(result) {
    }

    public string Controller {
      get {
        return Result.RouteValues["Controller"].ToString();
      }
    }

    public string Action {
      get {
        return Result.RouteValues["Action"].ToString();
      }
    }

    public void ShouldRedirectTo(string controller, string action) {
      Controller.ToLower().ShouldEqual(controller.ToLower());
      Action.ToLower().ShouldEqual(action.ToLower());
    }

    public void ShouldRedirectTo(string controller) {
      ShouldRedirectTo(controller, "index");
    }
  }

  public class RedirectResultHelper : ActionResultHelper<RedirectResult>
  {
    public RedirectResultHelper(ActionResult result)
      : base(result) {
    }
  }

  public class HttpNotFoundResultHelper : ActionResultHelper<HttpNotFoundResult>
  {
    public HttpNotFoundResultHelper(ActionResult result)
      : base(result) {
    }
  }
}
