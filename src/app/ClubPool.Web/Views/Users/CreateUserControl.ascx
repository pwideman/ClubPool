<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ClubPool.Web.Controllers.Users.ViewModels.CreateViewModel>" %>
<div class="formrow">
  <%= Html.LabelFor(m => m.Username)%>
  <div class="formInput">
    <%= Html.TextBoxFor(m => m.Username)%>
    <%= Html.ValidationMessageFor(m => m.Username)%>
  </div>
</div>
<div class="formrow">
  <%= Html.LabelFor(m => m.Password)%>
  <div class="formInput">
    <%= Html.PasswordFor(m => m.Password)%>
    <%= Html.ValidationMessageFor(m => m.Password)%>
  </div>
</div>
<div class="formrow">
  <%= Html.LabelFor(m => m.ConfirmPassword)%>
  <div class="formInput">
    <%= Html.PasswordFor(m => m.ConfirmPassword)%>
    <%= Html.ValidationMessageFor(m => m.ConfirmPassword)%>
  </div>
</div>
<div class="formrow">
  <%= Html.LabelFor(m => m.Email)%>
  <div class="formInput">
    <%= Html.TextBoxFor(m => m.Email)%>
    <%= Html.ValidationMessageFor(m => m.Email)%>
  </div>
</div>
<div class="formrow">
  <label for="FirstName">Name:</label>
  <div class="formInput">
    <%= Html.TextBoxFor(m => m.FirstName, new { @class = "short" })%>
    <%= Html.LabelFor(m => m.FirstName)%>
    <%= Html.ValidationMessageFor(m => m.FirstName)%>
  </div>
  <div class="formInput">
    <%= Html.TextBoxFor(m => m.LastName, new { @class = "short" })%>
    <%= Html.LabelFor(m => m.LastName)%>
    <%= Html.ValidationMessageFor(m => m.LastName)%>
  </div>
</div>

