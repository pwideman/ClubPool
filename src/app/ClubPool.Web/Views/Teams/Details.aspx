<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Teams.ViewModels.DetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h4><%= Html.Encode(Model.Name) %></h4>
Details:
<ul>
  <li>
    Players:
    <ul>
      <% foreach (var player in Model.Players) { %>
      <li><%= string.Format("{0} ({1})", Html.Encode(player.Name), Html.Encode(player.EightBallSkillLevel)) %></li>
      <% } %>
    </ul>
  </li>
  <li>Record: <%= Html.Encode(Model.Record) %></li>
</ul>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
