(function (sr) {
  sr.registerViewScript("dashboard/dashboard", function () {
    $("#lastmeet tbody tr:odd:not(:last)").addClass("match");
    $("#lastmeet tbody tr:even").addClass("player");
    $("#season_results tr:last").addClass("last");
    $("#skill_level_calc tr:last").addClass("last");
  });
})($.scriptRegistrar);