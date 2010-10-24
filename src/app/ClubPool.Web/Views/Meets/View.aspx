<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Match Details</span>
  </div>
  <p>
    <strong><%= Model.Team1Name %></strong> vs. <strong><%= Model.Team2Name %></strong>, 
    scheduled for week <%= Model.ScheduledWeek %> (<%= Model.ScheduledDate.ToShortDateString() %>)
  </p>
  <table class="match-details">
    <thead>
      <tr>
        <th>Team</th>
        <th>Player</th>
        <th>Innings</th>
        <th>Defensive Shots</th>
        <th>Wins</th>
        <th>Winner</th>
        <th>Date and Time Played</th>
      </tr>
    </thead>
    <tbody>
      <% 
        var firstMatch = true;
        foreach (var match in Model.Matches) {
          if (!firstMatch) { %>
            <tr class="spacer-row"><td colspan="99"></td></tr>
          <% }
           var firstResult = true;
           foreach (var result in match.Results) { %>
            <tr>
              <td><%= result.TeamName%></td>
              <td><%= result.PlayerName%></td>
              <% if (match.IsComplete) { %>
              <td><%= result.Innings.ToString()%></td>
              <td><%= result.DefensiveShots.ToString() %></td>
              <td><%= result.Wins.ToString() %></td>
              <td><%= result.Winner.ToString() %></td>
              <td>
              <% if (firstResult) {
                   firstResult = false; %>
                <%= match.DatePlayed%>
              <% } %>
              </td>
              <% } else { %>
              <td></td>
              <td></td>
              <td></td>
              <td></td>
              <td></td>
              <% } %>
            </tr>
          <% }
            firstMatch = false; %>
      <% } %>
    </tbody>
  </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
