<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>
<fieldset>
  <%= Html.HiddenFor(m => m.ReturnUrl) %>
  <div class="form-row">
    <span class="form-label-left"><%= Html.LabelFor(m => m.Username) %></span>
    <div class="form-input">
      <%= Html.TextBoxFor(m => m.Username) %>
    </div>
  </div>
  <div class="form-row">
    <span class="form-label-left"><%= Html.LabelFor(m => m.Password) %></span>
    <div class="form-input">
      <%= Html.PasswordFor(m => m.Password)%>
    </div>
  </div>
  <div class="form-row-span">
    <%= Html.CheckBoxFor(m => m.StayLoggedIn) %>
    <%= Html.LabelFor(m => m.StayLoggedIn) %>
  </div>
  <div class="submit-row">
    <button type="submit">Login</button>
  </div>
  <div class="form-row-span">
    <%= Html.ActionLink("Can't access your account?", "AccountHelp", "Users") %>
  </div>
  <div class="form-row-span">
    <%= Html.ActionLink("Not a member? Sign up here", "SignUp", "Users") %>
  </div>
</fieldset>