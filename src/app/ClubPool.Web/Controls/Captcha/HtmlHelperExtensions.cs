// this code is from Nick Berardi's Coder Journal: http://www.coderjournal.com/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace ClubPool.Web.Controls.Captcha
{
  /// <summary>
  /// 
  /// </summary>
  public static class HtmlHelperExtensions
  {
    /// <summary>
    /// Creates a captcha text box
    /// </summary>
    /// <param name="helper">The helper.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static string CaptchaTextBox(this HtmlHelper helper, string name)
    {
      return String.Format(@"<input type=""text"" id=""{0}"" name=""{0}"" value="""" maxlength=""{1}"" autocomplete=""off"" />",
          name,
          ClubPool.Web.Controls.Captcha.CaptchaImage.TextLength);

    }

    /// <summary>
    /// Generates the captcha image.
    /// </summary>
    /// <param name="helper">The helper.</param>
    /// <param name="height">The height.</param>
    /// <param name="width">The width.</param>
    /// <returns>
    /// Returns the <see cref="Uri"/> for the generated <see cref="CaptchaImage"/>.
    /// </returns>
    public static string CaptchaImage(this HtmlHelper helper, int height, int width)
    {
      CaptchaImage image = new CaptchaImage {
        Height = height,
        Width = width,
      };

      HttpRuntime.Cache.Add(
        image.UniqueId,
        image,
        null,
        DateTime.Now.AddSeconds(ClubPool.Web.Controls.Captcha.CaptchaImage.CacheTimeOut),
        Cache.NoSlidingExpiration,
        CacheItemPriority.NotRemovable,
        null);

      StringBuilder stringBuilder = new StringBuilder(256);
      stringBuilder.Append("<input type=\"hidden\" name=\"captcha-guid\" value=\"");
      stringBuilder.Append(image.UniqueId);
      stringBuilder.Append("\" />");
      stringBuilder.AppendLine();
      stringBuilder.Append("<img src=\"");
      var url = VirtualPathUtility.ToAbsolute("~/");
      stringBuilder.Append(url + "captcha.axd?guid=" + image.UniqueId);
      stringBuilder.Append("\" alt=\"CAPTCHA\" width=\"");
      stringBuilder.Append(width);
      stringBuilder.Append("\" height=\"");
      stringBuilder.Append(height);
      stringBuilder.Append("\" />");

      return stringBuilder.ToString();
    }
  }
}
