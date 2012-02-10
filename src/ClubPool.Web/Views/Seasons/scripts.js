(function (sr) {
  sr.registerViewScript("seasons/details", function () {
    $("tbody.content tr:odd").addClass("alt");
    $("#divisiontabs").tabs({
      cookie: {}
    });
    $(".division-details-tabs").tabs({
      cookie: {}
    });
    $(".submit-form-link").click(function () {
      $(this).parents("form:first").submit();
    });
    $(".schedule-byes").numeric();
  });
})($.scriptRegistrar);