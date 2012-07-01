(function (sr) {

  sr.registerViewScript("divisions/_editdivision", function (model) {
    // set up date & time controls

    $("input.divisions-datepicker").datepicker({
      showOn: 'button',
      buttonImage: model.calendarImageUrl,
      buttonImageOnly: true
    }).width(function (index, width) { return width - 26; });
  });

})($.scriptRegistrar);