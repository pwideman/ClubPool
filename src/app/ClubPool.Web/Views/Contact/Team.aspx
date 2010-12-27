<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Contact.ViewModels.TeamViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h4>Send email to <%= Model.TeamName %></h4>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Contact.ContactController>(c => c.Team(null), FormMethod.Post)) { %>
<%= Html.HiddenFor(m => m.TeamId) %>
<%= Html.AntiForgeryToken() %>
<% Html.RenderPartial("ContactView"); %>
<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
