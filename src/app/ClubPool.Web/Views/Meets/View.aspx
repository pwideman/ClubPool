<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

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
            <th/>
          </tr>
        </thead>
        <tbody>
          <% 
        var matchIndex = 1;
        foreach (var match in Model.IncompleteMatches) { %>
          <% if (matchIndex > 1) { %>
            <tr class="spacer-row"><td colspan="99">&nbsp;</td></tr>
          <% } %>
            <tr class="first">
              <td><%= matchIndex.ToString()%></td>
              <td><%= match.Player1.TeamName%></td>
              <td><%= match.Player1.Name%></td>
              <td><%= match.Player1.SkillLevel > 0 ? match.Player1.SkillLevel.ToString() : "None" %></td>
              <td><%= match.Player1.Wins.ToString()%> - <%= match.Player1.Losses.ToString()%> (<%= match.Player1.WinPercentage.ToString(".00")%>)</td>
              <td>
                <%= Html.ContentImage("enterresults-medium.png", "Enter results", new { @class = "enter-results-link", id = match.Id })%>
              </td>
            </tr>
            <tr>
              <td></td>
              <td><%= match.Player2.TeamName%></td>
              <td><%= match.Player2.Name%></td>
              <td><%= match.Player2.SkillLevel > 0 ? match.Player2.SkillLevel.ToString() : "None" %></td>
              <td><%= match.Player2.Wins.ToString()%> - <%= match.Player2.Losses.ToString()%> (<%= match.Player2.WinPercentage.ToString(".00")%>)</td>
              <td></td>
            </tr>
        <%  matchIndex++; 
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
  <div id="enter_results_window">
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Matches.MatchesController>(c => c.Edit(null), FormMethod.Post, new { id = "enter_results_form" })) { %>
    <input type="hidden" name="id"/>
    <table>
      <thead>
        <tr>
          <th>Player</th>
          <th>Innings</th>
          <th>Defensive Shots</th>
          <th>Wins</th>
          <th>Winner</th>
        </tr>
      </thead>
      <tbody>
        <tr id="player1">
          <td class="name" id="player1name"></td>
          <td class="innings"><input type="text" name="player1_innings" class="integer-input"/></td>
          <td class="defshots"><input type="text" name="player1_defshots" class="integer-input"/></td>
          <td class="wins"><input type="text" name="player1_wins" class="integer-input"/></td>
          <td class="winner"><input type="radio" name="winner"/></td>
        </tr>
        <tr id="player2">
          <td class="name" id="player2name"></td>
          <td class="innings"><input type="text" name="player2_innings" class="integer-input"/></td>
          <td class="defshots"><input type="text" name="player2_defshots" class="integer-input"/></td>
          <td class="wins"><input type="text" name="player2_wins" class="integer-input"/></td>
          <td class="winner"><input type="radio" name="winner"/></td>
        </tr>
      </tbody>
    </table>
    <% } %>
  </div>
  <script type="text/javascript">
    var $matches = {};
    <% foreach(var match in Model.IncompleteMatches) { %>
    $matches["<%= match.Id %>"] = {
      player1Name: "<%= match.Player1.Name %>",
      player2Name: "<%= match.Player2.Name %>"
    };
    <% } %>
    var $current_match = null;

    $(document).ready(function () {
      $("#matches_tabs").tabs();
      var $enter_results_dialog = $("#enter_results_window").dialog({
        autoOpen: false,
        title: "Enter Match Results",
        buttons: {
          "OK": function () {
            $("#enter_results_form").submit();
            $(this).dialog("close");
          },
          "Cancel": function () {
            $(this).dialog("close");
          }
        },
        width: 500,
        modal: true
      });
      $(".enter-results-link").click(function () {
        var match = $matches[this.id];
        $current_match = this.id;
        $("#id").val(this.id);
        $("#player1name").text(match.player1Name);
        $("#player2name").text(match.player2Name);
        $enter_results_dialog.dialog("open");
      });
      $(".integer-input").numeric();
    });
  </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <%= Html.ScriptInclude("jquery.alphanumeric.js") %>
</asp:Content>
