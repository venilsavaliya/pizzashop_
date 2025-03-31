//clear the modifier list and the partial view in the add modifier group modal
function ClearSectionForAddModifierGroup() {
  $("#selectedModifieritemcontainer").html("");
  selectedModifierItemsForAddExistingModifierForAddModal = [];
  $(".modifieritemcheckboxofaddmodal").prop("checked", false);
  $("#modifieritem_main_checkbox_addmodal").prop("checked", false);
}



//clear the modifier list and the partial view in the add modifier group modal
function ClearSectionForEditModifierGroup() {
  $("#selectedModifieritemcontainerforedit").html("");
  selectedModifierItemsForAddExistingModifier = [];
  $(".modifieritemcheckboxofmodal").prop("checked", false);
  $("#modifier_main_checkbox_modal").prop("checked", false);
}
// List of selected modifier item for add modifier group modal
let selectedModifierItemsForAddExistingModifierForAddModal = [];

// handle check box selection in add existing modifier item for add modal
function attachCheckboxListnerModifierItemForAddModal() {
  // Event listener for main checkbox
  $(document).on("change", "#modifieritem_main_checkbox_addmodal", function () {
    let isChecked = this.checked;
    $(".modifieritemcheckboxofaddmodal").prop("checked", isChecked);

    if (isChecked) {
      $(".modifieritemcheckboxofaddmodal").each(function () {
        let id = $(this).val();
        if (
          !selectedModifierItemsForAddExistingModifierForAddModal.includes(
            parseInt(id)
          )
        ) {
          selectedModifierItemsForAddExistingModifierForAddModal.push(
            parseInt(id)
          );
        }
      });
    } else {
      $(".modifieritemcheckboxofaddmodal").each(function () {
        let id = parseInt($(this).val());
        // Remove the unchecked item from the array
        selectedModifierItemsForAddExistingModifierForAddModal =
          selectedModifierItemsForAddExistingModifierForAddModal.filter(
            (item) => item != id
          );
      });
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".modifieritemcheckboxofaddmodal", function () {
    let id = $(this).val();
    if (this.checked) {
      if (
        !selectedModifierItemsForAddExistingModifierForAddModal.includes(
          parseInt(id)
        )
      ) {
        selectedModifierItemsForAddExistingModifierForAddModal.push(
          parseInt(id)
        );
      }
    } else {
      selectedModifierItemsForAddExistingModifierForAddModal =
        selectedModifierItemsForAddExistingModifierForAddModal.filter(
          (item) => item !== id
        );
    }

    // Update main checkbox state
    const allChecked =
      $(".modifieritemcheckboxofaddmodal").length ===
        $(".modifieritemcheckboxofaddmodal:checked").length &&
      $(".modifieritemcheckboxofaddmodal:checked").length !== 0;
    $("#modifieritem_main_checkbox_addmodal").prop("checked", allChecked);
  });

  // Restore previously selected checkboxes
  $(".modifieritemcheckboxofaddmodal").each(function () {
    if (
      selectedModifierItemsForAddExistingModifierForAddModal.includes(
        $(this).val()
      )
    ) {
      $(this).prop("checked", true);
    }
  });

  // Update main checkbox state initially
  const allChecked =
    $(".modifieritemcheckboxofaddmodal").length ===
      $(".modifieritemcheckboxofaddmodal:checked").length &&
    $(".modifieritemcheckboxofaddmodal:checked").length !== 0;
  $("#modifieritem_main_checkbox_addmodal").prop("checked", allChecked);
}

// add partial view of the  selected modifier item in add modifier group modal

function handleAddButtonClickForAdd() {
  console.log(
    "Selected IDs:",
    selectedModifierItemsForAddExistingModifierForAddModal
  );

  if (selectedModifierItemsForAddExistingModifierForAddModal.length == 0) {
    toastr.warning("Please Select Modifier Item!");
    return;
  }

  //clear section so that only selected items will shown avoid duplicates
  $("#selectedModifieritemcontainer").html("");

  // Perform further actions like submitting selectedModifiers to the server

  var itemlistmodal = bootstrap.Modal.getInstance(
    document.getElementById("modifieritemslistforaddgroup")
  );
  itemlistmodal.hide();

  selectedModifierItemsForAddExistingModifierForAddModal.forEach(function (
    modifierId
  ) {
    $.ajax({
      url: `/Menu/GetModifierItemsNamePVByModifieritemId`, // Update with your actual endpoint
      type: "GET",
      data: { modifier_id: modifierId },
      success: function (modifierData) {
        let $partialView = $(modifierData);

        $partialView.find(".delete-modifier-item").on("click", function () {
          let itemId = $(this).attr("item-id");
          console.log("itemid", itemId);
          $partialView.remove();
          selectedModifierItemsForAddExistingModifierForAddModal =
            selectedModifierItemsForAddExistingModifierForAddModal.filter(
              (i) => i != itemId
            );
          console.log(
            "Updated list after deletion:",
            selectedModifierItemsForAddExistingModifierForAddModal
          );
          checkSelectedCheckboxesforAdd();
        });
        console.log("succes");

        $("#selectedModifieritemcontainer").append($partialView);
      },
      error: function (error) {
        console.error(
          `Error fetching details for Modifier ID ${modifierId}:`,
          error
        );
      },
    });

    var modifiermodal = bootstrap.Modal.getInstance(
      document.getElementById("addmodifiergroupmodal")
    );
    modifiermodal.show();
  });
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
          url: `/Menu/GetModifierItemsNamePVByModifieritemId`, // Update with your actual endpoint
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
  // Event listener for main checkbox
  $(document).on("change", "#modifier_main_checkbox_modal", function () {
    let isChecked = this.checked;
    $(".modifieritemcheckboxofmodal").prop("checked", isChecked);

    if (isChecked) {
      $(".modifieritemcheckboxofmodal").each(function () {
        let id = $(this).val();
        if (
          !selectedModifierItemsForAddExistingModifier.includes(parseInt(id))
        ) {
          selectedModifierItemsForAddExistingModifier.push(parseInt(id));
        }
      });
    } else {
      $(".modifieritemcheckboxofmodal").each(function () {
        let id = parseInt($(this).val());
        // Remove the unchecked item from the array
        selectedModifierItemsForAddExistingModifier =
          selectedModifierItemsForAddExistingModifier.filter(
            (item) => item != id
          );
      });
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".modifieritemcheckboxofmodal", function () {
    let id = $(this).val();
    if (this.checked) {
      if (!selectedModifierItemsForAddExistingModifier.includes(parseInt(id))) {
        selectedModifierItemsForAddExistingModifier.push(parseInt(id));
      }
    } else {
      selectedModifierItemsForAddExistingModifier =
        selectedModifierItemsForAddExistingModifier.filter(
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
    if (selectedModifierItemsForAddExistingModifier.includes($(this).val())) {
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
function handleAddButtonClick() {
  console.log("hellokjfisdfiehfiuewhufehdfuwehfuihfufhiuewfhiwefd");
  if (selectedModifierItemsForAddExistingModifier.length == 0) {
    toastr.warning("Please Select Modifier Item!");
    return;
  }

  // Perform further actions like submitting selectedModifiers to the server
  var itemlistmodal = bootstrap.Modal.getInstance(
    document.getElementById("modifieritemslist")
  );
  itemlistmodal.hide();

  selectedModifierItemsForAddExistingModifier.forEach(function (modifierId) {
    $.ajax({
      url: `/Menu/GetModifierItemsNamePVByModifieritemId`, // Update with your actual endpoint
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
          checkSelectedCheckboxes();
        });

        var modifiermodal = bootstrap.Modal.getInstance(
          document.getElementById("editmodifiergroupmodal")
        );
        modifiermodal.show();
        $("#selectedModifieritemcontainerforedit").append($partialView);
        // updateCheckboxStates();
        attachCheckboxListnerModifierItemForEditModal();
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

$(document).ready(function () {
  attachHoverEffect();

  // Function to handle hover effect
  function attachHoverEffect() {
    document.querySelectorAll(".modifier-option").forEach((opt) => {
      opt.addEventListener("mouseover", function () {
        console.log("Mouse Entered");
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

  // showing and hiding modal manually

  document
    .getElementById("addexistingmodifierbtn")
    .addEventListener("click", () => {
      var modifiermodal = bootstrap.Modal.getInstance(
        document.getElementById("addmodifiergroupmodal")
      );
      modifiermodal.hide();
      console.log(
        "opening ..",
        selectedModifierItemsForAddExistingModifierForAddModal
      );
      attachCheckboxListnerModifierItemForAddModal();
      var itemlistmodal = new bootstrap.Modal(
        document.getElementById("modifieritemslistforaddgroup")
      );
      itemlistmodal.show();
    });

  // Add modifier Group Form Validation

  function validateFormAddModifierGroup() {
    let isValid = true;

    const itemName = $("#addmodifiergroupname").val();

    if (!itemName) {
      isValid = false;
    }

    return isValid;
  }

  // submit the add modifier group form

  $("#addModifierGroupForm").submit(function (e) {
    e.preventDefault();

    if(!validateFormAddModifierGroup())
    {
      return;
    }

    var formData = new FormData(this);

    console.log("he", selectedModifierItemsForAddExistingModifierForAddModal);

    formData.append(
      "ModifierItems",
      JSON.stringify(selectedModifierItemsForAddExistingModifierForAddModal)
    );

    $.ajax({
      url: "/Menu/AddModifierGroup",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("addmodifiergroupmodal")
        );
        Modal.hide();

        if (response.success) {
          //get the active category id
          let modgroup_id = $("#modifier-list .category-active-option").attr(
            "modifiergroup-id"
          );
          loadmodifiers(modgroup_id);
          ClearSectionForAddModifierGroup();
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

  // edit modifier Group Form Validation

  function validateFormeditModifierGroup() {
    let isValid = true;

    const itemName = $("#editmodifiergroupname").val();

    if (!itemName) {
      isValid = false;
    }

    return isValid;
  }


  //submit the edit modifier group form

  $("#editmodifierForm").submit(function (e) {
    e.preventDefault();

    if(!validateFormeditModifierGroup())
    {
      return;
    }

    var formData = new FormData(this);

    formData.append(
      "ModifierItems",
      JSON.stringify(selectedModifierItemsForAddExistingModifier)
    );

    $.ajax({
      url: "/Menu/EditModifierGroup",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("editmodifiergroupmodal")
        );
        Modal.hide();

        if (response.success) {
          //get the active category id
          let modgroup_id = $("#modifier-list .category-active-option").attr(
            "modifiergroup-id"
          );
          loadmodifiers(modgroup_id);
          ClearSectionForEditModifierGroup();
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
