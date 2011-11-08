<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("logoff.png", "Login") %>
    <span>Login</span>
  </div>
  <% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
       Html.RenderPartial("ErrorMessage");
     } %>
  <div>
    <div class="form-content login-form corner">
      <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Users.UsersController>(c => c.Login(string.Empty), 
            FormMethod.Post, new { @class = "normal", id = "loginForm" })) { %>
        <% Html.RenderPartial("LoginControl"); %>
      <% } %>
      <%= Html.ClientSideValidation<ClubPool.Web.Controllers.Users.ViewModels.LoginViewModel>().DisableMessages() %>
    </div>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">Login</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>