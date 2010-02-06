<%@ Control Language="C#" Inherits="ClubPool.Web.Views.AspxViewUserControlBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<div class="sidebarLoginForm" id="loginForm">
  <div class="formTitle">Login</div>
  <form method="post" class="<%= Model.IsInSidebar ? "sidebarLoginForm" : "loginForm"%>" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.Login(string.Empty))%>">
    <fieldset>
      <% if (!string.IsNullOrEmpty(ViewModel.Message)) { %>
      <p class="error"><%= ViewModel.Message%></p>
      <% } %>
      <%= this.Hidden(m => m.ReturnUrl) %>
      <div class="formrow">
        <%= this.TextBox(m => m.Username).Label("Username:") %>
      </div>
      <div class="formrow">
        <%= this.TextBox(m => m.Password).Label("Password:") %>
      </div>
      <div class="spacer">&nbsp;</div>
      <div class="formrowspan">
        <%= this.CheckBox(m => m.RememberMe).LabelAfter("Remember me next time") %>
      </div>
      <div class="formrow">
        <button type="submit">Login</button>
      </div>
    </fieldset>
  </form>
</div>
<script type="text/javascript">
  $("#loginForm").corner();
  $("#loginForm > form").corner();
</script>