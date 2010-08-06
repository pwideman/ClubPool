using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ClubPool.MSpecTests
{
  public class ViewResultHelper<T> where T : class
  {
    public ViewResult ViewResult;
    public T Model;

    public ViewResultHelper(ActionResult actionResult) {
      ViewResult = actionResult as ViewResult;
      if (null != ViewResult) {
        Model = ViewResult.ViewData.Model as T;
      }
    }
  }

}
