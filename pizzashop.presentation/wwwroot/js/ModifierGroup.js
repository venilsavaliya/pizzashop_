let selectedModifierItemsForExistingModifier = [];
let ModifierItemsAddExistingModifierTempList = [];

// pagination for modifier item list of add existing modifier
function ModifieritemListForAddExistingPaginationAjax(pageSize, pageNumber) {
  // Get the dropdown element
  let pageSizeDropdown = document.getElementById("pageSizeDropdownformodal");
  let searchkeyword = $("#modifieritemsforaddmodal-search-field").val();

  // Store the currently selected value
  let selectedPageSize = pageSizeDropdown?.value;

  $.ajax({
    url: "/Menu/GetAllModifierItemsListForAdd",
    data: {
      pageSize: pageSize,
      pageNumber: pageNumber,
      searchKeyword: searchkeyword,
    },
    type: "GET",
    dataType: "html",
    success: function (data) {
      $("#modifierItemTableContainerForModal").html(data);
      checkSelectedCheckboxesforAddExistingModifierList();
      attachCheckboxListnerModifierItemForAddModal();
    },
    error: function () {
      alert("No Projects Found");

      $("#modifierItemTableContainerForModal").html("An error has occurred");
    },
  });
}

// open Add Edit Modifier Group Modal

function openAddEditModifierGroupModal(id) {
  ClearListOfModifierItems();
  $.ajax({
    type: "GET",
    url: "/Menu/GetAddEditModifierGroupForm",
    data: { id: id },
    success: function (data) {
      $("#addEditModifierGroupFormContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("addEditModifierGroupModal")
      );
      modal.show();

      // This Will Load Partial View For Existing ModItem of ModGroup
      if (id) {
        GetModifierItemListForEdit(id);
      }
    },
  });
}

// for performing action on clicking add existing modifier 
$(document).on("click", "#addExistingModifierBtn", function () {
  var modal = new bootstrap.Modal(
    document.getElementById("modifierItemLIstForAddExistingModifierModal")
  );
  modal.show();

  // clear search field and load first page of modifier items
  $("#modifieritemsforaddmodal-search-field").val('');
  ModifieritemListForAddExistingPaginationAjax();
});

// for performing action on closing add existing modifier 
function closeAddExistingModifierItemModal() {
  var modal = bootstrap.Modal.getInstance(
    document.getElementById("modifierItemLIstForAddExistingModifierModal")
  );
  modal.hide();
  ModifierItemsAddExistingModifierTempList = [];
}

//111
$(document).on(
  "click",
  "modifierItemLIstForAddExistingModifierModal",
  function () {
    ModifierItemsAddExistingModifierTempList = [];
    console.log("temp list clear", ModifierItemsAddExistingModifierTempList);
  }
);

// function for getting existing mod item partial view for modifergroup
function GetModifierItemListForEdit(modifiergroupid) {
  //this will empty the all section before appending partial views
  selectedModifierItemsForExistingModifier = [];

  $.ajax({
    url: `/Menu/GetModifierItemsidByModifierGroupid`,
    type: "GET",
    data: { modifiergroup_id: modifiergroupid },
    success: function (data) {
      console.log("Modifier Items:", data);

      data.forEach(function (id) {
        selectedModifierItemsForExistingModifier.push(id);
      });

      selectedModifierItemsForExistingModifier.forEach(function (modifierId) {
        $.ajax({
          url: `/Menu/GetModifierItemsNamePVByModifieritemId`,
          type: "GET",
          data: { modifier_id: modifierId },
          success: function (modifierData) {
            let $partialView = $(modifierData);

            $partialView.find(".delete-modifier-item").on("click", function () {
              let itemId = $(this).attr("item-id");
              console.log("itemid", itemId);
              $partialView.remove();
              selectedModifierItemsForExistingModifier =
                selectedModifierItemsForExistingModifier.filter(
                  (i) => i != itemId
                );
              console.log(
                "Updated list after deletion:",
                selectedModifierItemsForExistingModifier
              );
            });

            $("#selectedModifieritemcontainerForAddEdit").append($partialView);
          },
          error: function (error) {
            console.error(
              `Error fetching details for Modifier ID ${modifierId}:`,
              error
            );
          },
        });
      });

      // checkSelectedCheckboxes();
    },
    error: function (error) {
      console.error("Error fetching modifiers:", error);
    },
  });
}

