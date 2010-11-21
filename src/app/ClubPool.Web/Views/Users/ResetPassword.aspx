<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.ResetPasswordViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h5>Reset Password</h5>
<p>Enter your username below to have a new temporary password sent to the email address registered with
the username. You may change the temporary password by going to My > Member Info after logging in.</p>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.ResetPassword(null), 
      FormMethod.Post)) { %>
  Username: <%= Html.TextBoxFor(m => m.Username, new { @class = "username" })%> <%= Html.SubmitButton("submit", "Reset Password") %>
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
