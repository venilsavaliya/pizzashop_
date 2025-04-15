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

function loadMenuItems(catid, searchkeyword, isfav) {
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetMenuitemListById",
    data: { catid: catid, searchkeyword, isfav },
    success: function (data) {
      $("#menuitems_card").html(data);
      ToggleActiveClass();
    },
  });
}

// for animation of heart
function AddToFavourite() {
  $(this).addClass('d-none'); // Make sure to select the <i> tag inside
  console.log("hello")
}

// open modal for select modifier item

function openSelectModifierItemModal(ele) {
  
  var modal = new bootstrap.Modal(document.getElementById("selectmodifieritemmodal")); 
  modal.show();

  var itemid = $(ele).attr('item-id');
  // ajax call to fetch data 
  console.log("hel",itemid);
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetModifierGroupListById",
      data: { itemid:itemid },
    success: function (data) {
      $("#category_list").html(data);
      ToggleActiveClass();
    },
  });

  // data 
  // flow
  // pass - itemid
  // get list of modifier group id
  // viemodel return 
  // itemname
  // modifiergroup -{modgroupname,min,max - {moditemname,price}}

}

$(document).ready(function () {
  loadcategories();
  loadMenuItems();

  $("#menuitem-search-field").on("input", function () {
    var catid = $(".OrderApp_menu_active_option").data("catid");
    var searchkeyword = $("#menuitem-search-field").val();
    loadMenuItems(catid, searchkeyword);

    
  });
});




