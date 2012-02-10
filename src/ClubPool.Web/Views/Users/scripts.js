(function (sr) {
  sr.registerViewScript("users/details", function () {
    $(document).ready(function () {
      $("#tabs").tabs();
    });
    $("#skill_level_calc tr:last").addClass("last");
  });
})($.scriptRegistrar);