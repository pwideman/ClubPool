using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc.Internal;

namespace ClubPool.Web.Controllers
{
  // this code based on Steve Sanderson's PartialRequest class from: http://blog.stevensanderson.com/2008/10/14/partial-requests-in-aspnet-mvc/
  public class PartialRequest
  {
    public RouteValueDictionary RouteValues { get; private set; }

    public void SetAction<TController>(Expression<Action<TController>> action) where TController : Controller {
      RouteValues = ExpressionHelper.GetRouteValuesFromExpression(action);
    }

    public void Invoke(ControllerContext context) {
      RouteData rd = new RouteData(context.RouteData.Route, context.RouteData.RouteHandler);
      foreach (var pair in RouteValues)
        rd.Values.Add(pair.Key, pair.Value);
      IHttpHandler handler = new MvcHandler(new RequestContext(context.HttpContext, rd));
      handler.ProcessRequest(System.Web.HttpContext.Current);
    }
  }
}
