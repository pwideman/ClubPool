(function (scriptRegistrar) {
  scriptRegistrar.registerViewScript("teams/_editteam", function () {
    $("#Name").focus();
    $(".integer-input").numeric();
  });
})($scriptRegistrar);