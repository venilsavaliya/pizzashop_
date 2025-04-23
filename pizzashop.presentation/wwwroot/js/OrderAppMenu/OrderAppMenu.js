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
  console.log("catid", catid);
  // if(!catid)
  // {
  //   catid = $(".OrderApp_menu_active_option").data("catid");
  // }
  // if(!searchkeyword)
  // {
  //   searchkeyword = $("#menuitem-search-field").val();
  // }

  // if(catid==-1)
  // {
  //   isfav = true;
  // }

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

// toggle heart between favourite and not
function toggleHeart(icon) {
  if (icon.classList.contains("bi-heart")) {
    icon.classList.remove("bi-heart");
    icon.classList.add("bi-heart-fill");
  } else {
    icon.classList.remove("bi-heart-fill");
    icon.classList.add("bi-heart");
  }
}

// ajax call to make item favourite

function ChangeItemFavouriteState(id, event) {
  event.stopPropagation();

  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/ChangeStatusOfFavouriteItem",
    data: { itemid: id },
    success: function (data) {
      var catid = $(".OrderApp_menu_active_option").data("catid");
      if (catid == "-1") {
        loadMenuItems(null, "", true);
      } else {
        loadMenuItems(catid);
      }
    },
  });
}

// add to favourite
function AddToFavourite(event, ele) {
  event.stopPropagation();

  const icon = ele.querySelector("i");

  toggleHeart(icon);
}

// open modal for select modifier item

function openSelectModifierItemModal(ele) {
  var modal = new bootstrap.Modal(
    document.getElementById("selectmodifieritemmodal")
  );
  modal.show();

  var itemid = $(ele).attr("item-id");
  // ajax call to fetch data
  console.log("hel", itemid);
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetModifierGroupListById",
    data: { itemid: itemid },
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
    if (catid == "-1") {
      loadMenuItems(null, searchkeyword, true);
    } else {
      loadMenuItems(catid, searchkeyword);
    }
  });
});
