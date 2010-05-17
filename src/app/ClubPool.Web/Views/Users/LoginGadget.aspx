<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<div class="sidebarGadgetContainer sidebar-corner">
  <div class="sidebarGadgetTitle">Login</div>
  <div class="sidebarGadgetContent sidebar-corner">
    <form method="post" id="sidebarLoginForm" class="sidebarLoginForm sidebar" action="<%= Html.BuildUrlFromExpression<ClubPool.Web.Controllers.UsersController>(c => c.Login(string.Empty))%>">
      <% Html.RenderPartial("LoginControl"); %>
    </form>
    <script type="text/javascript">
      $(document).ready(function() {
        $("#sidebarLoginForm").validate({
          errorPlacement: function() { /* don't show error labels, just highlight*/ }
        });
      });
    </script>
  </div>
</div>
