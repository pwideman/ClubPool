(function () {
  var sr = {};
  var registrations = {};

  sr.registerViewScript = function (name, fn) {
    if (!registrations[name]) {
      registrations[name] = [];
    }
    registrations[name].push(fn);
  };

  sr.initViewScript = function (name) {
    if (registrations[name]) {
      var scripts = registrations[name];
      for (var i = 0; i < scripts.length; i++) {
        scripts[i]($model);
      }
    }
  }

  $.scriptRegistrar = sr;
} ($));