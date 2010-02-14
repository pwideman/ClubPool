<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ClubPool.Web.Views.AspxViewPageBase<ClubPool.Web.Controllers.User.ViewModels.SignUpViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <strong>Sign up for a new Club Pool League account</strong>

  <p>Complete the form below to sign up for a new league account. A site administrator will
  need to verify your account information before you will be able to log in. You will receive
  an email once an administrator has verified your information.</p>
  
  <form method="post" id="signupForm" class="signupForm normal normalRoundCorners" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.SignUp())%>">
    <div class="formTitle">Sign Up</div>
    <div class="formContent normalRoundCorners">
      <fieldset class="normalRoundCorners">
        <% if (!ViewData.ModelState.IsValid) { %>
        <div class="formValidationSummary normalRoundCorners">
          <%= Html.ValidationSummary("There are problems with the information you provided:") %>
        </div>
        <% } %>
        <%= Html.AntiForgeryToken() %>
        <%= this.Hidden(m => m.PreviousUsername) %>
        <div class="formrow">
          <%= this.TextBox(m => m.Username).Label("Username:") %>
        </div>
        <div class="formrow">
          <%= this.TextBox(m => m.Password).Label("Password:") %>
        </div>
        <div class="formrow">
          <%= this.TextBox(m => m.ConfirmPassword).Label("Confirm password:") %>
        </div>
        <div class="formrow">
          <%= this.TextBox(m => m.Email).Label("Email:") %>
        </div>
        <div class="formrow">
          <%= this.TextBox(m => m.FirstName).Label("First name:") %>
        </div>
        <div class="formrow">
          <%= this.TextBox(m => m.LastName).Label("Last name:") %>
        </div>
        <div class="spacer">&nbsp;</div>
        <div class="formrowspan">
          <button type="submit">Sign Up</button>
        </div>
      </fieldset>
    </div>
  </form>
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Sign Up
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
