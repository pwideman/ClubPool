<%@ Control Language="C#" Inherits="ClubPool.Web.Views.AspxViewUserControlBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<fieldset>
  <% if (!string.IsNullOrEmpty(ViewModel.Message)) { %>
  <p class="error">
    <%= ViewModel.Message%></p>
  <% } %>
  <%= this.Hidden(m => m.ReturnUrl) %>
  <div class="formrow">
    <%= this.TextBox(m => m.Username).Class("required").Label("Username:") %>
  </div>
  <div class="formrow">
    <%= this.TextBox(m => m.Password).Class("required").Label("Password:") %>
  </div>
  <div class="spacer">&nbsp;</div>
  <div class="formrowspan">
    <%= this.CheckBox(m => m.StayLoggedIn).LabelAfter("Stay logged in") %>
  </div>
  <div class="submitrow">
    <button type="submit">Login</button>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLinkForAreas<ClubPool.Web.Controllers.UserController>(c => c.AccountHelp(), "Can't access your account?") %>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLinkForAreas<ClubPool.Web.Controllers.UserController>(c => c.SignUp(), "Not a member? Sign up here") %>
  </div>
</fieldset>
