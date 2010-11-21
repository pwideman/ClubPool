<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h4>I cannot access my account</h4>
<p>Choose one of the options below:</p>
<ul>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.ResetPassword(), "I forgot my password")%></li>
  <li><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.RecoverUsername(), "I forgot my username") %></li>
</ul>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Account Help
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
