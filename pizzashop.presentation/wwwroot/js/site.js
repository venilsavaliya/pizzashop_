// image validation function
function updateFileName(myFile, fileNameSpan, imageFileValidation) {
  var input = document.getElementById(myFile);
  var fileNameSpan = document.getElementById(fileNameSpan);
  var validationSpan = document.getElementById(imageFileValidation);
  validationSpan.textContent = "";

  if (input.files && input.files.length > 0) {
    var file = input.files[0];
    var fileSizeMB = file.size / (1024 * 1024);
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.jfif)$/i;

    //  Check extension
    if (!allowedExtensions.exec(file.name)) {
      validationSpan.textContent =
        "Only JPG, JPEG, PNG, and JFIF files are allowed.";
      fileNameSpan.textContent = "Drag and Drop or browse file";
      input.value = "";
      return;
    }

    //  Check file size
    if (fileSizeMB > 2) {
      validationSpan.textContent = "File size should not exceed 2 MB.";
      fileNameSpan.textContent = "Drag and Drop or browse file";
      input.value = "";
      return;
    }

    //  If valid, show file name
    fileNameSpan.textContent = file.name;
  } else {
    fileNameSpan.textContent = "Drag and Drop or browse file";
  }
}
// image validation function
function ValidateImage(myFile, fileNameSpan, imageFileValidation) {
  var input = document.getElementById(myFile);
  var fileNameSpan = document.getElementById(fileNameSpan);
  var validationSpan = document.getElementById(imageFileValidation);
  validationSpan.textContent = "";

  if (input.files && input.files.length > 0) {
    var file = input.files[0];
    var fileSizeMB = file.size / (1024 * 1024);
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.jfif)$/i;

    //  Check extension
    if (!allowedExtensions.exec(file.name)) {
      validationSpan.textContent =
        "Only JPG, JPEG, PNG, and JFIF files are allowed.";
      input.value = "";
      return;
    }

    //  Check file size
    if (fileSizeMB > 2) {
      validationSpan.textContent = "File size should not exceed 2 MB.";
      input.value = "";
      return;
    }

    //  If valid, show file name
    fileNameSpan.textContent = file.name;
  }
}

//update live time function
function updateTimers() {
  $(".live-timer").each(function () {
    const createdAt = new Date($(this).data("createdat"));

    const now = new Date();
    const diffMs = now - createdAt;

    if (isNaN(diffMs)) {
      $(this).text("0 min");
      return;
    }

    const diffSecs = Math.floor(diffMs / 1000);
    const hours = Math.floor(diffSecs / 3600);
    const minutes = Math.floor((diffSecs % 3600) / 60);
    const seconds = diffSecs % 60;

    const formatted = `${hours}h ${minutes}min ${seconds}s`;
    $(this).text(formatted);
  });
}

$(document).ready(function () {
  function loadPage(page) {
    var searchKeyword = $("#search").val();
    $.ajax({
      url: window.location.pathname,
      type: "GET",
      data: { searchKeyword: searchKeyword, page: page },
      success: function (data) {
        $("#paginationContent").html(data);
      },
    });
  }

  $(document).on("click", ".page-link", function () {
    var page = $(this).data("page");
    loadPage(page);
  });

  $("#search").keyup(function () {
    loadPage(1);
  });

  // This Function For Reinitialize Validation on Form because it is loaded dyanmically out of dom
  function reinitializeValidation() {
    $("form").each(function () {
      $.validator.unobtrusive.parse($(this));
    });
  }
  $(document).ajaxComplete(function () {
    reinitializeValidation();
  });

 
});
