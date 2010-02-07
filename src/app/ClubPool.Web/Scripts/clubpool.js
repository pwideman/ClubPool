// initialise plugins
$(document).ready(function() {
  $('ul.sf-menu').supersubs({
    minWidth: 12,
    maxWidth: 27,
    extraWidth: 1
  }).superfish({
    animation: {opacity:"show"},
    speed: "fast"
  });
  var sidebarCornerRadius = "10px";
  $("#sidebar").corner(sidebarCornerRadius);
  $(".sidebarGadgetContainer").corner(sidebarCornerRadius);
  $(".sidebarGadgetContent").corner(sidebarCornerRadius);
  var formCornerRadius = "12px";
  $("form.normal").corner(formCornerRadius);
  $("form.normal fieldset").corner(formCornerRadius);
  $("#loginStatusPanel").corner("bottom 30px");
});
