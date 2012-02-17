(function (sr) {
  sr.registerViewScript("usermatchhistory/usermatchhistory", function () {
    $("tbody.content tr:odd").addClass("alt");
  });
})($.scriptRegistrar);