const selectedModifierItems = new Set();

let TempOrderItemList = [];

let TempTaxList = [];

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
      $("#CategoryOffcanvas").find('.offcanvas-body').html(data);
      ToggleActiveClass();
    },
  });
}

function loadMenuItems(catid, searchkeyword, isfav) {
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

//For Filling The Frontened Order List

function SetOrderItemList(orderItems) {
  index = 0;
  TempOrderItemList = [];

  for (let i of orderItems) {
    TempOrderItemList.push({
      Index: index,
      ItemId: i.itemId,
      DishId: i.dishId,
      ItemName: i.itemName,
      Rate: i.rate,
      Quantity: i.quantity,
      TaxPercentage: i.taxPercentage,
      ItemComment: i.itemComment,
      ModifierItems: i.modifierItems,
    });
    index++;
  }
}

// for getting list Of Saved Order Items

function GetOrderItemList(orderid) {
  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetMenuOrderItemList",
    data: { orderid: orderid },
    success: function (data) {
      SetOrderItemList(data);

      appendOrderItemPartialView();
    },
  });
}

// for appending existing order item to view
async function appendOrderItemPartialView() {
  $("#MenuOrderItemTable").empty();
console.log(TempOrderItemList.length)
  if(TempOrderItemList.length == 0){
   console.log("hello")
    $("#MenuOrderItemTable").append(
      `<tr><td colspan="3" class="text-center">No Item(s) Added Yet!</td></tr>`);
      return;
  }
  for (var i of TempOrderItemList) {
    const data = await getPartialViewAsync(i);
    $("#MenuOrderItemTable").append(data);
  }
}

// for getting the partial view of order item by perticular order
function getPartialViewAsync(i) {
  return new Promise((resolve, reject) => {
    $.ajax({
      type: "POST",
      url: "/OrderAppMenu/GetMenuOrderItemPartialView",
      data: i,
      success: function (data) {
        resolve(data);
      },
      error: function (err) {
        reject(err);
      },
    });
  });
}

// Calculating The Tax
function calculateAllTaxes(taxList, TempOrderItemList) {
  let subtotal = 0;
  let otherTax = 0;
  let total = 0;
  TempTaxList = [];

  // 1. Calculate subtotal and item-wise tax
  TempOrderItemList.forEach((item) => {
    const baseRate = item.Rate;
    const quantity = item.Quantity;

    // Sum modifier item rates
    const modifierTotal =
      item.ModifierItems?.reduce((sum, mod) => {
        return sum + (mod.rate || 0);
      }, 0) || 0;

    const itemTotal = (baseRate + modifierTotal) * quantity;
    subtotal += itemTotal;

    // Item tax
    const itemTax = (itemTotal * item.TaxPercentage) / 100;
    otherTax += itemTax;
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
    TempTaxList.push({
      TaxId: -1,
      Type: "Flat Amount",
      TaxAmount: otherTax,
      TaxName: "Item Tax",
    });
  }

  total = subtotal + otherTax;

  // 4. Apply additional taxes
  taxList.forEach((tax) => {
    const taxId = tax.taxId;
    const taxType = tax.type;
    const taxAmount = tax.taxAmount;
    const isDefault = tax.isdefault;

    let applyTax = isDefault;

    if (!isDefault) {
      const checkbox = document.querySelector(
        `.tax-checkbox[data-taxid="${taxId}"]`
      );
      applyTax = checkbox && checkbox.checked;
    }

    let taxValue = 0;

    if (applyTax) {
      // storing tax in list
      TempTaxList.push({
        TaxId: taxId,
        Type: taxType,
        TaxAmount: taxAmount,
        TaxName: tax.taxName,
      });

      taxValue =
        taxType === "Percentage" ? (subtotal * taxAmount) / 100 : taxAmount;

      const taxValueSpan = document.querySelector(
        `.tax-value[data-taxid="${taxId}"]`
      );
      if (taxValueSpan) {
        taxValueSpan.textContent = `₹ ${taxValue.toFixed(2)}`;
      }

      total += taxValue;
    } else {
      const taxValueSpan = document.querySelector(
        `.tax-value[data-taxid="${taxId}"]`
      );
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
  const item = TempOrderItemList.find((x) => x.Index == index);
  if (item) {
    item.Quantity += 1;

    // Update Quantity In Row
    const row = $(`#${index}`);
    if (row) {
      row.find(".quantityInput").val(item.Quantity);
      // row.find(".OrderItemTotal").text(
      //   `₹ ${(item.Rate * item.Quantity).toFixed(2)}`
      // );
    }

    // Recalculate tax and Total
    calculateAllTaxes(TaxList, TempOrderItemList);
  }
}

// Function To Decrease Quantity
function decreaseQuantity(index, id) {
  const item = TempOrderItemList.find((x) => x.Index === index);

  // ajax call to get ready quantity
  $.ajax({
    type: "Get",
    url: "/OrderAppMenu/GetReadyQuantityOfItem",
    data: { id: id },
    success: function (readyQuantity) {

      if (item && item.Quantity > 1) {

        if (item.Quantity > readyQuantity) {
 
          item.Quantity -= 1;

          // Update Quantity In Row
          const row = $(`#${index}`);
          if (row) {
            row.find(".quantityInput").val(item.Quantity);
            // row.find(".OrderItemTotal").text(
            //   `₹ ${(item.Rate * item.Quantity).toFixed(2)}`
            // );
          }

          // Recalculate totals
          calculateAllTaxes(TaxList, TempOrderItemList);

        } else {
          toastr.error(readyQuantity + " Items Already Prepared !");
        }
      }
    },
  });
}

// Function For Delete Order Item
function removeItem(index) {
  // Remove item from the list
  TempOrderItemList = TempOrderItemList.filter((item) => item.Index != index);

  // Remove the row from the DOM
  $(`#${index}`).remove();

  // Recalculate totals
  calculateAllTaxes(TaxList, TempOrderItemList);
}

// Event Lisner For Delete Order Item
$(document).on("click", ".delete-item", function () {
  const index = $(this).closest("tr").attr("id");
  const dishid = $(this).attr("dish-id");

  $.ajax({
    type: "Get",
    url: "/OrderAppMenu/GetReadyQuantityOfItem",
    data: { id: dishid },
    success: function (readyQuantity) {
      if (readyQuantity != 0) {
        toastr.error("You can't Delete These Items(s) "+ readyQuantity + " Items Already Prepared !");
      }
      else{
        if (index !== undefined) {
          removeItem(parseInt(index));
        }
      }
    },
  });
  
});

// Event Lisner For Increasing Quantity Of Order Item
$(document).on("click", "#IncreaseQuantity", function () {
  const index = $(this).closest("tr").attr("id");
  if (index !== undefined) {
    increaseQuantity(parseInt(index));
  }
});

// Event Lisner For Decreasing Quantity Of Order Item
$(document).on("click", "#DecreaseQuantity", function () {
  const index = $(this).closest("tr").attr("id");
  const dishid = $(this).attr("dish-id");
  console.log("dish", dishid);
  if (index !== undefined) {
    decreaseQuantity(parseInt(index), parseInt(dishid));
  }
});

$(document).on("submit", "#customerDetailForm", function (e) {
  e.preventDefault();

  const formdata = new FormData(this);

  $.ajax({
    type: "POST",
    url: "/OrderAppMenu/SaveCustomerDetail",
    data: formdata,
    processData: false,
    contentType: false,
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);
        bootstrap.Modal.getInstance(
          document.getElementById("CustomerDetailModal")
        ).hide();
      } else {
        toastr.error(data.message);
      }
    },
  });
});

// Event Lisner For Opening Customer Detail Modal
$(document).on("click", "#CustomerDetailButton", function () {
  const orderid = $(this).attr("order-id");

  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetOrderCustomerDetail",
    data: { orderid },
    success: function (data) {
      $("#customerDetailModalContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("CustomerDetailModal")
      );
      modal.show();
    },
  });
});

