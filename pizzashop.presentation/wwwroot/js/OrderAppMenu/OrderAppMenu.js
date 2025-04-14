// function to toggle active class between category
function ToggleActiveClass() {
  $(".orderApp-category-item").on("click", function () {
    $(".orderApp-category-item").removeClass("OrderApp_menu_active_option"); // remove from all
    $(this).addClass("OrderApp_menu_active_option"); // add to the clicked one
  });
}

function loadcategories() {
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetCategoryList",
    //   data: { cat: id },
    success: function (data) {
      $("#category_list").html(data);
      ToggleActiveClass();
    },
  });
}

function loadMenuItems(catid,searchkeyword,isfav) {
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetMenuitemListById",
      data: { catid: catid,searchkeyword,isfav },
    success: function (data) {
      $("#menuitems_card").html(data);
      ToggleActiveClass();
    },
  });
}

$(document).ready(function () {
  loadcategories();
  loadMenuItems();

  $("#menuitem-search-field").on('input',function(){
    var catid = $(".OrderApp_menu_active_option").data('catid');
    var searchkeyword = $("#menuitem-search-field").val();
    loadMenuItems(catid,searchkeyword);
  })
});
