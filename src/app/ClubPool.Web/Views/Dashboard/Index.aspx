<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Dashboard.ViewModels.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <%= Html.ContentImage("home.png", "Home") %>
    <span><%= Model.UserFullName %>'s Dashboard</span>
  </div>
  <div class="dashboard-items">
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
    <% if (Model.HasLastMeetStats) { %>
    <div class="dashboard-item">
      <div class="dashboard-item-header">Last Match Results</div>
      <div class="dashboard-item-content">
        <p>Your last match was against <%= Html.Encode(Model.LastMeetStats.OpponentTeam) %></p>
        <table class="lastmeet">
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
  </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
<script type="text/javascript">
  $(document).ready(function () {
    $("table.lastmeet tbody tr:odd:not(:last)").addClass("match");
    $("table.lastmeet tbody tr:even").addClass("player");
  });
</script>
</asp:Content>
