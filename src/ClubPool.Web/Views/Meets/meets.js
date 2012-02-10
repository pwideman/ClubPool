(function (sr) {
  sr.registerViewScript("meets/details", function () {
    // preload images
    var loadingImage = new Image(16, 16);
    loadingImage.src = loadingImageUrl;
    var calendarImage = new Image(16, 16);
    calendarImage.src = calendarImageUrl;

    var $current_match_id = null;
    var $current_match_status = null;
    var $current_match_rows = null;
    var $waiting_on_submit = false;

    // set up date & time controls
    $("input.datepicker").datepicker({
      showOn: 'button',
      buttonImage: calendarImageUrl,
      buttonImageOnly: true
    });
    $("input.timepicker").timepicker({
      timeSeparator: ":",
      showPeriod: true,
      showLeadingZero: false
    });
    // create ajax form
    $("#enter_results_form").ajaxForm({
      success: function (response, status, xhr, form) {
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
                for (var i = 0; i < response.ValidationResults.length; i++) {
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
      error: function (xhr, status, error) {
        $waiting_on_submit = false;
        $("#enter_results_form_status").html("The server encountered an error processing your request, try again").addClass("error");
      },
      beforeSubmit: function () {
        var valid = $("#enter_results_form").validate().form();
        if (valid) {
          $("#enter_results_form_status").removeClass("error")
          .html('<img src="' + loadingImageUrl + '" alt="Loading" title="Loading"/>&nbsp;Please wait...');
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
    $("#IsForfeit").change(function () {
      var forfeit = $(this).attr("checked");
      forfeitChanged(forfeit);
    });

    // set up validation
    $("#enter_results_form").validate({
      errorPlacement: function () { }
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

      $("#enter_results_form").find("input[type='text']").each(function () {
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
      $("#enter_results_form").find("input[type='text']").each(function () {
        if (forfeit) {
          $(this).attr("disabled", "disabled").removeClass("required").removeClass("error");
        }
        else {
          $(this).removeAttr("disabled").addClass("required");
        }
      });
    }
  });
})($.scriptRegistrar);