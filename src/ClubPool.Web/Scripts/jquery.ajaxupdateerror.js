(function ($) {
  $.fn.ajaxUpdateError = function (options) {
    if (options === "close") {
      return this.each(function () {
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

      return this.each(function () {
        var target = $(this);
        var pos = target.position();
        pos.left += target.outerWidth() + options.xOffset;
        pos.top += options.yOffset;
        var errorDiv = $('<div style="padding: 0 10px; position: absolute; top: ' + pos.top + 'px; left: ' + pos.left +
          'px;" class="ui-state-error ui-corner-all" title="Click to close"><p><span class="ui-icon ui-icon-alert message-icon"></span>' +
          options.message + '</p></div>');
        this.ajaxUpdateError = errorDiv;
        errorDiv[0].target = this;
        errorDiv.insertAfter(target).click(function (e) {
          this.target.ajaxUpdateError = null;
          $(this).hide().detach();
        });
      });
    }
  };
})(jQuery);
