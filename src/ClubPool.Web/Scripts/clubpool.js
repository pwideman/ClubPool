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
