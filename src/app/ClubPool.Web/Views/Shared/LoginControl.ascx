<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<fieldset>
  <% if (!string.IsNullOrEmpty(Model.ErrorMessage)) { 
       Html.RenderPartial("FormError");
     } %>
  <%= Html.HiddenFor(m => m.ReturnUrl) %>
  <div class="formrow">
    <label for="Username" accesskey="u">Username:</label>
    <div class="formInput">
      <%= Html.TextBoxFor(m => m.Username, new { @class="required"}) %>
    </div>
  </div>
  <div class="formrow">
    <label for="Password" accesskey="p">Password:</label>
    <div class="formInput">
      <%= Html.PasswordFor(m => m.Password, new { @class = "required" })%>
    </div>
  </div>
  <div class="formrowspan">
    <%= Html.CheckBoxFor(m => m.StayLoggedIn) %>
    <%= Html.Label("Stay logged in") %>
  </div>
  <div class="submitrow">
    <button type="submit">Login</button>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLink<ClubPool.Web.Controllers.UsersController>(c => c.AccountHelp(), "Can't access your account?") %>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLink<ClubPool.Web.Controllers.UsersController>(c => c.SignUp(), "Not a member? Sign up here") %>
  </div>
</fieldset>