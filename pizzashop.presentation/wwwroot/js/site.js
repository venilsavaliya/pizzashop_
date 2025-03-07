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
});