// Event Lisner For Opening Order Wise Instruction Modal
$(document).on("click", ".InstructionBtn", function () {
  const orderid = $(this).attr("order-id");
  const dishid = $(this).attr("dish-id");

  const index = $(this).attr("index");

  var Instruction = "";

  if (dishid == "0" && orderid == "0") {
    Instruction = TempOrderItemList.find((x) => x.Index == index).ItemComment;
  }

  $.ajax({
    type: "GET",
    url: "/OrderAppMenu/GetInstruction",
    data: {
      orderid: orderid,
      dishid: dishid,
      index: index,
      instruction: Instruction,
    },
    success: function (data) {
      $("#InstructionModalContent").html(data);

      var modal = new bootstrap.Modal(
        document.getElementById("InstructionsModal")
      );
      modal.show();
    },
  });
});

// Save Instruction Orderwise And ItemWise
$(document).on("submit", "#InstructionForm", function (e) {
  e.preventDefault();

  const formdata = new FormData(this);

  var dishid = formdata.get("DishId");
  var orderid = formdata.get("OrderId");

  if (dishid == "0" && orderid == "0") {
    var index = formdata.get("Index");

    TempOrderItemList.find((x) => x.Index == index).ItemComment =
      formdata.get("Instruction");

    toastr.success("Instruction Saved Successfully!");

    bootstrap.Modal.getInstance(
      document.getElementById("InstructionsModal")
    ).hide();

    appendOrderItemPartialView();
    return;
  } else if (dishid != "0" && orderid == "0") {
    var index = formdata.get("Index");

    TempOrderItemList.find((x) => x.Index == index).ItemComment =
      formdata.get("Instruction");

    bootstrap.Modal.getInstance(
      document.getElementById("InstructionsModal")
    ).hide();
  }

  $.ajax({
    type: "POST",
    url: "/OrderAppMenu/SaveInstruction",
    data: formdata,
    processData: false,
    contentType: false,
    success: function (data) {
      if (data.success) {
        toastr.success(data.message);

        bootstrap.Modal.getInstance(
          document.getElementById("InstructionsModal")
        ).hide();

        appendOrderItemPartialView();
      } else {
        toastr.error(data.message);
      }
    },
  });
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
