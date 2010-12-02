<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("home.png", "Home") %>
    <span><%= Model.UserFullName %>'s Dashboard</span>
  </div>
  <div class="dashboard-item-container" style="width: 40%;">
    <% if (Model.HasLastMeetStats) { %>
    <div class="dashboard-item">
      <div class="dashboard-item-header">Last Match Results vs <%= Html.Encode(Model.LastMeetStats.OpponentTeam) %></div>
      <div class="dashboard-item-content">
        <table id="lastmeet" class="results">
          <thead>
            <tr>
              <th>Player</th>
              <th>Innings</th>
              <th>Defensive Shots</th>
              <th>Wins</th>
            </tr>
          </thead>
          <tbody>
          <% foreach (var match in Model.LastMeetStats.Matches) {
               foreach(var result in match.Results) { %>
            <tr <%= result.Winner ? @"class=""winner""" : "" %>>
              <td class="player"><%= Html.Encode(result.Player) %></td>
              <td><%= Html.Encode(result.Innings) %></td>
              <td><%= Html.Encode(result.DefensiveShots) %></td>
              <td><%= Html.Encode(result.Wins) %></td>
            </tr>
          <%  } } %>
          </tbody>
        </table>
      </div>
    </div>
    <% } %>
    <% if (Model.HasSeasonResults) { %>
    <div class="dashboard-item">
      <div class="dashboard-item-header">Season Results</div>
      <div class="dashboard-item-content">
        <table id="season_results" class="results">
          <thead>
            <tr>
              <th>Team</th>
              <th colspan="2">Player</th>
            </tr>
          </thead>
          <tbody>
          <% foreach (var result in Model.SeasonResults) { %>
            <tr <%= result.Win ? @"class=""winner""" : "" %>>
              <td class="team"><%= Html.Encode(result.Team) %></td>
              <td class="player"><%= Html.Encode(result.Player) %></td>
              <td class="win"><%= result.Win ? "W" : "L" %></td>
            </tr>
          <% } %>
          </tbody>
        </table>
      </div>
    </div>
    <% } %>
  </div>
  <div class="dashboard-item-container" style="width: 55%;">
    <% if (Model.HasCurrentSeasonStats) { %>
    <div class="dashboard-item">
      <div class="dashboard-item-header">Current Season Stats & Info</div>
      <div class="dashboard-item-content">
        <ul>
          <li>Skill Level: <%= Model.CurrentSeasonStats.SkillLevel %></li>
          <li>My Record: <%= Model.CurrentSeasonStats.PersonalRecord %></li>
          <li>Team Name: <%= Model.CurrentSeasonStats.TeamName %></li>
          <li>Teammate: <%= Model.CurrentSeasonStats.Teammate %></li>
          <li>Team Record: <%= Model.CurrentSeasonStats.TeamRecord %></li>
        </ul>
      </div>
    </div>
    <% } %>
    <% if (Model.SkillLevelCalculation.HasSkillLevel) { %>
    <div class="dashboard-item">
      <div class="dashboard-item-header">Skill Level Calculation Statistics</div>
      <div class="dashboard-item-content">
        <table id="skilllevelcalc" class="results">
          <thead>
            <tr>
              <th>Team</th>
              <th>Player</th>
              <th>Innings</th>
              <th>Defensive Shots</th>
              <th>Net Innings</th>
              <th>Wins</th>
            </tr>
          </thead>
          <tbody>
          <% foreach (var result in Model.SkillLevelCalculation.SkillLevelMatchResults) { %>
            <tr <%= result.Included ? @"class=""included""" : "" %>>
              <td class="team"><%= Html.Encode(result.Team) %></td>
              <td class="player"><%= Html.Encode(result.Player) %></td>
              <td><%= Html.Encode(result.Innings) %></td>
              <td><%= Html.Encode(result.DefensiveShots) %></td>
              <td><%= Html.Encode(result.NetInnings) %></td>
              <td><%= Html.Encode(result.Wins) %></td>
            </tr>
          <% } %>
          </tbody>
        </table>
      </div>
    </div>
    <% } %>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(document).ready(function () {
    $("table#lastmeet tbody tr:odd:not(:last)").addClass("match");
    $("table#lastmeet tbody tr:even").addClass("player");
  });
</script>
</asp:Content>
