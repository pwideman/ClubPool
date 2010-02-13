<%@ Page Language="C#" Inherits="ClubPool.Web.Views.AspxViewPageBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<div class="sidebarGadgetContainer">
  <div class="sidebarGadgetTitle">
    Login
  </div>
  <div class="sidebarGadgetContent">
    <form method="post" id="sidebarLoginForm" class="sidebarLoginForm" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.Login(string.Empty))%>">
      <% Html.RenderPartial("LoginControl"); %>
    </form>
    <script type="text/javascript">
      $(document).ready(function() {
        $("#sidebarLoginForm").validate({
          messages: {
            Username: null,
            Password: null
          },
          errorPlacement: function() { /* don't show error labels, just highlight*/ }
        });
      });
    </script>
  </div>
</div>
