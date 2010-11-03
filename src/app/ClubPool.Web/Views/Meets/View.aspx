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
         <%-- <div class="action-button-column">
          <div class="action-button enter-results-link" id="<%= match.Id %>">
            <%= Html.ContentImage("enterresults-medium.png", "Enter results") %>
            Enter Results
          </div>
        </div>
        <div class="status" id="incompletematch<%= match.Id.ToString() %>_status">
        </div>--%>

  <table id="match_details" class="match-details" cellpadding="0" cellspacing="0">
    <thead>
      <tr>
        <th>Match</th>
        <th>Player</th>
        <th>Innings</th>
        <th>Defensive Shots</th>
        <th>Wins</th>
        <th>Winner</th>
        <th>Date Played</th>
        <th></th>
      </tr>
    </thead>
    <tbody>
      <% 
        var matchIndex = 0;
        foreach (var match in Model.Matches) {
          matchIndex++;
          var firstWinnerClass = "";
          var secondWinnerClass = "";
          if (match.IsComplete) {
            if (match.Player1.Result.Winner) {
              firstWinnerClass = " winner";
            }
            else {
              secondWinnerClass = " winner";
            }
          }
           %>
          <tr class="first" id="<%= match.Id%>_1">
            <td><%= matchIndex.ToString() %></td>
            <td><%= match.Player1.Name%></td>
            <% if (match.IsComplete) { %>
            <td><%= match.Player1.Result.Innings.ToString()%></td>
            <td><%= match.Player1.Result.DefensiveShots.ToString()%></td>
            <td><%= match.Player1.Result.Wins.ToString()%></td>
            <td><%= match.Player1.Result.Winner ? Html.ContentImage("check-medium.png", "Winner") : MvcHtmlString.Empty%></td>
            <td><%= match.DatePlayed%></td>
            <% } else { %>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td>incomplete</td>
            <% } %>
            <td>
              <div class="action-button-row-small">
                <div class="action-button enter-results-link" id="<%= match.Id %>">
                  <%= Html.ContentImage("enterresults-medium.png", "Enter results") %>
                  Enter Results
                </div>
              </div>
            </td>
          </tr>
          <tr class="second" id="<%= match.Id%>_2">
            <td></td>
            <td><%= match.Player2.Name%></td>
            <% if (match.IsComplete) { %>
            <td><%= match.Player2.Result.Innings.ToString()%></td>
            <td><%= match.Player2.Result.DefensiveShots.ToString()%></td>
            <td><%= match.Player2.Result.Wins.ToString()%></td>
            <td><%= match.Player2.Result.Winner ? Html.ContentImage("check-medium.png", "Winner") : MvcHtmlString.Empty%></td>
            <% } else { %>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <% } %>
            <td></td>
            <td></td>
          </tr>
        <% } %>
    </tbody>
  </table>

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
  <script type="text/javascript">
    // initialize some variables needed to enter match results
    var $matches = {};
    <% foreach(var match in Model.Matches) { %>
    $matches["<%= match.Id %>"] = {
      id: <%= match.Id %>,
      isComplete: <%= match.IsComplete.ToString().ToLower() %>,
      player1: {
        name: "<%= match.Player1.Name %>",
        id: <%= match.Player1.Id%>,
        <% if (match.IsComplete) { %>
        innings: <%= match.Player1.Result.Innings%>,
        defensiveShots: <%= match.Player1.Result.DefensiveShots%>,
        wins: <%= match.Player1.Result.Wins%>,
        winner: <%= match.Player1.Result.Winner.ToString().ToLower()%>
        <% } else { %>
        innings: "",
        defensiveShots: "",
        wins: "",
        winner: false
        <% } %>
      },
      player2: {
        name: "<%= match.Player2.Name %>",
        id: <%= match.Player2.Id%>,
        <% if (match.IsComplete) { %>
        innings: <%= match.Player2.Result.Innings%>,
        defensiveShots: <%= match.Player2.Result.DefensiveShots%>,
        wins: <%= match.Player2.Result.Wins%>,
        winner: <%= match.Player2.Result.Winner.ToString().ToLower()%>
        <% } else { %>
        innings: "",
        defensiveShots: "",
        wins: "",
        winner: false
        <% } %>
      }
    };
    <% } %>
    var $current_match_id = null;
    var $current_match_status = null;
    var $current_match_row = null;

    $(document).ready(function () {
      // create tabs
      //$("#matches_tabs").tabs();
      // create ajax form
      $("#enter_results_form").ajaxForm(function(response, status, xhr, form) {
        $log("response: ", response);
        $log("status: ", status);
        $log("xhr: ", xhr);
        $log("form: ", form);
        if (xhr.status === 200) {
          // update was successful, remove match from table
          var id = form.find("input[name='Id']").val();
          $current_match_rows.effect("highlight", 2000);
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
            //$current_match_status.html('<%= Html.ContentImage("loading.gif", "Loading") %>&nbsp;Please wait...');
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
        $current_match_rows = $("tr[id^='" + $current_match_id + "_']");
        var form = $("#enter_results_form");
        populateResultsForm(form, match);
        $enter_results_dialog.dialog("open");
      });

      // set input.integer-input text boxes to accept numeric input only
      $(".integer-input").numeric();
    });

    function populateResultsForm(form, match) {
      function populateFormPlayer(form, playerIndex, player) {
        form.find("[name='Player" + playerIndex + ".Innings']").val(player.innings);
        form.find("[name='Player" + playerIndex + ".DefensiveShots']").val(player.defensiveShots);
        form.find("[name='Player" + playerIndex + ".Wins']").val(player.wins);
      }

      form.find("[name='Id']").val(match.id);
      form.find("#player1name").text(match.player1.name);
      form.find("#player2name").text(match.player2.name);
      form.find("#player1Winner, #player1_id").val(match.player1.id);
      form.find("#player2Winner, #player2_id").val(match.player2.id);
      if (match.isComplete) {
        populateFormPlayer(form, 1, match.player1);
        populateFormPlayer(form, 2, match.player2);
        if (match.player1.winner) {
          form.find("#player1Winner").attr("checked", "checked");
        }
        else {
          form.find("#player2Winner").attr("checked", "checked");
        }
      }
      else {
        form.clearForm();
      }
    }

  </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <%= Html.ScriptInclude("jquery.alphanumeric.js") %>
  <%= Html.ScriptInclude("jquery.form.js") %>
</asp:Content>
