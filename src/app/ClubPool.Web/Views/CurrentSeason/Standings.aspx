<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.CurrentSeason.ViewModels.CurrentSeasonStandingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h4><%= Html.Encode(Model.Name) %> Standings</h4>

<% if (!Model.HasDivisions) { %>
<p>This season has no divisions.</p>
<% } else { %>
<div id="divisiontabs">
  <ul>
  <% foreach (var division in Model.Divisions) { %>
    <li><a href="#division-<%= division.Id%>"><%= Html.Encode(division.Name)%></a></li>
  <% } %>
  </ul>
  <% foreach (var division in Model.Divisions) {
       var divisionPrefix = "division-" + division.Id.ToString(); %>
  <div id="<%= divisionPrefix %>">
    <div id="<%= divisionPrefix %>-tabs" class="division-standings-tabs">
      <ul>
        <li><a href="#<%= divisionPrefix%>-teams">Teams</a></li>
        <li><a href="#<%= divisionPrefix%>-players">Players</a></li>
      </ul>

      <div id="<%= divisionPrefix%>-teams">
      <% if (division.HasTeams) { %>
        <table class="standings-table team-standings-table">
          <thead>
            <tr>
              <th>Rank</th>
              <th>Team</th>
              <th>Record</th>
              <th>Win %</th>
              <th colspan="8">Players</th>
            </tr>
          </thead>
          <tbody>
          <% foreach (var team in division.Teams) { %>
            <tr <%= team.Highlight ? @"class=""highlight""" : "" %>>
              <td><%= team.Rank %></td>
              <td><%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(team.Id), team.Name) %></td>
              <td><%= string.Format("{0} - {1}", team.Wins, team.Losses) %></td>
              <td><%= string.Format("{0:0.00}", team.WinPercentage) %></td>
              <% Html.RenderPartial("StandingsPlayer", team.Player1); %>
              <% Html.RenderPartial("StandingsPlayer", team.Player2); %>
            </tr>
          <% } %>
          </tbody>
        </table>
      <% } else { %>
        <p>This division has no teams.</p>
      <% } %>
      </div>

      <div id="<%= divisionPrefix%>-players">
      <% if (division.HasPlayers) { %>
        <table class="standings-table player-standings-table">
          <thead>
            <tr>
              <th>Rank</th>
              <th>Name</th>
              <th>Skill Level</th>
              <th>Record</th>
              <th>Win %</th>
            </tr>
          </thead>
          <tbody>
          <% foreach (var player in division.Players) { %>
            <tr <%= player.Highlight ? @"class=""highlight""" : "" %>>
              <td><%= player.Rank %></td>
              <% Html.RenderPartial("StandingsPlayer", player); %>
            </tr>
          <% } %>
          </tbody>
        </table>
      <% } else { %>
        <p>This division has no players.</p>
      <% } %>
      </div>
    </div>
  </div>
  <% } %>
</div>
<% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Standings
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(function () {
    $("#divisiontabs").tabs();
    $(".division-standings-tabs").tabs();
    $("table tbody tr:odd").addClass("alt");
  });
</script>
</asp:Content>
