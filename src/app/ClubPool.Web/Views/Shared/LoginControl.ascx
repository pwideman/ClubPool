<%@ Control Language="C#" Inherits="ClubPool.Web.Views.AspxViewUserControlBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<fieldset>
  <% if (!string.IsNullOrEmpty(ViewModel.Message)) { %>
  <p class="error">
    <%= ViewModel.Message%></p>
  <% } %>
  <%= this.Hidden(m => m.ReturnUrl) %>
  <div class="formrow">
    <label for="Username" accesskey="u">Username:</label>
    <div class="formInput">
      <%= this.TextBox(m => m.Username).Class("required") %>
    </div>
  </div>
  <div class="formrow">
    <label for="Password" accesskey="p">Password:</label>
    <div class="formInput">
      <%= this.TextBox(m => m.Password).Class("required") %>
    </div>
  </div>
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