using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClubPool.Web.Controllers
{
  /// <summary>An implementation of <see cref="ActionResult" /> that throws an <see cref="HttpException" />.</summary>
  public class HttpNotFoundResult : ActionResult
  {
    /// <summary>Initializes a new instance of <see cref="HttpNotFoundResult" /> with the specified <paramref name="message"/>.</summary>
    /// <param name="message"></param>
    public HttpNotFoundResult(String message) {
      this.Message = message;
    }

    /// <summary>Initializes a new instance of <see cref="HttpNotFoundResult" /> with an empty message.</summary>
    public HttpNotFoundResult()
      : this(String.Empty) { }

    /// <summary>Gets or sets the message that will be passed to the thrown <see cref="HttpException" />.</summary>
    public String Message { get; set; }

    /// <summary>Overrides the base <see cref="ActionResult.ExecuteResult" /> functionality to throw an <see cref="HttpException" />.</summary>
    public override void ExecuteResult(ControllerContext context) {
      throw new HttpException((int)HttpStatusCode.NotFound, this.Message);
    }
  }
}