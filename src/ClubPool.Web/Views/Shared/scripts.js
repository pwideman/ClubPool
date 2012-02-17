(function (sr) {
  // pager
  sr.registerViewScript("shared/_pager", function () {
    $(".pagelink").click(function () {
      location.assign($.query.set("page", $(this).data("page")));
    });
  });

  // notification message
  sr.registerViewScript("shared/_notificationmessage", function () {
    $(".notification").effect("fade", { easing: "easeInExpo" }, 5000);
  });
})($.scriptRegistrar);