(function (sr) {
  // details
  sr.registerViewScript("users/details", function () {
    $(document).ready(function () {
      $("#tabs").tabs();
    });
    $("#skill_level_calc tr:last").addClass("last");
  });

  // index
  sr.registerViewScript("users/index", function () {
    $("tbody.content tr:odd").addClass("alt");
    $("#q").placeholder().keypress(function (e) {
      if (e.keyCode === 13) {
        doSearch($(this).val());
      }
    });
    $("#users-search-icon").click(function(e) {
      doSearch($("#q").val());
    });

    function doSearch(query) {
      if (query !== $.query.get("q")) {
        var search = "";
        if (query) {
          search = $.query.set("q", query).remove("page");
        }
        else {
          search = $.query.remove("q").remove("page");
        }
        var newurl = location.origin + location.pathname + search;
        location.assign(newurl);
      }
    }
  });
})($.scriptRegistrar);