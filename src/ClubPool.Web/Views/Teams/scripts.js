(function (sr) {
  // _editteam partial
  sr.registerViewScript("teams/_editteam", function () {
    $("#Name").focus();
    $(".integer-input").numeric();
  });

  // details
  sr.registerViewScript("teams/details", function () {
    $("#season-results-table tbody:last").removeClass("meet").find("tr:last").addClass("last");
  });

  // details - if user can edit name
  sr.registerViewScript("teams/details.editname", function (model) {
    // declare formOpts explicitly, since we need to use it in two different places
    var formOpts = {
      success: function (response, status, xhr, form) {
        if (xhr.status === 200) {
          if (response.Success) {
            model.currentTeamName = $("#name").val();
            $("#name").effect("highlight", 1500);
          }
          else {
            $("#name").ajaxUpdateError({ message: response.Message });
            $("#name").val(model.currentTeamName);
          }
        }
      },
      error: function (xhr, status, error) {
        $("#name").ajaxUpdateError({ message: "An error occurred on the server while saving your changes, try again" });
      },
      beforeSubmit: function () {
        var valid = $("#update_name_form").validate().form();
        var different = $("#name").val() != model.currentTeamName;
        var ret = valid && different;
        $("#name").ajaxUpdateError("close");
        return ret;
      }
    };

    // set up team name text box event handlers
    $("#name").blur(saveName).focus(function (e) {
      model.currentTeamName = $(this).val();
    });
    // create ajax form
    $("#update_name_form").ajaxForm(formOpts);

    // set up validation
    $("#update_name_form").validate({
      errorPlacement: function () { }
    });

    function saveName() {
      $("#update_name_form").ajaxSubmit(formOpts);
    }
  });
})($.scriptRegistrar);