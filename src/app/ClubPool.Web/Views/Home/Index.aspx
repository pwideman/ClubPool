<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Home.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <h3>
    Welcome to the Club Pool League Website</h3>
  <h4>
    About the league</h4>
  <p>
    About...
    <%= Html.ActionLink<ClubPool.Web.Controllers.User.UserController>(c => c.Logout(), "login") %>
  </p>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">ClubPool Home</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>