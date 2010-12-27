<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ClubPool.Web.Controllers.Meets.ViewModels.MeetViewModel>" %>
<%@ Import Namespace="MvcContrib.UI.Html" %>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContentPlaceHolder" runat="server">
Match Details
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
  <%= Html.ScriptInclude("jquery.alphanumeric.js") %>
  <%= Html.ScriptInclude("jquery.form.js") %>
  <%= Html.ScriptInclude("jquery.timeentry.min.js") %>
  <%= Html.Stylesheet("jquery.timeentry.css") %>
  <script type="text/javascript">
    // preload loading image
    var img = new Image(16, 16);
    img.src = "<%= Url.ContentImageUrl("loading-small.gif")%>";
  </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="heading">
    <span>Match Details</span>
  </div>
  <p>
    <strong><%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(Model.Team1Id), Model.Team1Name) %></strong> vs. 
    <strong><%= Html.ActionLink<ClubPool.Web.Controllers.Teams.TeamsController>(c => c.Details(Model.Team2Id), Model.Team2Name) %></strong>, 
    scheduled for week <%= Model.ScheduledWeek %> (<%= Model.ScheduledDate%>)
  </p>
  <div class="action-button-row">
    <div class="action-button">
      <%= Html.ContentImage("printer-medium.png", "Print a soresheet") %>
      <%= Html.ActionLink<ClubPool.Web.Controllers.Meets.MeetsController>(u => u.Scoresheet(Model.Id), "Print a scoresheet") %>
    </div>
  </div>
  <table id="match_details" class="match-details" cellpadding="0" cellspacing="0">
    <thead>
      <tr>
        <th>Match</th>
        <th>Player</th>
        <th>Innings</th>
        <th>Defensive Shots</th>
        <th>Wins</th>
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
            if (match.Player1.Winner) {
              firstWinnerClass = " winner";
            }
            else {
              secondWinnerClass = " winner";
            }
          }
           %>
          <tr class="first<%= firstWinnerClass%>" id="<%= match.Id%>_1">
            <td><%= matchIndex %></td>
            <td><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.View(match.Player1.Id), match.Player1.Name)%></td>
            <td id="<%=match.Id%>_p1innings"><%= match.Player1.Innings%></td>
            <td id="<%=match.Id%>_p1defshots"><%= match.Player1.DefensiveShots%></td>
            <td id="<%=match.Id%>_p1wins"><%= match.Player1.Wins%></td>
            <td/>
          </tr>
          <tr class="second<%= secondWinnerClass%>" id="<%= match.Id%>_2">
            <td></td>
            <td><%= Html.ActionLink<ClubPool.Web.Controllers.Users.UsersController>(c => c.View(match.Player2.Id), match.Player2.Name)%></td>
            <td id="<%=match.Id%>_p2innings"><%= match.Player2.Innings%></td>
            <td id="<%=match.Id%>_p2defshots"><%= match.Player2.DefensiveShots%></td>
            <td id="<%=match.Id%>_p2wins"><%= match.Player2.Wins%></td>
            <td/>
          </tr>
          <tr class="status" id="<%= match.Id%>_3">
            <td>Status:</td>
            <td colspan="4" class="status" id="<%= match.Id%>_status">
              <%= Html.Encode(match.Status)%>
            </td>
            <td>
              <% if (Model.AllowUserToEnterResults) { %>
              <div class="action-button-row-small">
                <div class="action-button enter-results-link" id="<%= match.Id %>">
                  <%= Html.ContentImage("enterresults-medium.png", "Enter results")%>
                  Enter Results
                </div>
              </div>
              <% } %>
            </td>
          </tr>
        <% } %>
    </tbody>
  </table>

  <div id="enter_results_window">
    <% using (var form = Html.BeginForm<ClubPool.Web.Controllers.Matches.MatchesController>(c => c.Edit(null), FormMethod.Post, new { id = "enter_results_form" })) { %>
    <%= Html.AntiForgeryToken() %>
    <input type="hidden" name="Id" id="match_id" />
    <input type="hidden" name="Player1Id" id="Player1Id" />
    <input type="hidden" name="Player2Id" id="Player2Id" />
    <table>
      <thead>
        <tr>
          <th>Player</th>
          <th>Innings</th>
          <th>Defensive Shots</th>
          <th>Wins</th>
          <th>Winner</th>
          <th>Date and Time Played</th>
        </tr>
      </thead>
      <tbody>
        <tr class="results">
          <td class="name" id="player1name"></td>
          <td><input type="text" name="Player1Innings" class="integer-input"/></td>
          <td><input type="text" name="Player1DefensiveShots" class="integer-input"/></td>
          <td><input type="text" name="Player1Wins" class="integer-input"/></td>
          <td><input type="radio" name="Winner" id="player1Winner" /></td>
          <td class="date-time"><%= Html.TextBox("Date", "", new { @class = "datepicker" }) %></td>
        </tr>
        <tr class="results">
          <td class="name" id="player2name"></td>
          <td><input type="text" name="Player2Innings" class="integer-input"/></td>
          <td><input type="text" name="Player2DefensiveShots" class="integer-input"/></td>
          <td><input type="text" name="Player2Wins" class="integer-input"/></td>
          <td><input type="radio" name="Winner" id="player2Winner" /></td>
          <td class="date-time"><%= Html.TextBox("Time", "", new { @class = "timeentry" }) %></td>
        </tr>
        <tr class="forfeit">
          <td colspan="99">
            <input type="checkbox" name="IsForfeit" id="IsForfeit" value="true"/><label for="IsForfeit">Match was forfeited (you must still select a winner)</label>
          </td>
        </tr>
        <tr class="status">
          <td colspan="99" id="enter_results_form_status">&nbsp;</td>
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
      datePlayed: "<%= match.DatePlayed%>",
      timePlayed: "<%= match.TimePlayed%>",
      dateScheduled: "<%= Model.ScheduledDate%>",
      timeScheduled: "<%= match.TimeScheduled%>",
      isForfeit: <%= match.IsForfeit.ToString().ToLower()%>,
      player1: {
        name: "<%= match.Player1.Name %>",
        id: <%= match.Player1.Id%>,
        innings: "<%= match.Player1.Innings%>",
        defensiveShots: "<%= match.Player1.DefensiveShots%>",
        wins: "<%= match.Player1.Wins%>",
        winner: <%= match.Player1.Winner.ToString().ToLower()%>
      },
      player2: {
        name: "<%= match.Player2.Name %>",
        id: <%= match.Player2.Id%>,
        innings: "<%= match.Player2.Innings%>",
        defensiveShots: "<%= match.Player2.DefensiveShots%>",
        wins: "<%= match.Player2.Wins%>",
        winner: <%= match.Player2.Winner.ToString().ToLower()%>
      }
    };
    <% } %>
    var $current_match_id = null;
    var $current_match_status = null;
    var $current_match_rows = null;
    var $waiting_on_submit = false;

    $(document).ready(function () {
      // set up date & time controls
      $("input.datepicker").datepicker({
        showOn: 'button',
        buttonImage: '<%= Url.ContentImageUrl("calendar.gif") %>',
        buttonImageOnly: true
      });
      $("input.timeentry").timeEntry({
        ampmPrefix: " ",
        spinnerImage: '<%= Url.ContentImageUrl("spinnerDefault.png") %>'
      });
      // create ajax form
      $("#enter_results_form").ajaxForm({
        success: function(response, status, xhr, form) {
          $log("ajax form submit success:");
          $log("response: ", response);
          $log("status: ", status);
          $log("xhr: ", xhr);
          $log("form: ", form);
          $waiting_on_submit = false;
          $("#enter_results_form_status").html("");
          if (xhr.status === 200) {
            if (response.Success) {
              // update was successful, update match object
              $("#enter_results_window").dialog("close");
              var match = updateMatchFromForm($current_match_id);
              updateTableForMatch(match);
              $current_match_rows.effect("highlight", 2000);
            }
            else {
              var status = "<div>";
              if (response.ValidationResults && response.ValidationResults.length > 0) {
                if (response.ValidationResults.length > 1) {
                  // we have multiple validation errors, display them in a list
                  status += "Validation errors: <ul>";
                  for(var i=0;i < response.ValidationResults.length;i++) {
                    var result = response.ValidationResults[i];
                    status += "<li>" + result.Message + "</li>";
                  }
                  status += "</ul>"
                }
                else {
                  status += "Validation error: " + response.ValidationResults[0].Message;
                }
              }
              else {
                // just display the message
                status += response.Message;
              }
              status += "</div>";
              $("#enter_results_form_status").html(status).addClass("error");
            }
          }
        },
        error: function(xhr, status, error) {
          $log("ajax form submit error:");
          $log("error: ", error);
          $log("status: ", status);
          $log("xhr: ", xhr);
          $waiting_on_submit = false;
          $("#enter_results_form_status").html("The server encountered an error processing your request, try again").addClass("error");
        },
        beforeSubmit: function() {
          var valid = $("#enter_results_form").validate().form();
          if (valid) {
            $("#enter_results_form_status").removeClass("error").html('<%= Html.ContentImage("loading-small.gif", "Loading") %>&nbsp;Please wait...');
            $waiting_on_submit = true;
          }
          return valid;
        }
      });

      // set up enter results modal dialog
      var $enter_results_dialog = $("#enter_results_window").dialog({
        autoOpen: false,
        title: "Enter Match Results",
        buttons: {
          "OK": function () {
            if ($waiting_on_submit) {
              return;
            }
            $("#enter_results_form").submit();
          },
          "Cancel": function () {
            if ($waiting_on_submit) {
              return;
            }
            $(this).dialog("close");
          }
        },
        resizable: false,
        width: 600,
        modal: true
      });

      // when user checks forfeit checkbox, disable all text inputs
      $("#IsForfeit").change(function() {
        var forfeit = $(this).attr("checked");
        forfeitChanged(forfeit);
      });

      // set up validation
      $("#enter_results_form").validate({
        errorPlacement: function() { }
      });

      // add click event handler to enter results image links
      $(".enter-results-link").click(function () {
        var match = $matches[this.id];
        $current_match_id = this.id;
        $current_match_rows = $("tr[id^='" + $current_match_id + "_']");
        $current_match_status = $("#" + match.id + "_status");
        var form = $("#enter_results_form");
        $("#enter_results_form_status").html("&nbsp;").removeClass("error");
        populateResultsForm(form, match);
        $enter_results_dialog.dialog("open");
      });

      // set input.integer-input text boxes to accept numeric input only
      $(".integer-input").numeric();
    });

    function updateTableForMatch(match) {
      var prefix = "#" + match.id + "_";
      function updatePlayerRow(index, player) {
        var playerPrefix = prefix + "p" + index;
        $(playerPrefix + "innings").html(player.innings);
        $(playerPrefix + "defshots").html(player.defensiveShots);
        $(playerPrefix + "wins").html(player.wins);
      }

      if (match.isForfeit) {
        $current_match_status.html("Forfeited");
      }
      else {
        $current_match_status.html("Played on {0} {1}".format(match.datePlayed, match.timePlayed));
      }
      updatePlayerRow(1, match.player1);
      updatePlayerRow(2, match.player2);
      var winnerRow = loserRow = "#" + match.id + "_";
      if (match.player1.winner) {
        winnerRow += "1";
        loserRow += "2";
      }
      else {
        winnerRow += "2";
        loserRow += "1";
      }
      $(winnerRow).addClass("winner");
      $(loserRow).removeClass("winner");
    }

    function updateMatchFromForm(id) {
      function updatePlayerFromForm(form, playerIndex, player) {
        player.innings = form.find("[name='Player" + playerIndex + "Innings']").val() || 0;
        player.defensiveShots = form.find("[name='Player" + playerIndex + "DefensiveShots']").val() || 0;
        player.wins = form.find("[name='Player" + playerIndex + "Wins']").val() || 0;
      }
      var form = $("#enter_results_form");
      var match = $matches[id];
      match.isComplete = true;
      match.isForfeit = form.find("[name='IsForfeit']").attr("checked");
      if (match.isForfeit) {
        match.datePlayed = "";
        match.timePlayed = "";
        match.player1 = { id: match.player1.id, innings: "", defensiveShots: "", wins: "" };
        match.player2 = { id: match.player2.id, innings: "", defensiveShots: "", wins: "" };
      }
      else {
        match.datePlayed = form.find("[name='Date']").val();
        match.timePlayed = form.find("[name='Time']").val();
        updatePlayerFromForm(form, 1, match.player1);
        updatePlayerFromForm(form, 2, match.player2);
      }
      if (form.find("#player1Winner").attr("checked")) {
        match.player1.winner = true;
        match.player2.winner = false;
      }
      else {
        match.player1.winner = false;
        match.player2.winner = true;
      }
      return match;
    }

    function populateResultsForm(form, match) {
      function populateFormPlayer(form, playerIndex, player) {
        form.find("[name='Player" + playerIndex + "Innings']").val(player.innings);
        form.find("[name='Player" + playerIndex + "DefensiveShots']").val(player.defensiveShots);
        form.find("[name='Player" + playerIndex + "Wins']").val(player.wins);
      }

      form.find("[name='Id']").val(match.id);
      form.find("#player1name").text(match.player1.name);
      form.find("#player2name").text(match.player2.name);
      form.find("#player1Winner, #Player1Id").val(match.player1.id);
      form.find("#player2Winner, #Player2Id").val(match.player2.id);
  
      form.clearForm();
      forfeitChanged(false);
      
      $("#enter_results_form").find("input[type='text']").each(function() {
        $(this).removeClass("error");
      });     
      
      if (match.isComplete) {
        if (!match.isForfeit) {
          populateFormPlayer(form, 1, match.player1);
          populateFormPlayer(form, 2, match.player2);
        }
        else {
          form.find("[name='IsForfeit']").attr("checked", "checked");
          forfeitChanged(true);
        }
        if (match.player1.winner) {
          form.find("#player1Winner").attr("checked", "checked");
        }
        else {
          form.find("#player2Winner").attr("checked", "checked");
        }
      }

      var date = null;
      var time = null;
      if (!match.isComplete || match.isForfeit) {
        date = match.dateScheduled;
        time = match.timeScheduled;
      }
      else {
        date = match.datePlayed;
        time = match.timePlayed;
      }
      form.find("[name='Date']").val(date);
      form.find("[name='Time']").val(time);
    }

    function forfeitChanged(forfeit) {
      $("#enter_results_form").find("input[type='text']").each(function() {
        if (forfeit) {
          $(this).attr("disabled", "disabled").removeClass("required").removeClass("error");
        }
        else {
          $(this).removeAttr("disabled").addClass("required");
        }
      });
    }

  </script>
</asp:Content>