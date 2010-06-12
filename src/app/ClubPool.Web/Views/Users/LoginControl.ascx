<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<fieldset>
  <% if (!string.IsNullOrEmpty(Model.ErrorMessage)) { 
       Html.RenderPartial("FormError");
     } %>
  <%= Html.HiddenFor(m => m.ReturnUrl) %>
  <div class="formrow">
    <%= Html.LabelFor(m => m.Username) %>
    <div class="formInput">
      <%= Html.TextBoxFor(m => m.Username, new { @class="required"}) %>
    </div>
  </div>
  <div class="formrow">
    <%= Html.LabelFor(m => m.Password) %>
    <div class="formInput">
      <%= Html.PasswordFor(m => m.Password, new { @class = "required" })%>
    </div>
  </div>
  <div class="formrowspan">
    <%= Html.CheckBoxFor(m => m.StayLoggedIn) %>
    <%= Html.LabelFor(m => m.StayLoggedIn) %>
  </div>
  <div class="submitrow">
    <button type="submit">Login</button>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.AccountHelp(), "Can't access your account?") %>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.SignUp(), "Not a member? Sign up here") %>
  </div>
</fieldset>