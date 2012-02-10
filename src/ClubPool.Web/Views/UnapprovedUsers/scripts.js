(function (sr) {
  sr.registerViewScript("unapprovedusers", function () {
    $("#approveall").click(function () {
      var approved = $(this).attr("checked");
      $("input[name=userids]").attr("checked", approved);
    });
    $("#unapproved_users tbody.content tr:odd").addClass("alt");
  });
})($.scriptRegistrar);