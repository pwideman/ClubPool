var $scriptRegistrar = (function () {
  var my = {};
  var registrations = {};

  my.registerViewScript = function (name, fn) {
    if (!registrations[name]) {
      registrations[name] = [];
    }
    registrations[name].push(fn);
  };

  my.initViewScript = function (name) {
    if (registrations[name]) {
      var scripts = registrations[name];
      for (var i=0; i < scripts.length; i++) {
        scripts[i]();
      }
    }
  }

  return my;
} ());