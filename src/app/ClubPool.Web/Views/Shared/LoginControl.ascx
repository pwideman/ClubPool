<%@ Control Language="C#" Inherits="ClubPool.Web.Views.AspxViewUserControlBase<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>
<fieldset>
  <% if (!string.IsNullOrEmpty(ViewModel.Message)) { %>
  <p class="error">
    <%= ViewModel.Message%></p>
  <% } %>
  <%= this.Hidden(m => m.ReturnUrl) %>
  <div class="formrow">
    <%= this.TextBox(m => m.Username).Label("Username:") %>
  </div>
  <div class="formrow">
    <%= this.TextBox(m => m.Password).Label("Password:") %>
  </div>
  <div class="spacer">
    &nbsp;</div>
  <div class="formrowspan">
    <%= this.CheckBox(m => m.RememberMe).LabelAfter("Remember me next time") %>
  </div>
  <div class="formrowspan">
    <%= Html.ActionLinkForAreas<ClubPool.Web.Controllers.UserController>(c => c.ResetPassword(), "I forgot my password") %>
  </div>
  <div class="submitrow">
    <button type="submit">Login</button>
  </div>
</fieldset>
