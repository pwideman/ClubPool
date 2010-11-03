<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.ScoresheetViewModel>" %>
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
                matchIndex++; %>
                <tr>
                  <td><%= matchIndex.ToString() %></td>
                  <td class="name-column"><%= match.Player1.TeamName%></td>
                  <td class="name-column"><%= match.Player1.Name%></td>
                  <td><%= match.Player1.SkillLevel > 0 ? match.Player1.SkillLevel.ToString() : "N/A" %></td>
                  <td><%= match.Player1.GamesToWin.ToString() %></td>
                  <td class="innings-column"><hr/></td>
                  <td class="defshots-column"><hr/></td>
                  <td class="wins-column"></td>
                </tr>
                <tr>
                  <td></td>
                  <td class="name-column"><%= match.Player2.TeamName%></td>
                  <td class="name-column"><%= match.Player2.Name%></td>
                  <td><%= match.Player2.SkillLevel > 0 ? match.Player2.SkillLevel.ToString() : "N/A" %></td>
                  <td><%= match.Player2.GamesToWin.ToString() %></td>
                  <td class="innings-column"><hr/></td>
                  <td class="defshots-column"><hr/></td>
                  <td class="wins-column"></td>
                </tr>
          <% } %>
        </tbody>
      </table>
    </div>
  </body>
</html>
