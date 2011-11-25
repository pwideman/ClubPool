<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.SignUpViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controls.Captcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <strong>Sign up for a new <%= Html.Encode(Model.SiteName) %> account</strong>

  <p>Complete the form below to sign up for a new league account. A site administrator will
  need to verify your account information before you will be able to log in. You will receive
  an email once an administrator has verified your information.</p>
  
  <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
       Html.RenderPartial("ErrorMessage");
     } %>
  <div class="form-content">
    <% using (var form = Html.BeginForm("SignUp", "Users", FormMethod.Post, new { @class = "normal" })) { %>
      <fieldset>
        <%= Html.AntiForgeryToken()%>
        <% Html.RenderPartial("CreateUserControl"); %>
        <div class="spacer">&nbsp;</div>
        <div class="form-row">
          <span class="form-label-left"><%= Html.CaptchaImage(50, 180)%></span>
          <div class="form-input">
            <span class="form-sublabel"><label for="captcha">Enter the text from the image below:</label></span><br />
            <%= Html.CaptchaTextBox("captcha")%>
            <%= Html.ValidationMessage("captcha")%>
          </div>
        </div>
        <div class="spacer">&nbsp;</div>
        <div class="form-row-span">
          <input class="submit-button" type="submit" value="Sign Up" />
        </div>
      </fieldset>
    <% } %>
  </div>
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Sign Up
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
