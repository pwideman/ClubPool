<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Error</title>
    <%= Html.Stylesheet("site.css") %>
    <%= Html.ScriptInclude("jquery-1.4.1.min.js") %>
    <%= Html.ScriptInclude("jquery.corner.js") %>
    <script type="text/javascript">
      // normal radius
      $(document).ready(function() {
        var normalCornerRadius = "12px";
        $(".normalRoundCorners").corner(normalCornerRadius);
      });
    </script>
</head>
<body>
    <div class="rescuediv normalRoundCorners">
      Oops! An error has occurred. Information about the problem has been logged and we'll take a look at it.
    </div>
</body>
</html>
