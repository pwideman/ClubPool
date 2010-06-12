<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.SignUpViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controls.Captcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <strong>Sign up for a new Club Pool League account</strong>

  <p>Complete the form below to sign up for a new league account. A site administrator will
  need to verify your account information before you will be able to log in. You will receive
  an email once an administrator has verified your information.</p>
  
  <div class="formContainer">
    <div class="formTitle">Sign Up</div>
    <div class="formContent">
      <div class="formHeader">
        All fields are required
      </div>
      <% if (!string.IsNullOrEmpty(Model.ErrorMessage)) {
           Html.RenderPartial("FormError");
         } %>
      <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.SignUp(), FormMethod.Post, new { @class = "signupForm normal" })) { %>
        <fieldset>
          <%= Html.AntiForgeryToken()%>
          <%= Html.HiddenFor(m => m.PreviousUsername)%>
          <% Html.RenderPartial("CreateUserControl"); %>
          <div class="spacer">&nbsp;</div>
          <div class="formrow">
            <label><%= Html.CaptchaImage(50, 180)%></label>
            <div class="formInput">
              <label for="captcha">Enter the text from the image below:</label><br />
              <%= Html.CaptchaTextBox("captcha")%>
              <%= Html.ValidationMessage("captcha")%>
            </div>
          </div>
          <div class="spacer">&nbsp;</div>
          <div class="formrowspan">
            <input class="submit-button" type="submit" value="Sign Up" />
          </div>
        </fieldset>
      <% } %>
      <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.SignUpViewModel>() %>
    </div>
  </div>
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Sign Up
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
