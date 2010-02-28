<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Not Found</title>
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
    Oops! The requested resource cannot be found.
  </div>
</body>
</html>