// function to keep checkbox cheked if id existed in list

function checkSelectedCheckboxesforAddExistingModifierList() {
  // Uncheck all checkboxes first
  $(".modifierItemListInnerCheckbox").prop("checked", false);
  $(".modifieritemlist_main_checkbox").prop("checked", false);

  // Check only the ones that match IDs in selectedModifierItemsForAddExistingModifier
  ModifierItemsAddExistingModifierTempList.forEach((id) => {
    $(`.modifierItemListInnerCheckbox[value='${id}']`).prop("checked", true);
  });
}


$(document).on('submit',"#addEditModifierForm",function(e){
  e.preventDefault();

  if (!$(this).valid()) {
    return;
  }

  var formData = new FormData(this);

  formData.append(
    "ModifierItems",
    JSON.stringify(selectedModifierItemsForExistingModifier)
  );

  $.ajax({
    url: "/Menu/AddEditModifierGroup",
    type: "POST",
    processData: false,
    contentType: false,
    data: formData,
    success: function (response) {
      var Modal = bootstrap.Modal.getInstance(
        document.getElementById("addEditModifierGroupModal")
      );
      Modal.hide();

      if (response.success) {
        //get the active category id
        let modgroup_id = $("#modifier-list .category-active-option").attr(
          "modifiergroup-id"
        );
        loadmodifiers(modgroup_id);
        // ClearSectionForEditModifierGroup();
        toastr.success(response.message);
      } else {
        toastr.error(response.message);
      }
    },
    error: function (err) {
      console.error("Error adding item:", err);
    },
  });
})

// ==========================================================================================

// load partial views of selected item for add modifier group modal

function SetPartialViewOfSelectedModifierItemsForAddModal() {
  $("#selectedModifieritemcontainerForAddEdit").html("");
  // for showing partial view of selected modifier item
  selectedModifierItemsForExistingModifier.forEach(function (
    modifierId
  ) {
    $.ajax({
      url: `/Menu/GetModifierItemsNamePVByModifieritemId`,
      type: "GET",
      data: { modifier_id: modifierId },
      success: function (modifierData) {
        let $partialView = $(modifierData);

        $partialView.find(".delete-modifier-item").on("click", function () {
          let itemId = $(this).attr("item-id");
          console.log("itemid", itemId);
          $partialView.remove();
          selectedModifierItemsForExistingModifier =
            selectedModifierItemsForExistingModifier.filter(
              (i) => i != itemId
            );
          console.log(
            "Updated list after deletion:",
            selectedModifierItemsForExistingModifier
          );
          checkSelectedCheckboxesforAdd();
        });
        console.log("succes");

        $("#selectedModifieritemcontainerForAddEdit").append($partialView);
      },
      error: function (error) {
        console.error(
          `Error fetching details for Modifier ID ${modifierId}:`,
          error
        );
      },
    });
  });
}
// load partial views of selected item for edit modifier group modal

// function SetPartialViewOfSelectedModifierItemsForEditModal() {
//   $("#selectedModifieritemcontainer").html("");
//   selectedModifierItemsForAddExistingModifier.forEach(function (modifierId) {
//     $.ajax({
//       url: `/Menu/GetModifierItemsNamePVByModifieritemId`, // Update with your actual endpoint
//       type: "GET",
//       data: { modifier_id: modifierId },
//       success: function (modifierData) {
//         let $partialView = $(modifierData);

//         $partialView.find(".delete-modifier-item").on("click", function () {
//           let itemId = $(this).attr("item-id");
//           console.log("itemid", itemId);
//           $partialView.remove();
//           selectedModifierItemsForAddExistingModifier =
//             selectedModifierItemsForAddExistingModifier.filter(
//               (i) => i != itemId
//             );
//           console.log(
//             "Updated list after deletion:",
//             selectedModifierItemsForAddExistingModifier
//           );
//           checkSelectedCheckboxes();
//         });
//         $("#selectedModifieritemcontainer").append($partialView);
//         attachCheckboxListnerModifierItemForEditModal();
//       },
//       error: function (error) {
//         console.error(
//           `Error fetching details for Modifier ID ${modifierId}:`,
//           error
//         );
//       },
//     });
//   });
// }

