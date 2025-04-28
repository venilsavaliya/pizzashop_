const selectedModifierItems = new Set();

let TempOrderItemList = [];

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

  console.log("hello", data);

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
      itemRate: data.rate,
      itemName: data.name,
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


//======== Order Taking Menu ===========

// Calculating The Tax 
function calculateAllTaxes(taxList, TempOrderItemList) {
  let subtotal = 0;
  let otherTax = 0;
  let total = 0;

  // 1. Calculate subtotal and item-wise tax
  TempOrderItemList.forEach(item => {
      const baseRate = item.Rate;
      const quantity = item.Quantity;
      console.log(item)
      // Sum modifier item rates
      const modifierTotal = item.ModifierItems?.reduce((sum, mod) => {
          return sum + (mod.rate || 0);
      }, 0) || 0;

      console.log(modifierTotal)

      const itemTotal = (baseRate + modifierTotal) * quantity;
      subtotal += itemTotal;

      // Item tax
      const itemTax = (itemTotal * item.TaxPercentage) / 100;
      otherTax += itemTax;

      console.log("itemtotal",itemTotal);
  });

  // 2. Show Subtotal
  const subtotalSpan = document.getElementById("subTotal");
  if (subtotalSpan) {
      subtotalSpan.textContent = `₹ ${subtotal.toFixed(2)}`;
  }

  // 3. Show Other Tax
  const otherTaxSpan = document.getElementById("otherTaxAmount");
  if (otherTaxSpan) {
      otherTaxSpan.textContent = `₹ ${otherTax.toFixed(2)}`;
  }

  total = subtotal + otherTax;

  // 4. Apply additional taxes
  taxList.forEach(tax => {
   
    const taxId = tax.taxId;
      const taxType = tax.type;
      const taxAmount = tax.taxAmount;
      const isDefault = tax.isdefault;

      let applyTax = isDefault;

      if (!isDefault) {
          const checkbox = document.querySelector(`.tax-checkbox[data-taxid="${taxId}"]`);
          applyTax = checkbox && checkbox.checked;
      }

      let taxValue = 0;

      if (applyTax) {
          taxValue = taxType === "Percentage"
              ? (subtotal * taxAmount) / 100
              : taxAmount;

          const taxValueSpan = document.querySelector(`.tax-value[data-taxid="${taxId}"]`);
          if (taxValueSpan) {
              taxValueSpan.textContent = `₹ ${taxValue.toFixed(2)}`;
          }

          total += taxValue;
      } else {
          const taxValueSpan = document.querySelector(`.tax-value[data-taxid="${taxId}"]`);
          if (taxValueSpan) {
              taxValueSpan.textContent = `₹ 0.00`;
          }
      }
  });

  // 5. Show grand total
  const totalSpan = document.getElementById("grandTotal");
  if (totalSpan) {
      totalSpan.textContent = `₹ ${total.toFixed(2)}`;
  }
}

// Function To Increase Quantity
function increaseQuantity(index) {
  const item = TempOrderItemList.find(x => x.Index == index);
  if (item) {
      item.Quantity += 1;

      // Update Quantity In Row
      const row = $(`#${index}`);
      if (row) {
        row.find(".quantityInput").val(item.Quantity);
      }

      // Recalculate tax and Total
      calculateAllTaxes(TaxList, TempOrderItemList);
  }
}

// Function To Decrease Quantity
function decreaseQuantity(index) {
  const item = TempOrderItemList.find(x => x.Index === index);
  if (item && item.Quantity > 1) {
      item.Quantity -= 1;

      // Update Quantity In Row
      const row = $(`#${index}`);
      if (row) {
        row.find(".quantityInput").val(item.Quantity);
      }

      // Recalculate totals
      calculateAllTaxes(taxList, TempOrderItemList);
  } 
}

// Function For Delete Order Item 
function removeItem(index) {
  // Remove item from the list
  TempOrderItemList = TempOrderItemList.filter(item => item.Index != index);

  // Remove the row from the DOM
  $(`#${index}`).remove();

  // Recalculate totals
  calculateAllTaxes(TaxList, TempOrderItemList);
}

// Event Lisner For Delete Order Item
$(document).on('click', '.delete-item', function () {
  const index = $(this).closest("tr").attr("id");
  if (index !== undefined) {
      removeItem(parseInt(index));
  }
});

// Event Lisner For Increasing Quantity Of Order Item
$(document).on('click', '#IncreaseQuantity', function () {
  const index = $(this).closest("tr").attr("id");
  if (index !== undefined) {
      increaseQuantity(parseInt(index));
  }
});

// Event Lisner For Decreasing Quantity Of Order Item
$(document).on('click', '#DecreaseQuantity', function () {
  const index = $(this).closest("tr").attr("id");
  if (index !== undefined) {
      decreaseQuantity(parseInt(index));
  }
});






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

  $("#selectmodifieritemmodal").on("hidden.bs.modal", function () {
    selectedModifierItems.clear();
  });
});
