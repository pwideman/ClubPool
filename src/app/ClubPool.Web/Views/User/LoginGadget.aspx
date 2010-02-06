<%@ Page Language="C#" Inherits="ClubPool.Web.Views.AspxViewPageBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<div class="sidebarGadgetContainer">
  <div class="sidebarGadgetTitle">
    Login
  </div>
  <div class="sidebarGadgetContent">
    <form method="post" class="sidebarLoginForm" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.Login(string.Empty))%>">
      <% Html.RenderPartial("LoginControl"); %>
    </form>
  </div>
</div>
