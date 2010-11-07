using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers
{
  public class HttpInternalServerErrorResult : ContentResult
  {
    public HttpInternalServerErrorResult()
      : base() {
    }

    public HttpInternalServerErrorResult(string content)
      : base() {
      Content = content;
    }

    public override void ExecuteResult(ControllerContext context) {
      context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      base.ExecuteResult(context);
    }

  }
}
