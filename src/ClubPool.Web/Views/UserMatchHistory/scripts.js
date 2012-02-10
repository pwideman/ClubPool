(function (sr) {
  sr.registerViewScript("usermatchhistory", function () {
    $("tbody.content tr:odd").addClass("alt");
  });
})($.scriptRegistrar);