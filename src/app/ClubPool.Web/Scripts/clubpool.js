// initialise plugins
$(document).ready(function() {
  // set up the menu
  $('ul.sf-menu').supersubs({
    minWidth: 12,
    maxWidth: 27,
    extraWidth: 1
  }).superfish({
    animation: { opacity: "show" },
    speed: "fast"
  });
  // round the corners on standard elements
  if (!$.browser.msie) {
    // only do these if the browser is not IE, because IE sucks and will screw it up
    // right edge of menu
    $("ul.sf-menu > li:last").corner("right");
    $("ul.sf-menu > li:last > a").corner("right");
    // login status panel
    $("#loginStatusPanel").corner("bottom 30px");
  }
  // sidebar gadgets
  var sidebarCornerRadius = "10px";
  $("#sidebar").corner(sidebarCornerRadius);
  $(".sidebarGadgetContainer").corner(sidebarCornerRadius);
  $(".sidebarGadgetContent").corner(sidebarCornerRadius);
  // normal forms
  var formCornerRadius = "12px";
  $("form.normal").corner(formCornerRadius);
  $("form.normal fieldset").corner(formCornerRadius);
});
