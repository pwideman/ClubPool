<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.ResetPasswordViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<strong>Reset Password</strong>
<p>Enter your username below to have a new temporary password sent to the email address registered with
the username. You may change the temporary password by going to My > Member Info after logging in.</p>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.ResetPassword(null), 
      FormMethod.Post)) { %>
  Username: <%= Html.TextBoxFor(m => m.Username) %> <%= Html.SubmitButton("submit", "Reset Password") %>
<% } %>
<%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>().DisableMessages() %>
<% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) { 
       Html.RenderPartial("ErrorMessage");
   } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
