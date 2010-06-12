using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubPool.Web.Code
{
  public static class UrlHelperExtensions
  {
    public static string ContentImageUrl(this UrlHelper helper, string image) {
      return helper.Content("~/content/images/" + image);
    }
  }
}