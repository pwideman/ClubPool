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
            <button type="submit">Sign Up</button>
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
