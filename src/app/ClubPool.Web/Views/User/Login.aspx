<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.User.ViewModels.LoginViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <form method="post" class="loginForm normal" action="<%= Html.BuildUrlFromExpressionForAreas<ClubPool.Web.Controllers.UserController>(c => c.Login(string.Empty))%>">
    <div class="formTitle">Login</div>
  <% Html.RenderPartial("LoginControl"); %>
  </form>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Login
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
