<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Seasons.ViewModels.ChangeActiveViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="heading">
  <span>Change the Active Season</span>
</div>

<% if (!string.IsNullOrEmpty(Model.CurrentActiveSeasonName)) { %>
<p>The current active season is: <strong><%= Model.CurrentActiveSeasonName%></strong></p>
<% } else { %>
<p>There is no active season.</p>
<% } %>

<% if (Model.InactiveSeasons.Any()) { %>
<p>
<% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Seasons.SeasonsController>(c => c.ChangeActive(), FormMethod.Post, new { @class = "normal" })) { %>
  <%= Html.AntiForgeryToken()%>
  Select a new active season:
  <select name="id">
  <% foreach (var s in Model.InactiveSeasons) { %>
    <option value="<%= s.Id %>"><%= s.Name %></option>
  <% } %>
  </select>
  <input class="submit-button" type="submit" value="Change" />
<% } %>
</p>
<% } else { %>
<p>There are no inactive seasons to make active.</p>
<% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Change Active Season - ClubPool
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
