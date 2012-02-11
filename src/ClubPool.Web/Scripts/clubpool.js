// initialise plugins
$(function () {
  // set up the menu
  $('ul.sf-menu').supersubs({
    minWidth: 12,
    maxWidth: 27,
    extraWidth: 1
  }).superfish({
    animation: { opacity: "show" },
    speed: "fast"
  });
  $(".notification").effect("fade", { easing: "easeInExpo" }, 5000);
});

function $log(text, obj) {
  if (window.console && console.log) {
    console.log(text);
    if (obj) {
      console.log(obj);
    }
  }
}

String.prototype.format = function () {
  var s = this,
      i = arguments.length;

  while (i--) {
    s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
  }
  return s;
};

var $model = {};