//clear the modifier list and the partial view in the add modifier group modal
// function ClearSectionForEditModifierGroup() {
//   $("#selectedModifieritemcontainerforedit").html("");
//   selectedModifierItemsForAddExistingModifier = [];
//   selectedModifierItemsForAddExistingModifierTemp = [];
//   console.log("clear", selectedModifierItemsForAddExistingModifierTemp);
//   $(".modifieritemcheckboxofmodal").prop("checked", false);
//   $("#modifier_main_checkbox_modal").prop("checked", false);
// }
// List of selected modifier item for add modifier group modal
// let selectedModifierItemsForAddExistingModifierForAddModal = [];
// let selectedModifierItemsForAddExistingModifierForAddModalTemp = [];

// handle check box selection in add existing modifier item 
function attachCheckboxListnerModifierItemForAddModal() {
  if (ModifierItemsAddExistingModifierTempList.length == 0) {
    ModifierItemsAddExistingModifierTempList = [
      ...selectedModifierItemsForExistingModifier,
    ];
  }

  // Event listener for main checkbox
  $(document).on("change", "#modifieritemlist_main_checkbox", function () {
    let isChecked = this.checked;
    $(".modifierItemListInnerCheckbox").prop("checked", isChecked);

    if (isChecked) {
      $(".modifierItemListInnerCheckbox").each(function () {
        let id = $(this).val();
        if (!ModifierItemsAddExistingModifierTempList.includes(parseInt(id))) {
          ModifierItemsAddExistingModifierTempList.push(parseInt(id));
        }
      });
    } else {
      $(".modifierItemListInnerCheckbox").each(function () {
        let id = parseInt($(this).val());
        // Remove the unchecked item from the array
        ModifierItemsAddExistingModifierTempList =
          ModifierItemsAddExistingModifierTempList.filter((item) => item != id);
      });
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".modifierItemListInnerCheckbox", function () {
    let id = $(this).val();
    if (this.checked) {
      if (!ModifierItemsAddExistingModifierTempList.includes(parseInt(id))) {
        ModifierItemsAddExistingModifierTempList.push(parseInt(id));
      }
    } else {
      ModifierItemsAddExistingModifierTempList =
        ModifierItemsAddExistingModifierTempList.filter((item) => item != id);
    }

    // Update main checkbox state
    const allChecked =
      $(".modifierItemListInnerCheckbox").length ===
        $(".modifierItemListInnerCheckbox:checked").length &&
      $(".modifierItemListInnerCheckbox:checked").length !== 0;
    $("#modifieritemlist_main_checkbox").prop("checked", allChecked);
    console.log("Selected Items:", ModifierItemsAddExistingModifierTempList);
    console.log("all checked:", $(".modifierItemListInnerCheckbox").length,$(".modifierItemListInnerCheckbox:checked").length);
  });

  // Restore previously selected checkboxes
  $(".modifierItemListInnerCheckbox").each(function () {
    if (ModifierItemsAddExistingModifierTempList.includes(parseInt($(this).val()))) {
      $(this).prop("checked", true);
    }
  });
  
  $(".modifierItemListInnerCheckbox").each(function (index) {
    console.log(`DOM location ${index + 1}:`, this, "Parent:", $(this).closest("*"));
  });
  // Update main checkbox state initially
  const allChecked =
  $(".modifierItemListInnerCheckbox").length ===
    $(".modifierItemListInnerCheckbox:checked").length &&
  $(".modifierItemListInnerCheckbox:checked").length !== 0;

  $("#modifieritemlist_main_checkbox").prop("checked", allChecked);


}

// add partial view of the  selected modifier item in add modifier group modal

function handleAddButtonClickForAdd() {
  console.log(
    "Selected IDs:",
    selectedModifierItemsForExistingModifier
  );

  selectedModifierItemsForExistingModifier = [
    ...ModifierItemsAddExistingModifierTempList,
  ];
  ModifierItemsAddExistingModifierTempList = [];

  if (selectedModifierItemsForExistingModifier.length == 0) {
    toastr.warning("Please Select Modifier Item!");
    return;
  }

  //clear section so that only selected items will shown avoid duplicates
  $("#selectedModifieritemcontainer").html("");

  // Perform further actions like submitting selectedModifiers to the server

  var itemlistmodal = bootstrap.Modal.getInstance(
    document.getElementById("modifierItemLIstForAddExistingModifierModal")
  );
  itemlistmodal.hide();

  // set partial view of selected modifier items in add modifier group modal
  SetPartialViewOfSelectedModifierItemsForAddModal();
  // var modifiermodal = bootstrap.Modal.getInstance(
  //   document.getElementById("addmodifiergroupmodal")
  // );
  // modifiermodal.show();
}

// load modifier group list
function loadmodifiers(id) {
  selectedModifierItems = [];

  $.ajax({
    type: "GET",
    url: "/Menu/GetModifiers",
    data: { modifiergroup_id: id },
    success: function (data) {
      $("#modifier-list").html(data);
    },
  });

  $.ajax({
    url: "/Menu/GetModifierItemsList",
    type: "GET",
    data: { modifiergroup_id: id },
    success: function (data) {
      $("#modifieritemstablecontainer").html(data);
      attachMassDeleteForModifierItem();
    },
  });
}

// function for opening mass delete modal for menu item

function openMassDeleteModalForModifierItem() {
  if (selectedModifierItems.length == 0) {
    toastr.warning("Please Select Modifier Item!");
    return;
  }
  var deleteModal = new bootstrap.Modal(
    document.getElementById("deletemassitemmodal")
  );
  deleteModal.show();
}

// List of selected modifier item for edit modifier group modal
let selectedModifierItemsForAddExistingModifier = [];
let selectedModifierItemsForAddExistingModifierTemp = [];

// function for load partial view of modifier item name for edit modifier group
function GetModifierItemListByGroupid(ele) {
  $("#selectedModifieritemcontainerforedit").html("");

  selectedModifierItemsForAddExistingModifier = [];
  let obj = JSON.parse(ele.getAttribute("data-obj"));
  let modifiergroupid = obj.modifiergroupId;
  let name = obj.name;
  let description = obj.description;

  document.getElementById("editmodifiergroupid").value = modifiergroupid;
  document.getElementById("editmodifiergroupname").value = name;
  document.getElementById("editmodifiergroupdescription").value = description;

  $.ajax({
    url: `/Menu/GetModifierItemsidByModifierGroupid`,
    type: "GET",
    data: { modifiergroup_id: modifiergroupid },
    success: function (data) {
      console.log("Modifier Items:", data);

      data.forEach(function (id) {
        selectedModifierItemsForAddExistingModifier.push(id);
      });

      console.log("f", selectedModifierItemsForAddExistingModifier);

      selectedModifierItemsForAddExistingModifier.forEach(function (
        modifierId
      ) {
        $.ajax({
          url: `/Menu/GetModifierItemsNamePVByModifieritemId`,
          type: "GET",
          data: { modifier_id: modifierId },
          success: function (modifierData) {
            let $partialView = $(modifierData);

            $partialView.find(".delete-modifier-item").on("click", function () {
              let itemId = $(this).attr("item-id");
              console.log("itemid", itemId);
              $partialView.remove();
              selectedModifierItemsForAddExistingModifier =
                selectedModifierItemsForAddExistingModifier.filter(
                  (i) => i != itemId
                );
              console.log(
                "Updated list after deletion:",
                selectedModifierItemsForAddExistingModifier
              );
            });
            $("#selectedModifieritemcontainerforedit").append($partialView);
          },
          error: function (error) {
            console.error(
              `Error fetching details for Modifier ID ${modifierId}:`,
              error
            );
          },
        });
      });

      checkSelectedCheckboxes();
    },
    error: function (error) {
      console.error("Error fetching modifiers:", error);
    },
  });
}

// function to keep checkbox cheked if id existed in list

function checkSelectedCheckboxes() {
  // Uncheck all checkboxes first
  $(".modifieritemcheckboxofmodal").prop("checked", false);

  // Check only the ones that match IDs in selectedModifierItemsForAddExistingModifier
  selectedModifierItemsForAddExistingModifier.forEach((id) => {
    $(`.modifieritemcheckboxofmodal[value='${id}']`).prop("checked", true);
  });
}

// handle check box selection in add existing modifier item for add modal
function attachCheckboxListnerModifierItemForEditModal() {
  console.log("first", selectedModifierItemsForAddExistingModifierTemp);
  console.log("second", selectedModifierItemsForAddExistingModifier);
  if (selectedModifierItemsForAddExistingModifierTemp.length == 0) {
    selectedModifierItemsForAddExistingModifierTemp = [
      ...selectedModifierItemsForAddExistingModifier,
    ];
  }
  // Event listener for main checkbox
  $(document).on("change", "#modifier_main_checkbox_modal", function () {
    let isChecked = this.checked;
    $(".modifieritemcheckboxofmodal").prop("checked", isChecked);

    if (isChecked) {
      $(".modifieritemcheckboxofmodal").each(function () {
        let id = $(this).val();
        if (
          !selectedModifierItemsForAddExistingModifierTemp.includes(
            parseInt(id)
          )
        ) {
          selectedModifierItemsForAddExistingModifierTemp.push(parseInt(id));
        }
      });
    } else {
      $(".modifieritemcheckboxofmodal").each(function () {
        let id = parseInt($(this).val());
        // Remove the unchecked item from the array
        selectedModifierItemsForAddExistingModifierTemp =
          selectedModifierItemsForAddExistingModifierTemp.filter(
            (item) => item != id
          );
      });
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".modifieritemcheckboxofmodal", function () {
    let id = $(this).val();
    if (this.checked) {
      if (
        !selectedModifierItemsForAddExistingModifierTemp.includes(parseInt(id))
      ) {
        selectedModifierItemsForAddExistingModifierTemp.push(parseInt(id));
      }
    } else {
      selectedModifierItemsForAddExistingModifierTemp =
        selectedModifierItemsForAddExistingModifierTemp.filter(
          (item) => item !== id
        );
    }

    // Update main checkbox state
    const allChecked =
      $(".modifieritemcheckboxofmodal").length ===
        $(".modifieritemcheckboxofmodal:checked").length &&
      $(".modifieritemcheckboxofmodal:checked").length !== 0;
    $("#modifier_main_checkbox_modal").prop("checked", allChecked);
  });

  // Restore previously selected checkboxes
  $(".modifieritemcheckboxofmodal").each(function () {
    if (
      selectedModifierItemsForAddExistingModifierTemp.includes($(this).val())
    ) {
      $(this).prop("checked", true);
    }
  });

  // Update main checkbox state initially
  const allChecked =
    $(".modifieritemcheckboxofmodal").length ===
      $(".modifieritemcheckboxofmodal:checked").length &&
    $(".modifieritemcheckboxofmodal:checked").length !== 0;
  $("#modifier_main_checkbox_modal").prop("checked", allChecked);
}

//set delete category url to the delete category modal yes button

function setDeleteModifierGroupId(ele) {
  let id = ele.getAttribute("data-id");
  let deleteBtn = document.getElementById("deleteModifierGroupBtn");
  deleteBtn.setAttribute("modifiergroup-id", id);
}

// Function to check checkboxes based on selectedModifiers
// function updateCheckboxStates() {
//   console.log(selectedModifierItemsForAddExistingModifier);
//   document
//     .querySelectorAll(".modifieritemcheckboxofmodal")
//     .forEach((checkbox) => {
//       if (
//         selectedModifierItemsForAddExistingModifier.includes(
//           parseInt(checkbox.value)
//         )
//       ) {
//         checkbox.checked = true;
//       } else {
//         checkbox.checked = false;
//       }
//     });
// }

// add partial view of the  selected modifier item in edit modifier group modal
// function handleAddButtonClick() {
//   console.log("add click");
//   console.log(selectedModifierItemsForAddExistingModifierTemp);
//   selectedModifierItemsForAddExistingModifier = [
//     ...selectedModifierItemsForAddExistingModifierTemp,
//   ];
//   console.log(selectedModifierItemsForAddExistingModifier);
//   selectedModifierItemsForAddExistingModifierTemp = [];
//   if (selectedModifierItemsForAddExistingModifier.length == 0) {
//     toastr.warning("Please Select Modifier Item!");
//     return;
//   }

  // Perform further actions like submitting selectedModifiers to the server
  var itemlistmodal = bootstrap.Modal.getInstance(
    document.getElementById("modifieritemslist")
  );
  itemlistmodal.hide();
}

// this will make sure list is clear before opening add modifier group modal
function ClearListOfModifierItems() {
 
  selectedModifierItemsForExistingModifier = [];
  ModifierItemsAddExistingModifierTempList = [];

  $(".modifieritemcheckboxofaddmodal").prop("checked", false);
  $("#modifieritem_main_checkbox_addmodal").prop("checked", false);
}

$(document).ready(function () {

  // search
  $("#modifieritemsforaddmodal-search-field").on('keyup',()=>{
    ModifieritemListForAddExistingPaginationAjax();
  })

  attachHoverEffect();

  // Add Modifier Group Form Validation

  // $("#addModifierGroupForm").validate({
  //   rules: {
  //     "ModifierGroup.Name": {
  //       required: true,
  //       minlength: 2,
  //     },
  //   },
  //   messages: {
  //     "ModifierGroup.Name": {
  //       required: "Please enter a Modifier Group name.",
  //       minlength: "Name must be at least 2 characters.",
  //     },
  //   },
  //   errorElement: "span",
  //   errorClass: "text-danger",
  //   highlight: function (element) {
  //     $(element).addClass("is-invalid");
  //   },
  //   unhighlight: function (element) {
  //     $(element).removeClass("is-invalid");
  //   },
  // });

  // Reset Modifier Group Form When Modal Close
  // $("#addmodifiergroupmodal").on("hidden.bs.modal", function () {
  //   // Reset the form inside the modal
  //   $(this).find("form")[0].reset();

  //   // Reset ASP.NET Core validation
  //   var validator = $(this).find("form").validate();
  //   validator.resetForm();

  //   // Remove invalid classes
  //   $(this).find(".is-invalid").removeClass("is-invalid");
  // });

  // submit the add modifier group form

  // $("#addModifierGroupForm").submit(function (e) {
  //   e.preventDefault();

  //   // if (!validateFormAddModifierGroup()) {
  //   //   return;
  //   // }
  //   if (!$(this).valid()) {
  //     return;
  //   }

  //   var formData = new FormData(this);

  //   console.log("he", selectedModifierItemsForExistingModifier);

  //   formData.append(
  //     "ModifierItems",
  //     JSON.stringify(selectedModifierItemsForExistingModifier)
  //   );

  //   $.ajax({
  //     url: "/Menu/AddModifierGroup",
  //     type: "POST",
  //     processData: false,
  //     contentType: false,
  //     data: formData,
  //     success: function (response) {
  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("addmodifiergroupmodal")
  //       );
  //       Modal.hide();

  //       if (response.success) {
  //         //get the active category id
  //         let modgroup_id = $("#modifier-list .category-active-option").attr(
  //           "modifiergroup-id"
  //         );
  //         loadmodifiers(modgroup_id);
  //         ModifierItemsAddExistingModifierTempList = [];
  //         toastr.success(response.message);
  //       } else {
  //         toastr.error(response.message);
  //       }
  //     },
  //     error: function (err) {
  //       console.error("Error adding item:", err);
  //     },
  //   });
  // });

  // Edit Modifier Group Form Validation

  // $("#editmodifierForm").validate({
  //   rules: {
  //     "ModifierGroup.Name": {
  //       required: true,
  //       minlength: 2,
  //     },
  //   },
  //   messages: {
  //     "ModifierGroup.Name": {
  //       required: "Please enter a Modifier Group name.",
  //       minlength: "Name must be at least 2 characters.",
  //     },
  //   },
  //   errorElement: "span",
  //   errorClass: "text-danger",
  //   highlight: function (element) {
  //     $(element).addClass("is-invalid");
  //   },
  //   unhighlight: function (element) {
  //     $(element).removeClass("is-invalid");
  //   },
  // });

  // Reset Edit Modifier Group Form When Modal Close
  // $("#editmodifiergroupmodal").on("hidden.bs.modal", function () {
  //   // Reset the form inside the modal
  //   $(this).find("form")[0].reset();

  //   // Reset ASP.NET Core validation
  //   var validator = $(this).find("form").validate();
  //   validator.resetForm();

  //   // Remove invalid classes
  //   $(this).find(".is-invalid").removeClass("is-invalid");
  // });

  //submit the edit modifier group form

  // $("#editmodifierForm").submit(function (e) {
  //   e.preventDefault();

  //   if (!$(this).valid()) {
  //     return;
  //   }

  //   var formData = new FormData(this);

  //   formData.append(
  //     "ModifierItems",
  //     JSON.stringify(selectedModifierItemsForAddExistingModifier)
  //   );

  //   $.ajax({
  //     url: "/Menu/EditModifierGroup",
  //     type: "POST",
  //     processData: false,
  //     contentType: false,
  //     data: formData,
  //     success: function (response) {
  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("editmodifiergroupmodal")
  //       );
  //       Modal.hide();

  //       if (response.success) {
  //         //get the active category id
  //         let modgroup_id = $("#modifier-list .category-active-option").attr(
  //           "modifiergroup-id"
  //         );
  //         loadmodifiers(modgroup_id);
  //         ClearSectionForEditModifierGroup();
  //         toastr.success(response.message);
  //       } else {
  //         toastr.error(response.message);
  //       }
  //     },
  //     error: function (err) {
  //       console.error("Error adding item:", err);
  //     },
  //   });
  // });

  // Event listener for add modifier group modal
  // $("#modifieritemslist").on("shown.bs.modal", function () {
  //   selectedModifierItemsForAddExistingModifierTemp = [];
  // });
  // $("#modifieritemslistforaddgroup").on("hide.bs.modal", function () {
  //   ModifierItemsAddExistingModifierTempList = [];

  //   SetPartialViewOfSelectedModifierItemsForAddModal();

  //   var Modal = bootstrap.Modal.getInstance(
  //     document.getElementById("addmodifiergroupmodal")
  //   );
  //   Modal.show();
  // });

  // $("#modifieritemslist").on("hide.bs.modal", function () {
  //   selectedModifierItemsForAddExistingModifierTemp = [];

  //   SetPartialViewOfSelectedModifierItemsForEditModal();

  //   var Modal = bootstrap.Modal.getInstance(
  //     document.getElementById("editmodifiergroupmodal")
  //   );
  //   Modal.show();
  // });

  // Function to handle hover effect
  function attachHoverEffect() {
    document.querySelectorAll(".modifier-option").forEach((opt) => {
      opt.addEventListener("mouseover", function () {
   
        let actionbtn = opt.querySelector("#categoryoption_actionbtn");
        if (actionbtn) {
          actionbtn.classList.add("d-block");
          actionbtn.classList.remove("d-none");
        }
      });

      opt.addEventListener("mouseleave", function () {
        let actionbtn = opt.querySelector("#categoryoption_actionbtn");
        if (actionbtn) {
          actionbtn.classList.remove("d-block");
          actionbtn.classList.add("d-none");
        }
      });
    });
  }

  // If the partial view is loaded dynamically, reattach events
  $(document).on("ajaxComplete", function () {
    attachHoverEffect();
  });

  // clear the selected modifier item list when the add modifier group modal is closed
  //change1
  // $("#addmodifiergroupmodal").on("hide.bs.modal", function () {
  //   $("#selectedModifieritemcontainer").html("");
  // });

  // showing and hiding add modifier group modal manually
  // document
  //   .getElementById("addexistingmodifierbtn")
  //   .addEventListener("click", () => {
  //     var modifiermodal = bootstrap.Modal.getInstance(
  //       document.getElementById("addmodifiergroupmodal")
  //     );
  //     modifiermodal.hide();

  //     // to show modifier item from the first page
  //     ModifieritemListForAddPaginationAjax();
  //     attachCheckboxListnerModifierItemForAddModal();
  //     var itemlistmodal = new bootstrap.Modal(
  //       document.getElementById("modifieritemslistforaddgroup")
  //     );
  //     itemlistmodal.show();
  //   });

  // showing and hiding edit modifier group modal manually
  // document
  //   .getElementById("addexistingmodifierbtnforedit")
  //   .addEventListener("click", () => {
  //     var modifiermodal = bootstrap.Modal.getInstance(
  //       document.getElementById("editmodifiergroupmodal")
  //     );
  //     modifiermodal.hide();
  //     // to show modifier item from the first page
  //     PaginationAjax();
  //     attachCheckboxListnerModifierItemForEditModal();
  //     checkSelectedCheckboxes();
  //     var itemlistmodal = new bootstrap.Modal(
  //       document.getElementById("modifieritemslist")
  //     );
  //     itemlistmodal.show();
  //   });

  // Delete Modifier Group Modal

  $("#deleteModifierGroupBtn").click(function (e) {
    var modgroupid = $(this).attr("modifiergroup-id").toString();
    $.ajax({
      url: "/Menu/DeleteModifierGroupById",
      method: "POST",
      data: {
        id: modgroupid,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deleteModifierGroupmodal")
        );
        deleteModal.hide();

        if (response.success) {
          //get the active category id
          let mod_id = $("#modifier-list .category-active-option").attr(
            "modifiergroup-id"
          );
          mod_id == modgroupid ? loadmodifiers() : loadmodifiers(mod_id); // if current modgropup id is deleted than by default select first modgroup

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
