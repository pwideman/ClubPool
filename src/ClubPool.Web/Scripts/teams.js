(function (sr) {
  // teams/_editteam partial
  sr.registerViewScript("teams/_editteam", function () {
    $("#Name").focus();
    $(".integer-input").numeric();
  });
})($.scriptRegistrar);