// Mass Delete Functionality For Modifier Item

let selectedModifierItems = [];

function attachMassDeleteForModifierItem() {
  // Event listener for main checkbox
  $(document).on("change", "#modifieritem_main_checkbox", function () {
    let isChecked = this.checked;
    $(".inner_checkbox_modifieritem").prop("checked", isChecked);

    if (isChecked) {
      $(".inner_checkbox_modifieritem").each(function () {
        let id = $(this).val();
        if (!selectedModifierItems.includes(id)) {
          selectedModifierItems.push(id);
        }
      });
    } else {
      $(".inner_checkbox_modifieritem").each(function () {
        let id = parseInt($(this).val());
        // Remove the unchecked item from the array
        selectedModifierItems = selectedModifierItems.filter(
          (item) => item != id
        );
      });
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".inner_checkbox_modifieritem", function () {
    let id = $(this).val();
    if (this.checked) {
      if (!selectedModifierItems.includes(id)) {
        selectedModifierItems.push(id);
      }
    } else {
      selectedModifierItems = selectedModifierItems.filter(
        (item) => item !== id
      );
    }

    // Update main checkbox state
    const allChecked =
      $(".inner_checkbox_modifieritem").length ===
        $(".inner_checkbox_modifieritem:checked").length &&
      $(".inner_checkbox_modifieritem:checked").length !== 0;
    $("#modifieritem_main_checkbox").prop("checked", allChecked);
  });

  // Restore previously selected checkboxes
  $(".inner_checkbox_modifieritem").each(function () {
    if (selectedModifierItems.includes($(this).val())) {
      $(this).prop("checked", true);
    }
  });

  // Update main checkbox state initially
  const allChecked =
    $(".inner_checkbox_modifieritem").length ===
      $(".inner_checkbox_modifieritem:checked").length &&
    $(".inner_checkbox_modifieritem:checked").length !== 0;
  $("#modifieritem_main_checkbox").prop("checked", allChecked);
}

// load modifier item list
function loadmodifieritems(id, pagesize, pagenumber) {
  selectedModifierItems = [];

  $.ajax({
    url: "/Menu/GetModifierItemsList",
    type: "GET",
    data: { modifiergroup_id: id, pageSize: pagesize, pageNumber: pagenumber },
    success: function (data) {
      $("#modifieritemstablecontainer").html(data);
      attachMassDeleteForModifierItem();
    },
  });
}

// list for selected group for add modifier item
const selectedGroups = new Set();

// list for selected group for edit modifier item
const selectedGroupsForedit = new Set();

// Update the input box with the selected names
function updateSelectedNamesForEdit() {
  let names = Array.from(selectedGroupsForedit)
    .map((item) => item.name)
    .join(", ");
  if (names.length === 0) {
    names = "Select Modifier Group";
  }
  $("#modifierGroupSelectBoxForEdit").text(names);
}

// Update the input box with the selected names
function updateSelectedNames() {
  let names = Array.from(selectedGroups)
    .map((item) => item.name)
    .join(", ");
  if (names.length === 0) names = "Select Modifier Group";
  $("#modifierGroupSelectBox").text(names);
}

// function to keep checkbox cheked if id existed in list

function checkSelectedCheckboxesforAdd() {
  // Uncheck all checkboxes first
  $(".modifieritemcheckboxofaddmodal").prop("checked", false);
  $(".modifieritem_main_checkbox_addmodal").prop("checked", false);

  // Check only the ones that match IDs in selectedModifierItemsForAddExistingModifier
  selectedModifierItemsForAddExistingModifierForAddModalTemp.forEach((id) => {
    $(`.modifieritemcheckboxofaddmodal[value='${id}']`).prop("checked", true);
  });
}

//  set edit detail for edit modifier item

function seteditmodifieritemdata(ele) {
  var c = JSON.parse(ele.getAttribute("item-obj"));

  // Make an AJAX call to get the modifier group ID list
  $.ajax({
    url: `/Menu/GetModifierGroupIdListByModifierItemId`,
    type: "GET",
    data: { id: c.modifierId },
    success: function (selectedData) {
      selectedGroupsForedit.clear();

      // Save selected data to a temp variable so it's available inside next ajax call
      const selectedIdsSet = new Set(
        selectedData.map((item) => {
          selectedGroupsForedit.add({ id: item.id, name: item.name });
          return item.id;
        })
      );

      $.ajax({
        type: "GET",
        url: "/Menu/GetModifierGroupListData",
        success: function (allGroups) {
          console.log(allGroups);

          $("#modifierGroupDropdownForEdit").html("");
          allGroups.forEach((m) => {
            const isChecked = selectedIdsSet.has(m.modifiergroupId)
              ? "checked"
              : "";
            $("#modifierGroupDropdownForEdit").append(`
              <div class="form-check">
                <input type="checkbox" class="form-check-input modifier-group-checkbox-foredit" 
                    id="modifierGroup_${m.modifiergroupId}" 
                    value="${m.modifiergroupId}" 
                    data-name="${m.name}"
                    ${isChecked}>
                <label class="form-check-label" for="modifierGroup_${m.modifiergroupId}">${m.name}</label>
              </div>`);
          });

          // Update names display
          updateSelectedNamesForEdit();
        },
        error: function (xhr, status, error) {
          console.error("Error loading modifier groups:", error);
        },
      });
    },
    error: function (xhr, status, error) {
      console.error("Error fetching modifier group IDs:", error);
    },
  });

  var editmenuitem = document.getElementById("editmodifieritemmodal");
  editmenuitem.querySelector("#Rate").value = c.rate;
  editmenuitem.querySelector("#ModifierName").value = c.name;
  editmenuitem.querySelector("#Quantity").value = c.quantity;
  editmenuitem.querySelector("#Unit").value = c.unit;
  editmenuitem.querySelector("#Description").value = c.description;
  editmenuitem.querySelector("#ModifierId").value = c.modifierId;
}

// set delete modifier itemid to delete modal
function setDeleteModifierid(element) {
  var modifiergroupid = element.getAttribute("modifiergroup-id");
  var modifierid = element.getAttribute("modifier-id");
  var deleteBtn = document.getElementById("deleteModifierItemBtn");
  deleteBtn.setAttribute("modifiergroup-id", modifiergroupid);
  deleteBtn.setAttribute("modifieritem-id", modifierid);
}

$(document).ready(function () {
  $("#AddModifierItemForm").validate({
    rules: {
      "ModifierItem.ModifierName": {
        required: true,
        minlength: 2,
      },
      "ModifierItem.Rate": {
        required: true,
        number: true,
        min: 0,
      },
      "ModifierItem.Quantity": {
        required: true,
        number: true,
        min: 0,
      },
      "ModifierItem.Unit": {
        required: true,
      },
    },
    messages: {
      "ModifierItem.ModifierName": {
        required: "Please enter a name.",
        minlength: "Name must be at least 2 characters.",
      },
      "ModifierItem.Rate": {
        required: "Please enter a rate.",
        number: "Please enter a valid number.",
        min: "Rate cannot be negative.",
      },
      "ModifierItem.Quantity": {
        required: "Please enter a quantity.",
        number: "Please enter a valid number.",
        min: "Quantity cannot be negative.",
      },
      "ModifierItem.Unit": {
        required: "Please select a unit.",
      },
    },
    errorElement: "span",
    errorClass: "text-danger",
    highlight: function (element) {
      $(element).addClass("is-invalid");
    },
    unhighlight: function (element) {
      $(element).removeClass("is-invalid");
    },
  });

  // Reset Modifier Group Form When Modal Close
  $("#addmodifieritemmodal").on("hidden.bs.modal", function () {
    console.log("closed");
    // Reset the form inside the modal
    $(this).find("form")[0].reset();

    // Reset ASP.NET Core validation
    var validator = $(this).find("form").validate();
    validator.resetForm();

    $("#modifierGroupSelectBox").text("Select Modifier Group");
    // Remove invalid classes
    $(this).find(".is-invalid").removeClass("is-invalid");
    selectedGroups.clear();

    $("#selectmodifiergroupvalidationforadd").addClass('d-none');
  });

  // add modifier item form submit
  $("#AddModifierItemForm").submit(function (e) {
    e.preventDefault();

    // if (!validateFormAddModifierItem()) {
    //   return;
    // }

    const selectedModifierGroups = $("#modifierGroupDropdown input:checked").length;

      if (selectedModifierGroups == 0) {
        console.log('selected group len',selectedModifierGroups)
        $("#selectmodifiergroupvalidationforadd").removeClass("d-none")

          return;
      }

    if(!$(this).valid())
    {
      return;
    }

    var formData = new FormData(this);

    var selectedModGroupId = Array.from(selectedGroups).map((item) => item.id);

    formData.append("ModifierGroupid", JSON.stringify(selectedModGroupId));

    $.ajax({
      url: "/Menu/AddModifierItem",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        // close the opened  modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("addmodifieritemmodal")
        );
        Modal.hide();

        const modgroupid = $("#modifieritem_main_checkbox").attr(
          "modifiergroup-id"
        );

        if (response.success) {
          loadmodifieritems(modgroupid);
          console.log("MODGROUPID".modgroupid);
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (err) {
        console.error("Error adding item:", err);
      },
    });
  });

  $("#EditModifierItemForm").validate({
    rules: {
      "ModifierItem.ModifierName": {
        required: true,
        minlength: 2,
      },
      "ModifierItem.Rate": {
        required: true,
        number: true,
        min: 0,
      },
      "ModifierItem.Quantity": {
        required: true,
        number: true,
        min: 0,
      },
      "ModifierItem.Unit": {
        required: true,
      },
    },
    messages: {
      "ModifierItem.ModifierName": {
        required: "Please enter a name.",
        minlength: "Name must be at least 2 characters.",
      },
      "ModifierItem.Rate": {
        required: "Please enter a rate.",
        number: "Please enter a valid number.",
        min: "Rate cannot be negative.",
      },
      "ModifierItem.Quantity": {
        required: "Please enter a quantity.",
        number: "Please enter a valid number.",
        min: "Quantity cannot be negative.",
      },
      "ModifierItem.Unit": {
        required: "Please select a unit.",
      },
    },
    errorElement: "span",
    errorClass: "text-danger",
    highlight: function (element) {
      $(element).addClass("is-invalid");
    },
    unhighlight: function (element) {
      $(element).removeClass("is-invalid");
    },
  });

  // Reset Modifier Group Form When Modal Close
  $("#editmodifieritemmodal").on("hidden.bs.modal", function () {
    console.log("closed");
    // Reset the form inside the modal
    $(this).find("form")[0].reset();

    // Reset ASP.NET Core validation
    var validator = $(this).find("form").validate();
    validator.resetForm();

    $("#modifierGroupSelectBox").text("Select Modifier Group");
    // Remove invalid classes
    $(this).find(".is-invalid").removeClass("is-invalid");
    selectedGroupsForedit.clear();

    $("#selectmodifiergroupvalidationforedit").addClass('d-none');
  });

  // edit modifier item form submit
  $("#EditModifierItemForm").submit(function (e) {
    e.preventDefault();

    // if (!validateFormeditModifierItem()) {
    //   return;
    // }
    if(!$(this).valid())
    {
      return;
    }

     const selectedModifierGroups = $("#modifierGroupDropdownForEdit input:checked").length;

      if (selectedModifierGroups == 0) {
        console.log('selected group len',selectedModifierGroups)
        $("#selectmodifiergroupvalidationforedit").removeClass("d-none")

          return;
      }

    var formData = new FormData(this);

    var selectedModGroupId = Array.from(selectedGroupsForedit).map(
      (item) => item.id
    );

    console.log("modidforedit", selectedModGroupId);
    formData.append("ModifierGroupid", JSON.stringify(selectedModGroupId));

    $.ajax({
      url: "/Menu/EditModifierItem",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        selectedGroups.clear();
        // close the opened  modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("editmodifieritemmodal")
        );
        Modal.hide();

        const modgroupid = $("#modifieritem_main_checkbox").attr(
          "modifiergroup-id"
        );

        if (response.success) {
          loadmodifieritems(modgroupid);
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (err) {
        console.error("Error adding item:", err);
      },
    });
  });


  // Load Modifier Group list when addmodifieritemmodal open

  // handle list showing of modifier group in add modifier item form
  $("#addmodifieritemmodal").on("shown.bs.modal", function () {
    $.ajax({
      type: "GET",
      url: "/Menu/GetModifierGroupListData",
      success: function (data) {
        // this will load modifier group list
        $("#modifierGroupDropdown").html("");
        data.forEach((m) => {
          $("#modifierGroupDropdown").append(`
            <div class="form-check">
                <input type="checkbox" class="form-check-input modifier-group-checkbox" 
                    id="modifierGroup_${m.modifiergroupId}" 
                    value="${m.modifiergroupId}" 
                    data-name="${m.name}">
                <label class="form-check-label" for="modifierGroup_${m.modifiergroupId}">${m.name}</label>
            </div>`);
        });
      },
    });
  });



  // mass delete of modifier items
  $("#deletemassitemBtn").click(function (e) {
    const modgroupid = $("#modifieritem_main_checkbox").attr(
      "modifiergroup-id"
    );

    $.ajax({
      url: "/Menu/DeleteModifierItems",
      method: "POST",
      data: {
        ids: selectedModifierItems,
        ModifierGroupid: modgroupid,
      },
      success: function (response) {
        // close the opened delete modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("deletemassitemmodal")
        );
        Modal.hide();

        //clear the search field after delete
        $("#modifieritems-search-field").val("");

        var page = $("#modifieritem-pagination-section").data("page");

        if (response.success) {
          loadmodifieritems(modgroupid, page.pageSize, page.currentPage);
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error deleting items:", error);
      },
    });
  });

  // Hide the dropdown when clicking outside
  $(document).on("click", function (e) {
    if (
      !$(e.target).closest("#modifierGroupSelectBox, #modifierGroupDropdown")
        .length
    ) {
      $("#modifierGroupDropdown").hide();
    }
  });

  // Update selected group names when checkboxes are changed
  $(document).on("change", ".modifier-group-checkbox", function () {
    const groupId = $(this).val();
    const groupName = $(this).data("name");

    if ($(this).is(":checked")) {
      selectedGroups.add({ id: groupId, name: groupName });
    } else {
      selectedGroups.forEach((item) => {
        if (item.id == groupId) {
          selectedGroups.delete(item);
        }
      });
    }

    // remove validation span if user check any checkbox
    if (selectedGroups.size > 0) {
      $("#selectmodifiergroupvalidationforadd").addClass("d-none");
    }

    updateSelectedNames();
  });

  // Hide the dropdown when clicking outside for edit
  $(document).on("click", function (e) {
    if (
      !$(e.target).closest(
        "#modifierGroupSelectBoxForEdit, #modifierGroupDropdownForEdit"
      ).length
    ) {
      $("#modifierGroupDropdownForEdit").hide();
    }
  });

  // Update selected group names when checkboxes are changed for edit
  $(document).on("change", ".modifier-group-checkbox-foredit", function () {
    const groupId = $(this).val();
    const groupName = $(this).data("name");

    if ($(this).is(":checked")) {
      selectedGroupsForedit.add({ id: groupId, name: groupName });
    } else {
      selectedGroupsForedit.forEach((item) => {
        if (item.id == groupId) {
          selectedGroupsForedit.delete(item);
        }
      });
    }

    // remove validation span if user check any checkbox
    if (selectedGroupsForedit.size > 0) {
      $("#selectmodifiergroupvalidationforedit").addClass("d-none");
    }

    updateSelectedNamesForEdit();
  });

  // Toggle the visibility of the dropdown
  $("#modifierGroupSelectBox").on("click", function () {
    $("#modifierGroupDropdown").toggle();
  });

  // Toggle the visibility of the dropdown for edit
  $("#modifierGroupSelectBoxForEdit").on("click", function () {
    $("#modifierGroupDropdownForEdit").toggle();
  });

  // Add modifier item Form Validation

  function validateFormAddModifierItem() {
    let isValid = true;
    let errorMessage = "";

    const itemName = $("#addmodifieritemname").val();
    const unit = $("#addmodifieritemunit").val();
    const rate = $("#addmodifieritemrate").val();
    const quantity = $("#addmodifieritemquantity").val();

    if (selectedGroups.size == 0) {
      isValid = false;
      $("#selectmodifiergroupvalidationforadd").removeClass("d-none");
    } else {
      $("#selectmodifiergroupvalidationforadd").addClass("d-none");
    }

    if (!itemName) {
      isValid = false;
    }

    if (!unit) {
      isValid = false;
    }

    if (!rate || isNaN(rate) || rate < 0) {
      isValid = false;
    }

    if (!quantity || isNaN(quantity) || quantity < 0) {
      isValid = false;
    }
    return isValid;
  }

  // edit modifier item Form Validation

  function validateFormeditModifierItem() {
    let isValid = true;

    const itemName = $("#ModifierName").val();
    const unit = $("#Unit").val();
    const rate = $("#Rate").val();
    const quantity = $("#Quantity").val();

    if (selectedGroupsForedit.size == 0) {
      isValid = false;
      $("#selectmodifiergroupvalidationforedit").removeClass("d-none");
    } else {
      $("#selectmodifiergroupvalidationforedit").addClass("d-none");
    }

    if (!itemName) {
      isValid = false;
    }

    if (!unit) {
      isValid = false;
    }

    if (!rate || isNaN(rate) || rate < 0) {
      isValid = false;
    }

    if (!quantity || isNaN(quantity) || quantity < 0) {
      isValid = false;
    }
    return isValid;
  }

  
  // delete single modifier item
  $("#deleteModifierItemBtn").click(function (e) {
    var modgroupid = $(this).attr("modifiergroup-id");
    var moditemid = $(this).attr("modifieritem-id");
    $.ajax({
      url: "/Menu/DeleteModifierItemById",
      method: "POST",
      data: {
        modifierid: moditemid,
        modifiergroupid: modgroupid,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModifieritemmodal")
        );
        deleteModal.hide();

        //clear the search field after delete
        $("#modifieritems-search-field").val("");

        var page = $("#modifieritem-pagination-section").data("page");

        if (response.success) {
          //get the active mod group id
          let mod_id = $("#modifier-list .category-active-option").attr(
            "modifiergroup-id"
          );
          loadmodifieritems(mod_id, page.pageSize, page.currentPage);

          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error deleting items:", error);
      },
    });
  });
});
