const selectedModifierItems = new Set();

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

function openSelectModifierItemModal(id) {
  var modal = new bootstrap.Modal(
    document.getElementById("selectmodifieritemmodal")
  );
  modal.show();

  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetModifierItemsOfMenuItem",
    data: { id: id },
    success: function (data) {
      $("#selectmodifieritemmodalContent").html(data);
      console.log("hell",data);
      ToggleActiveClass();
    },
  });
}

function AddModifierItemToOrder(ele) {
  let itemid = $(ele).attr("item-id");
  let data = $(ele).data("obj");

  console.log("hii", itemid, data);
  return;

  $(ele).toggleClass("green_border");

  if ($(ele).hasClass("green_border")) {
    var tableId = $(ele).attr("table-id");
    var tableCapacity = $(ele).attr("table-capacity");

    selectedTables.add({
      id: tableId,
      capacity: tableCapacity,
    });
  } else {
    const toRemove = [...selectedTables].find((item) => item.id == tableId);

    if (toRemove) {
      selectedTables.delete(toRemove);
    }
  }
}

// toggle selected modifier for item

function toggleSelectedModifier(ele) {

  var itemid = $(ele).attr("item-id");
  let data = $(ele).data("obj");

  console.log("hello",data)


  const count = Array.from(selectedModifierItems).filter(
    (i) => i.modifierGroupId == data.modifiergroupId
  ).length;

  if (count == data.maxValue && !$(ele).hasClass("skyblue_bg")) {
    toastr.warning("Can't Add More Modifier Items Of This Group");
    return;
  }

  $(ele).toggleClass("skyblue_bg");

  if ($(ele).hasClass("skyblue_bg")) {
  
    selectedModifierItems.add({
      id: itemid,
      modifierGroupId: data.modifiergroupId,
      itemRate : data.rate,
      itemName : data.name,
      modifierItem: data.modifierItems?.filter((i) => i.modifierId == itemid),
    });
  } else {
    const toRemove = [...selectedModifierItems].find(
      (item) => item.id == itemid
    );

    if (toRemove) {
      selectedModifierItems.delete(toRemove);
    }
  }

  console.log(selectedModifierItems);

}

function handleAddOrder(ele)
{
  var alldata = $(ele).data('obj');
  console.log(alldata)

  const modifierItems = Array.from(selectedModifierItems).map(item => item.modifierItem);





  for(i of alldata.modifierGroups)
  {
    console.log(i.modifiergroupId)
    if(Array.from(selectedModifierItems).filter(j=>j.modifierGroupId==i.modifiergroupId)<i.minValue)
    {
      toastr.warning("Select Minimum Number Of Modifier!")
      return;
    }
  }
  $.ajax({
    type:"Post",
    url:"/OrderAppMenu/GetMenuOrderItemPartialView",
    // contentType: "application/json",
    data: {
      ItemId : alldata.itemId,
      ItemName: alldata.itemName,
      Rate:alldata.rate,
      Quantity :1,
      TaxPercentage:alldata.taxPercentage,
      ItemComment : "",
      ModifierItems:JSON.stringify(modifierItems.flat())
    },
    success:function(data)
    {
      $("#MenuOrderItemTable").append(data);
    }
  })
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

  $("#selectmodifieritemmodal").on('hidden.bs.modal',function(){
    selectedModifierItems.clear();
  })
});
