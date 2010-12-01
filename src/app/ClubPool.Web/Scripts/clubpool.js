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
  if ($.browser.msie && $.browser.version.substr(0, 1) > 7) {

    // round the corners on standard elements
    // only do this in IE, others will use css
    // sidebar gadgets
    var sidebarCornerRadius = "10px";
    $(".sidebar-corner").corner(sidebarCornerRadius);
    // normal radius
    var normalCornerRadius = "12px";
    $(".corner").corner(normalCornerRadius);
  }
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


// begin ajaxUpdateError jQuery plugin
(function ($) {
  $.fn.ajaxUpdateError = function(options) {
    if (options === "close") {
      return this.each(function() {
        $(this.ajaxUpdateError).hide().detach();
        this.ajaxUpdateError = null;
      });
    }
    else {
      var defaults = {
        xOffset: 10,
        yOffset: 0,
        message: "Error"
      };
      var options = $.extend(defaults, options);

      return this.each(function() {
        var target = $(this);
        var pos = target.position();
        pos.left += target.outerWidth() + options.xOffset;
        pos.top += options.yOffset;
        var errorDiv = $('<div style="padding: 0 10px; position: absolute; top: ' + pos.top + 'px; left: ' + pos.left + 
          'px;" class="ui-state-error ui-corner-all" title="Click to close"><p><span class="ui-icon ui-icon-alert message-icon"></span>' + 
          options.message + '</p></div>');
        this.ajaxUpdateError = errorDiv;
        errorDiv[0].target = this;
        errorDiv.insertAfter(target).click(function(e) {
          this.target.ajaxUpdateError = null;
          $(this).hide().detach();
        });
      });
    }
  };
})(jQuery);
// end ajaxUpdateError jQuery plugin