<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Not Found</title>
  <%= Html.Stylesheet("site.less") %>
</head>
<body>
  <div class="rescuediv corner">
    Oops! The requested resource cannot be found.
  </div>
</body>
</html>
