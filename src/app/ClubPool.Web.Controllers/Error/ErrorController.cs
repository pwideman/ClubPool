using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers.Error
{
  /// <summary>
  /// This class is based on similar functionality from the Who Can Help Me? showcase app
  /// </summary>
  public class ErrorController : BaseController
  {
    /// <summary>
    /// Action that deals with Unhandled Server Exceptions
    /// </summary>
    /// <returns>
    /// Unhandled Error View.
    /// </returns>
    public ActionResult Error() {
      this.Response.StatusCode = 500;
      this.Response.StatusDescription = "Internal Server Error";
      this.Response.TrySkipIisCustomErrors = true;
      return this.View();
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// </returns>
    public ActionResult InvalidInput() {
      this.Response.StatusCode = 400;
      this.Response.StatusDescription = "Bad Request";
      this.Response.TrySkipIisCustomErrors = true;
      return this.View();
    }

    /// <summary>
    /// Not found action.
    /// </summary>
    /// <returns>
    /// Not found view
    /// </returns>
    public ActionResult NotFound() {
      this.Response.StatusCode = 404;
      this.Response.StatusDescription = "Not Found";
      this.Response.TrySkipIisCustomErrors = true;
      return this.View();
    }
  }
}
