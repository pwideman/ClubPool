<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<!DOCTYPE html>

<html>
  <head>
    <title>Scoresheet</title>
    <%= Html.Stylesheet("scoresheet.less") %>
  </head>
  <body>
    <div class="header">
      <%= Model.Team1Name %> vs. <%= Model.Team2Name %>
    </div>
    <div class="scoresheet">
      <table class="scoresheet">
        <thead>
          <tr>
            <th>Match</th>
            <th>Team</th>
            <th>Player</th>
            <th>SL</th>
            <th>GTW</th>
            <th>Innings</th>
            <th>Defensive Shots</th>
            <th>Wins</th>
          </tr>
        </thead>
        <tbody>
          <% 
            var matchIndex = 0;
            foreach (var match in Model.Matches) {
              if (matchIndex > 0) { %>
                <tr class="spacer-row"><td colspan="99">&nbsp;</td></tr>
           <% }
                var firstResult = true;
                matchIndex++;
                foreach (var result in match.Results) { %>
                <tr>
                  <td>
                  <% if (firstResult) { %>
                  <%= matchIndex.ToString() %>
                  <% } %>
                  </td>
                  <td class="name-column"><%= result.TeamName %></td>
                  <td class="name-column"><%= result.PlayerName %></td>
                  <td>SL</td>
                  <td>GTW</td>
                  <% if (match.IsComplete) { %>
                  <td><%= result.Innings.ToString() %></td>
                  <td><%= result.DefensiveShots.ToString() %></td>
                  <td><%= result.Wins.ToString() %></td>
                  <% } else { %>
                  <td class="innings-column"><hr/></td>
                  <td class="defshots-column"><hr/></td>
                  <td class="wins-column"></td>
                  <% } %>
                </tr>
            <%    firstResult = false;
                } 
            } %>
        </tbody>
      </table>
    </div>
  </body>
</html>
