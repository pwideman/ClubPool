<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.DetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<p><input type="text" id="name" name="name" class="team-name" value="<%= Html.Encode(Model.Name) %>"/></p>
Details:
<ul>
  <li>
    Players:
    <ul>
      <% foreach (var player in Model.Players) { %>
      <li><%= Html.Encode(string.Format("{0} ({1})", player.Name, player.EightBallSkillLevel)) %></li>
      <% } %>
    </ul>
  </li>
  <li>Record: <%= Html.Encode(Model.Record) %></li>
  <li>Division Ranking: <%= Html.Encode(Model.Rank) %></li>
</ul>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Team Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
