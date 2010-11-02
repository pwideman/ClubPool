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
    <div id="incomplete_matches">
      <% foreach (var match in Model.IncompleteMatches) { %>
      <div id="incompletematch<%= match.Id.ToString() %>" class="incomplete-match">
        <% Html.RenderPartial("PlayerCard", match.Player1); %>
        vs.
        <% Html.RenderPartial("PlayerCard", match.Player2); %>
        <div class="action-button-column">
          <div class="action-button enter-results-link" id="<%= match.Id %>">
            <%= Html.ContentImage("enterresults-medium.png", "Enter results") %>
            Enter Results
          </div>
        </div>
        <div class="status" id="incompletematch<%= match.Id.ToString() %>_status">
        </div>
      </div>
      <% } %>
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
    <%= Html.AntiForgeryToken() %>
    <input type="hidden" name="Id" id="match_id" />
    <input type="hidden" name="Player1.Id" id="player1_id" />
    <input type="hidden" name="Player2.Id" id="player2_id" />
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
        <tr class="first">
          <td class="name" id="player1name"></td>
          <td><input type="text" name="Player1.Innings" class="integer-input"/></td>
          <td><input type="text" name="Player1.DefensiveShots" class="integer-input"/></td>
          <td><input type="text" name="Player1.Wins" class="integer-input"/></td>
          <td><input type="radio" name="Winner" id="player1Winner" /></td>
        </tr>
        <tr>
          <td class="name" id="player2name"></td>
          <td><input type="text" name="Player2.Innings" class="integer-input"/></td>
          <td><input type="text" name="Player2.DefensiveShots" class="integer-input"/></td>
          <td><input type="text" name="Player2.Wins" class="integer-input"/></td>
          <td><input type="radio" name="Winner" id="player2Winner" /></td>
        </tr>
      </tbody>
    </table>
    <% } %>
  </div>
  <div id="enter_results_waiting_indicator">
    <%= Html.ContentImage("loading.gif", "Loading") %>
    Please wait...
  </div>
  <script type="text/javascript">
    // initialize some variables needed to enter match results
    var $matches = {};
    <% foreach(var match in Model.IncompleteMatches) { %>
    $matches["<%= match.Id %>"] = {
      id: <%= match.Id %>,
      player1Name: "<%= match.Player1.Name %>",
      player1Id: <%= match.Player1.Id.ToString() %>,
      player2Name: "<%= match.Player2.Name %>",
      player2Id: <%= match.Player2.Id.ToString() %>
    };
    <% } %>
    var $current_match_id = null;
    var $current_match_status = null;
    var $current_match_row = null;

    $(document).ready(function () {
      // create tabs
      $("#matches_tabs").tabs();
      // create ajax form
      $("#enter_results_form").ajaxForm(function(response, status, xhr, form) {
        $current_match_status.html("");
        $log("response: ", response);
        $log("status: ", status);
        $log("xhr: ", xhr);
        $log("form: ", form);
        if (xhr.status === 200) {
          // update was successful, remove match from table
          var id = form.find("input[name='Id']").val();
          $current_match_row.effect("highlight", "slow", function() {
            $(this).slideUp(500);
          });
        }
        else {
          // TODO: error, what to do?
        }
      });

      // set up enter results modal dialog
      var $enter_results_dialog = $("#enter_results_window").dialog({
        autoOpen: false,
        title: "Enter Match Results",
        buttons: {
          "OK": function () {
            $(this).dialog("close");
            $current_match_status.html('<%= Html.ContentImage("loading.gif", "Loading") %>&nbsp;Please wait...');
            $("#enter_results_form").submit();
          },
          "Cancel": function () {
            $(this).dialog("close");
          }
        },
        resizable: false,
        width: 500,
        modal: true
      });

      // add click event handler to enter results image links
      $(".enter-results-link").click(function () {
        var match = $matches[this.id];
        $current_match_id = this.id;
        $current_match_row = $("#incompletematch" + $current_match_id);
        $current_match_status = $("#incompletematch" + $current_match_id + "_status");
        $("#match_id").val(this.id);
        $("#player1name").text(match.player1Name);
        $("#player2name").text(match.player2Name);
        $("#player1Winner, #player1_id").val(match.player1Id);
        $("#player2Winner, #player2_id").val(match.player2Id);
        $("#enter_results_form").clearForm();
        $enter_results_dialog.dialog("open");
      });

      // set input.integer-input text boxes to accept numeric input only
      $(".integer-input").numeric();
    });
  </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <%= Html.ScriptInclude("jquery.alphanumeric.js") %>
  <%= Html.ScriptInclude("jquery.form.js") %>
</asp:Content>
