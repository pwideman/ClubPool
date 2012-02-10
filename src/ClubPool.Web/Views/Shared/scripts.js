(function (sr) {
  sr.registerViewScript("shared/_pager", function () {
    $(".pagelink").click(function () {
      location.assign($.query.set("page", $(this).data("page")));
    });
  });
})($.scriptRegistrar);