<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ClubPool.Web.Views.AspxViewPageBase<ClubPool.Web.Controllers.User.ViewModels.SignUpViewModel>" %>
<%@ Import Namespace="ClubPool.Web.Controls.Captcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <strong>Sign up for a new Club Pool League account</strong>

  <p>Complete the form below to sign up for a new league account. A site administrator will
  need to verify your account information before you will be able to log in. You will receive
  an email once an administrator has verified your information.</p>
  
  <div class="formContainer corner">
    <div class="formTitle">Sign Up</div>
    <div class="formContent corner">
      <div class="formHeader">
        All fields are required
      </div>
      <% if (!string.IsNullOrEmpty(Model.ErrorMessage)) {
           Html.RenderPartial("FormError");
         } %>
      <form method="post" id="signupForm" class="signupForm normal" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.SignUp())%>">
        <fieldset>
          <%= Html.AntiForgeryToken() %>
          <%= Html.HiddenFor(m => m.PreviousUsername) %>
          <div class="formrow">
            <label for="Username" accesskey="u">Username:</label>
            <div class="formInput">
              <%= this.TextBox(m => m.Username) %>
              <%= Html.ValidationMessageFor(m => m.Username) %>
            </div>
          </div>
          <div class="formrow">
            <label for="Password" accesskey="p">Password:</label>
            <div class="formInput">
              <%= this.Password(m => m.Password) %>
              <%= Html.ValidationMessageFor(m => m.Password) %>
            </div>
          </div>
          <div class="formrow">
            <label for="ConfirmPassword" accesskey="c">Confirm password:</label>
            <div class="formInput">
              <%= this.Password(m => m.ConfirmPassword) %>
              <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %>
            </div>
          </div>
          <div class="formrow">
            <label for="Email" accesskey="e">Email address:</label>
            <div class="formInput">
              <%= this.TextBox(m => m.Email) %>
              <%= Html.ValidationMessageFor(m => m.Email) %>
            </div>
          </div>
          <div class="formrow">
            <label for="FirstName">Name:</label>
            <div class="formInput">
              <%= this.TextBox(m => m.FirstName).Class("short") %>
              <label for="FirstName">First</label>
              <%= Html.ValidationMessageFor(m => m.FirstName) %>
            </div>
            <div class="formInput">
              <%= this.TextBox(m => m.LastName).Class("short") %>
              <label for="LastName">Last</label>
              <%= Html.ValidationMessageFor(m => m.LastName) %>
            </div>
          </div>
          <div class="spacer">&nbsp;</div>
          <div class="formrow">
            <label><%= Html.CaptchaImage(50, 180) %></label>
            <div class="formInput">
              <label for="captcha">Enter the text from the image below:</label><br />
              <%= Html.CaptchaTextBox("captcha") %>
              <%= Html.ValidationMessage("captcha") %>
            </div>
          </div>
          <div class="spacer">&nbsp;</div>
          <div class="formrowspan">
            <button type="submit">Sign Up</button>
          </div>
        </fieldset>
      </form>
      <%= Html.ClientSideValidation<ClubPool.Web.Controllers.User.ViewModels.SignUpViewModel>() %>
    </div>
  </div>
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Sign Up
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
