const selectedGroupsForAddEdit = new Set();
// open Add Edit Modifier Group Modal

function openAddEditModifierItemModal(id) {
  //first clear the list of selected mod groups
  selectedGroupsForAddEdit.clear();

  $.ajax({
    type: "GET",
    url: "/Menu/GetAddEditModifierItemForm",
    data: { id: id },
    success: function (data) {
      $("#AddEditModifierItemModalContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("AddEditModifierItemModal")
      );
      modal.show();

      // This Will Load Modifier Group Id For Existing ModGroup of ModItem
      if (id) {
        GetExistingModifierGroupIdForModifierItem(id);
      } else {
        GetModifierGroupListForAddItem();
      }
    },
  });
}

function GetModifierGroupListForAddItem() {
  $.ajax({
    type: "GET",
    url: "/Menu/GetModifierGroupListData",
    success: function (data) {
      // this will load modifier group list
      $("#modifierGroupDropdownForAddEdit").html("");
      data.forEach((m) => {
        $("#modifierGroupDropdownForAddEdit").append(`
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
}

function GetExistingModifierGroupIdForModifierItem(id) {
  // Make an AJAX call to get the modifier group ID list
  $.ajax({
    url: `/Menu/GetModifierGroupIdListByModifierItemId`,
    type: "GET",
    data: { id: id },
    success: function (selectedData) {
      selectedGroupsForAddEdit.clear();

      // Save selected data to a temp variable so it's available inside next ajax call
      const selectedIdsSet = new Set(
        selectedData.map((item) => {
          selectedGroupsForAddEdit.add({ id: item.id, name: item.name });
          return item.id;
        })
      );

      $.ajax({
        type: "GET",
        url: "/Menu/GetModifierGroupListData",
        success: function (allGroups) {
          console.log(allGroups);

          $("#modifierGroupDropdownForAddEdit").html("");
          allGroups.forEach((m) => {
            const isChecked = selectedIdsSet.has(m.modifiergroupId)
              ? "checked"
              : "";
            $("#modifierGroupDropdownForAddEdit").append(`
              <div class="form-check">
                <input type="checkbox" class="form-check-input modifier-group-checkbox" 
                    id="modifierGroup_${m.modifiergroupId}" 
                    value="${m.modifiergroupId}" 
                    data-name="${m.name}"
                    ${isChecked}>
                <label class="form-check-label" for="modifierGroup_${m.modifiergroupId}">${m.name}</label>
              </div>`);
          });

          // Update names display
          updateSelectedNamesForAddEdit();
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
}

// Update the input box with the selected names
function updateSelectedNamesForAddEdit() {
  let names = Array.from(selectedGroupsForAddEdit)
    .map((item) => item.name)
    .join(", ");
  if (names.length === 0) {
    names = "Select Modifier Group";
  }
  $("#modifierGroupSelectBoxForAddEdit").text(names);
}

// Hide the dropdown when clicking outside for edit
$(document).on("click", function (e) {
  if (
    !$(e.target).closest(
      "#modifierGroupSelectBoxForAddEdit, #modifierGroupDropdownForAddEdit"
    ).length
  ) {
    $("#modifierGroupDropdownForAddEdit").hide();
  }
});

// Toggle the visibility of the dropdown

$(document).on("click", "#modifierGroupSelectBoxForAddEdit", function () {
  $("#modifierGroupDropdownForAddEdit").toggle();
});

// Update selected group names when checkboxes are changed
$(document).on("change", ".modifier-group-checkbox", function () {
  const groupId = $(this).val();
  const groupName = $(this).data("name");

  if ($(this).is(":checked")) {
    selectedGroupsForAddEdit.add({ id: groupId, name: groupName });
  } else {
    selectedGroupsForAddEdit.forEach((item) => {
      if (item.id == groupId) {
        selectedGroupsForAddEdit.delete(item);
      }
    });
  }

  // remove validation span if user check any checkbox
  if (selectedGroupsForAddEdit.size > 0) {
    $("#selectModifierGroupValidationForEdit").addClass("d-none");
  }

  updateSelectedNamesForAddEdit();
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

$(document).on("submit", "#AddEditModifierItemForm", function (e) {
  e.preventDefault();

  // if (!validateFormAddModifierItem()) {
  //   return;
  // }

  const selectedModifierGroups = $(
    "#modifierGroupDropdownForAddEdit input:checked"
  ).length;

  console.log("hellloo", selectedModifierGroups);

  if (selectedModifierGroups == 0) {
    $("#selectModifierGroupValidationForEdit").removeClass("d-none");

    return;
  }

  if (!$(this).valid()) {
    return;
  }

  var formData = new FormData(this);

  var selectedModGroupId = Array.from(selectedGroupsForAddEdit).map(
    (item) => item.id
  );

  formData.append("ModifierGroupid", JSON.stringify(selectedModGroupId));

  $.ajax({
    url: "/Menu/AddEditModifierItem",
    type: "POST",
    processData: false,
    contentType: false,
    data: formData,
    success: function (response) {
      // close the opened  modal

      var Modal = bootstrap.Modal.getInstance(
        document.getElementById("AddEditModifierItemModal")
      );
      Modal.hide();

      if (response.success) {
        var page = $("#modifieritem-pagination-section").data("page");

        let mod_id = $("#modifier-list .category-active-option").attr(
          "modifiergroup-id"
        );
        loadmodifieritems(mod_id, page?.pageSize, page?.currentPage);

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

//===============================================

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

// set delete modifier itemid to delete modal
function setDeleteModifierid(element) {
  var modifiergroupid = element.getAttribute("modifiergroup-id");
  var modifierid = element.getAttribute("modifier-id");
  var deleteBtn = document.getElementById("deleteModifierItemBtn");
  deleteBtn.setAttribute("modifiergroup-id", modifiergroupid);
  deleteBtn.setAttribute("modifieritem-id", modifierid);
}

$(document).ready(function () {
  document
    .getElementById("modifieritems-search-field")
    .addEventListener("keyup", () => {
      ModifieritemsPaginationAjax();
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
          loadmodifieritems(mod_id, page?.pageSize, page?.currentPage);

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
