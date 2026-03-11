mergeInto(LibraryManager.library, {

  ShowAlertMessage: function (messagePtr) {
    var message = UTF8ToString(messagePtr);
    window.alert(message);
  }

});
