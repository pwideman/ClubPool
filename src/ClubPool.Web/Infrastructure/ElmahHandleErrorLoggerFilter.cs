using System;
using System.Web.Mvc;

using Elmah;

namespace ClubPool.Web.Infrastructure
{
  public class ElmahHandleErrorLoggerFilter : IExceptionFilter
  {
    public void OnException(ExceptionContext context)
    {
      // Log only handled exceptions, because all other will be caught by ELMAH anyway.
      if (context.ExceptionHandled)
        ErrorSignal.FromCurrentContext().Raise(context.Exception);
    }
  }
}