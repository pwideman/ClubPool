<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("home.png", "Home") %>
    <span><%= Html.Encode(Model.UserFullName) %>'s Dashboard</span>
  </div>
  <div class="dashboard">
    <div class="dashboard-item-container dashboard-column-left">
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
                <td><%= result.Innings %></td>
                <td><%= result.DefensiveShots %></td>
                <td><%= result.Wins %></td>
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
    <div class="dashboard-item-container dashboard-column-right">
      <% if (Model.HasCurrentSeasonStats) { %>
      <div class="dashboard-item">
        <div class="dashboard-item-header">Current Season Stats & Info</div>
        <div class="dashboard-item-content">
          <ul>
            <li>Skill Level: <%= Model.CurrentSeasonStats.SkillLevel %></li>
            <li>My Record: <%= Model.CurrentSeasonStats.PersonalRecord %></li>
            <li>Team Name: <%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(Model.CurrentSeasonStats.TeamId), Model.CurrentSeasonStats.TeamName) %></li>
            <li>Teammate: <%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.View(Model.CurrentSeasonStats.TeammateId), Model.CurrentSeasonStats.TeammateName) %></li>
            <li>Team Record: <%= Model.CurrentSeasonStats.TeamRecord %></li>
          </ul>
        </div>
      </div>
      <% } %>
      <% if (Model.SkillLevelCalculation.HasSkillLevel) { %>
      <div class="dashboard-item">
        <div class="dashboard-item-header">Skill Level Calculation Statistics</div>
        <div class="dashboard-item-content">
          <table id="skill_level_calc" class="results">
            <thead>
              <tr>
                <th>Date</th>
                <th>Team</th>
                <th>Player</th>
                <th>I</th>
                <th>D</th>
                <th>Net</th>
                <th>W</th>
              </tr>
            </thead>
            <tbody class="skill_level_matches">
            <% foreach (var result in Model.SkillLevelCalculation.SkillLevelMatchResults) { %>
              <tr <%= result.Included ? @"class=""included""" : "" %>>
                <td><%= string.Format("{0} {1}", result.Date.ToShortDateString(), result.Date.ToShortTimeString()) %></td>
                <td class="team"><%= Html.Encode(result.Team) %></td>
                <td class="player"><%= Html.Encode(result.Player) %></td>
                <td><%= result.Innings %></td>
                <td><%= result.DefensiveShots %></td>
                <td><%= result.NetInnings %></td>
                <td><%= result.Wins %></td>
              </tr>
            <% } %>
            </tbody>
            <tbody class="skill_level_totals">
              <tr>
                <td colspan="2">Totals</td>
                <td><%= string.Format("I/G: {0:.000}", Model.SkillLevelCalculation.TotalIG) %></td>
                <td><%= Model.SkillLevelCalculation.TotalInnings %></td>
                <td><%= Model.SkillLevelCalculation.TotalDefensiveShots %></td>
                <td><%= Model.SkillLevelCalculation.TotalNetInnings %></td>
                <td><%= Model.SkillLevelCalculation.TotalWins %></td>
              </tr>
              <tr>
                <td colspan="2">Culled Totals</td>
                <td><%= string.Format("I/G: {0:.000}", Model.SkillLevelCalculation.CulledIG) %></td>
                <td><%= Model.SkillLevelCalculation.TotalCulledInnings %></td>
                <td><%= Model.SkillLevelCalculation.TotalCulledDefensiveShots%></td>
                <td><%= Model.SkillLevelCalculation.TotalCulledNetInnings%></td>
                <td><%= Model.SkillLevelCalculation.TotalCulledWins%></td>
              </tr>
              <tr>
                <td colspan="7">Skill Level: <%= Model.SkillLevelCalculation.EightBallSkillLevel%></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <% } %>
    </div>
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(document).ready(function () {
    $("#lastmeet tbody tr:odd:not(:last)").addClass("match");
    $("#lastmeet tbody tr:even").addClass("player");
    $("#season_results tr:last").addClass("last");
    $("#skill_level_calc tr:last").addClass("last");
  });
</script>
</asp:Content>
