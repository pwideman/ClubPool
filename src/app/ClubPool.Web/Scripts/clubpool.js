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
  if ($.browser.msie) {
    // round the corners on standard elements
    // only do this in IE, others will use css
    // sidebar gadgets
    var sidebarCornerRadius = "10px";
    $(".sidebar-corner").corner(sidebarCornerRadius);
    // normal radius
    var normalCornerRadius = "12px";
    $(".corner").corner(normalCornerRadius);
  }
  $(".notification").effect("fade", {easing:"easeInExpo"}, 5000);
});
