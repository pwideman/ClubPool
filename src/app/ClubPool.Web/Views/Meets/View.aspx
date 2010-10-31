﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Match Details</span>
  </div>
  <p>
    <strong><%= Model.Team1Name %></strong> vs. <strong><%= Model.Team2Name %></strong>, 
    scheduled for week <%= Model.ScheduledWeek %> (<%= Model.ScheduledDate.ToShortDateString() %>)
  </p>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("printer-medium.png", "Print a soresheet") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Meets.MeetsController>(u => u.Scoresheet(Model.Id), "Print a scoresheet") %>
    </div>
  </div>
  <div id="matches_tabs">
    <ul>
      <% if (Model.IncompleteMatches.Any()) { %>
      <li><a href="#incomplete_matches">Incomplete Matches</a></li>
      <% }
        if (Model.CompletedMatches.Any()) { %>
      <li><a href="#complete_matches">Complete Matches</a></li>
      <% } %>
    </ul>
    <% if (Model.IncompleteMatches.Any()) { %>
    <div id="#incomplete_matches">
      <table class="incomplete-match-details">
        <thead>
          <tr>
            <th>Match</th>
            <th>Team</th>
            <th>Player</th>
            <th>Skill Level</th>
            <th>Record</th>
            <th>Ranking</th>
            <th/>
          </tr>
        </thead>
        <tbody>
          <% 
        var matchIndex = 0;
        foreach (var match in Model.IncompleteMatches) {
          var firstResult = true;
          matchIndex++;
          foreach (var player in match.Players) { %>
            <tr>
              <td>
              <% if (firstResult) { %>
              <%= matchIndex.ToString()%>
              <% } %>
              </td>
              <td><%= player.TeamName%></td>
              <td><%= player.Name%></td>
              <td><%= player.SkillLevel > 0 ? player.SkillLevel.ToString() : "None" %></td>
              <td><%= player.Wins.ToString()%> - <%= player.Losses.ToString()%> (<%= player.WinPercentage.ToString(".00")%>)</td>
              <td><%= player.Ranking.ToString()%></td>
              <td>
              <% if (firstResult) { %>
              commands
              <% } %>
              </td>
            </tr>
        <%    firstResult = false;
          }
        } %>
        </tbody>
      </table>
    </div>
  <% } %>
  <% if (Model.CompletedMatches.Any()) { %>
    <div id="#complete_matches">
      <table class="match-details" cellpadding="0" cellspacing="0">
        <thead>
          <tr>
            <th>Match</th>
            <th>Team</th>
            <th>Player</th>
            <th>Innings</th>
            <th>Defensive Shots</th>
            <th>Wins</th>
            <th>Winner</th>
            <th>Date and Time Played</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <% 
            var matchIndex = 0;
            foreach (var match in Model.CompletedMatches) {
                var firstResult = true;
                matchIndex++;
                foreach (var result in match.Results) { %>
                <tr>
                  <td>
                  <% if (firstResult) { %>
                  <%= matchIndex.ToString() %>
                  <% } %>
                  </td>
                  <td><%= result.TeamName%></td>
                  <td><%= result.PlayerName%></td>
                  <td><%= result.Innings.ToString()%></td>
                  <td><%= result.DefensiveShots.ToString() %></td>
                  <td><%= result.Wins.ToString() %></td>
                  <td><%= result.Winner.ToString() %></td>
                  <td>
                  <% if (firstResult) { %>
                    <%= match.DatePlayed%>
                  <% } %>
                  </td>
                  <td>
                  <% if (firstResult) { %>
                  commands
                  <% } %>
                  </td>
                </tr>
            <%    firstResult = false;
                }
          } %>
        </tbody>
      </table>
    </div>
    <% } %>
  </div>
  <script type="text/javascript">
    $(document).ready(function () {
      $("#matches_tabs").tabs();
    });
  </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
