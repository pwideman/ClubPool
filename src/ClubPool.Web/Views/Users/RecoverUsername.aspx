<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.RecoverUsernameViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controls.Captcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h5>Recover Username</h5>
<p>Enter the email address registered for the lost username below. An email containing all registered usernames for that
address will be sent.</p>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.RecoverUsername(), FormMethod.Post)) { %>
  <%= Html.AntiForgeryToken() %>
  <div>Email: <%= Html.TextBoxFor(m => m.Email, new { @class = "email" })%></div>
  <p>
    <div><%= Html.CaptchaImage(50, 180)%></div>
    <div><label for="captcha">Enter the text from the image above:</label></div>
    <div><%= Html.CaptchaTextBox("captcha")%><%= Html.ValidationMessage("captcha")%></div>
  </p>
  <p><%= Html.SubmitButton("submit", "Recover Username") %></p>
<% } %>
<% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) { 
       Html.RenderPartial("ErrorMessage");
   } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Recover Username
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<style type="text/css">
  input.email
  {
    width: 200px;
  }
</style>
</asp:Content>
