<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<div class="sidebar-form-gadget">
  <% using (var form = Html.BeginForm("Login", "Users", FormMethod.Post)) { %>
    <% Html.RenderPartial("LoginControl"); %>
  <% } %>
</div>