<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<strong>I cannot access my account</strong>
<p>We're sorry for the inconvenience. Choose one of the options below:</p>
<ul>
  <li><%= Html.ActionLink<UsersController>(c => c.ResetPassword(), "I forgot my password") %></li>
</ul>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
I cannot access my account
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
