<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<div class="sidebarGadgetContainer sidebar-corner">
  <div class="sidebarGadgetTitle">Login</div>
  <div class="sidebarGadgetContent sidebar-corner">
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Login(string.Empty),
          FormMethod.Post, new { @class = "sidebarLoginForm sidebar", id = "sidebarLoginForm" })) { %>
      <% Html.RenderPartial("LoginControl"); %>
    <% } %>
    <script type="text/javascript">
      $(document).ready(function() {
        $("#sidebarLoginForm").validate({
          errorPlacement: function() { /* don't show error labels, just highlight*/ }
        });
      });
    </script>
  </div>
</div>
