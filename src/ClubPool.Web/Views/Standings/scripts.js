(function (sr) {
  sr.registerViewScript("standings", function () {
    $("#divisiontabs").tabs();
    $(".division-standings-tabs").tabs();
    $("table tbody tr:odd").addClass("alt");
  });
})($.scriptRegistrar);