<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h5 class="error">Error</h5>

<% if (TempData.ContainsKey(GlobalViewDataProperty.PageErrorMessage)) {
     Html.RenderPartial("ErrorMessage");
   } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Error
</asp:Content>