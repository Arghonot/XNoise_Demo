mergeInto(LibraryManager.library, {
  DownloadFile: function (base64Ptr, fileNamePtr) {
    // Convert pointers to JS strings
    var base64 = UTF8ToString(base64Ptr);
    var fileName = UTF8ToString(fileNamePtr);

    // Decode base64 into a blob
    var byteCharacters = atob(base64);
    var byteArrays = [];
    for (var i = 0; i < byteCharacters.length; i += 512) {
      var slice = byteCharacters.slice(i, i + 512);
      var byteNumbers = new Array(slice.length);
      for (var j = 0; j < slice.length; j++) {
        byteNumbers[j] = slice.charCodeAt(j);
      }
      var byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }
    var blob = new Blob(byteArrays, { type: 'image/png' });

    // Trigger download
    var link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
  }
});
