<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.ResetPasswordViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controls.Captcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h5>Reset Password</h5>
<p>Enter your username or email address below to have an email containing password reset instructions sent to the email address registered with
the username (or to the supplied email address).</p>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.ResetPassword(), FormMethod.Post)) { %>
  <%= Html.AntiForgeryToken() %>
  <div><span class="resetpassword-label">Username:</span><%= Html.TextBoxFor(m => m.Username, new { @class = "resetpassword-field" })%></div>
  <div><span class="resetpassword-label">Email:</span><%= Html.TextBoxFor(m => m.Email, new { @class="resetpassword-field"}) %></div>
  <p>
  <div><%= Html.CaptchaImage(50, 180)%></div>
  <div><label for="captcha">Enter the text from the image above:</label></div>
  <div><%= Html.CaptchaTextBox("captcha")%><%= Html.ValidationMessage("captcha")%></div>
  </p>
  <div><%= Html.SubmitButton("submit", "Reset Password") %></div>
<% } %>
<%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.ResetPasswordViewModel>().DisableMessages() %>
<% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) { 
       Html.RenderPartial("ErrorMessage");
   } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Reset Password
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<style type="text/css">
  input.username
  {
    width: 200px;
  }
</style>
</asp:Content>
