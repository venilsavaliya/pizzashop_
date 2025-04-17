// mass delete checbox selection fuctionality for menu item

let selectedMenuItems = [];

function attachMassDeleteForMenuItem() {
  // Event listener for main checkbox
  $(document).on("change", "#menu-main-checkbox", function () {
    let isChecked = this.checked;
    $(".inner_checkbox_menuitem").prop("checked", isChecked);

    if (isChecked) {
      $(".inner_checkbox_menuitem").each(function () {
        let id = $(this).val();
        if (!selectedMenuItems.includes(id)) {
          selectedMenuItems.push(id);
        }
      });
    } else {
      selectedMenuItems = [];
    }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".inner_checkbox_menuitem", function () {
    let id = $(this).val();
    if (this.checked) {
      if (!selectedMenuItems.includes(id)) {
        selectedMenuItems.push(id);
      }
    } else {
      selectedMenuItems = selectedMenuItems.filter((item) => item !== id);
    }

    // Update main checkbox state
    const allChecked =
      $(".inner_checkbox_menuitem").length ===
        $(".inner_checkbox_menuitem:checked").length &&
      $(".inner_checkbox_menuitem:checked").length !== 0;
    $("#menu-main-checkbox").prop("checked", allChecked);
  });

  // Restore previously selected checkboxes
  $(".inner_checkbox_menuitem").each(function () {
    if (selectedMenuItems.includes($(this).val())) {
      $(this).prop("checked", true);
    }
  });

  // Update main checkbox state initially
  const allChecked =
    $(".inner_checkbox_menuitem").length ===
      $(".inner_checkbox_menuitem:checked").length &&
    $(".inner_checkbox_menuitem:checked").length !== 0;
  $("#menu-main-checkbox").prop("checked", allChecked);
}

// function for opening mass delete modal for menu item

function openMassDeleteModalForMenuItem() {
  if (selectedMenuItems.length == 0) {
    toastr.warning("Please Select Menu Item!");
    return;
  }
  var deleteModal = new bootstrap.Modal(
    document.getElementById("deletemassmenuitemmodal")
  );
  deleteModal.show();
}

//setting delete id attribute in modal
function setDeleteItemId(element) {
  var Id = element.getAttribute("item-id");
  let cat_id = $("#category-list .category-active-option").attr("category-id");
  var deleteBtn = document.getElementById("deleteitemBtn");
  deleteBtn.setAttribute("category-id", cat_id);
  deleteBtn.setAttribute("item-id", Id);
}

// Edit menu Item functionality
let selectedModifierGroupsforedit = [];
function setedititemdata(ele) {
  let modifiergroupids = [];
  selectedModifierGroupsforedit = [];
  var c = JSON.parse(ele.getAttribute("item-obj"));
  var catid = ele.getAttribute("category-id");
  console.log("edititem",c);

  var editmenuitem = document.getElementById("editmenuitem");
  editmenuitem.querySelector("#CategoryForEdit").value = catid;
  editmenuitem.querySelector("#ItemRateForEdit").value = c.rate;
  editmenuitem.querySelector("#itemtypeforedit").value = c.type;
  editmenuitem.querySelector("#Isavailableforedit").checked = c.isavailable
    ? true
    : false;
  editmenuitem.querySelector("#itemnameforedit").value = c.itemName;
  editmenuitem.querySelector("#ItemQuantityForEdit").value = c.quantity;
  editmenuitem.querySelector("#ItemUnitForEdit").value = c.unit;
  editmenuitem.querySelector("#itemTaxPercentageForEdit").value =
    c.taxPercentage;
  editmenuitem.querySelector("#ShortCodeforedit").value = c.shortCode;
  editmenuitem.querySelector("#Descriptionforedit").value = c.description;
  $("#fileNameSpanForEdit").text(c.image.substring(17,25)+'.jpg');
  editmenuitem.querySelector("#DefaultTaxforedit").checked = c.defaultTax
    ? true
    : false;
  editmenuitem.querySelector("#itemId").value = c.itemId;

  console.log("id", c.itemId);
  console.log("image", c.image);

  $("#modifieritemspartialviewforedit").html("");

  $.ajax({
    url: "/Menu/GetModifierGroupIdsByItemId",
    type: "GET",
    data: { itemid: c.itemId },
    success: function (response) {
      console.log("edit item modifier groups", response);
      modifiergroupids = response;

      // Move the forEach inside the AJAX success function
      modifiergroupids.forEach(function (groupId) {
        $.ajax({
          url: "/Menu/GetModifierItemsForEdit",
          type: "GET",
          data: { modifierGroupId: groupId, itemid: c.itemId },
          success: function (response) {
            let $partialView = $(response);

            let minValue = $partialView.find("select.min-select").val() || "0";
            let maxValue = $partialView.find("select.max-select").val() || "10";

            // Store modifier group data
            let newModifierGroup = {
              modifierGroupId: groupId,
              min: minValue,
              max: maxValue,
            };

            selectedModifierGroupsforedit.push(newModifierGroup);

            console.log("response", response);

            // Modify select dropdowns to track changes
            $partialView
              .find("select.min-select")
              .attr("data-group-id", groupId);
            $partialView
              .find("select.max-select")
              .attr("data-group-id", groupId);

            // Attach event listeners to dropdowns inside the newly added partial view
            $partialView
              .find("select.min-select")
              .change(updateMinValueForEdit);
            $partialView
              .find("select.max-select")
              .change(updateMaxValueForEdit);

            // Trigger updateMaxValue once after rendering the partial view
            let maxSelect = $partialView.find("select.max-select");
            updateMaxValueForEdit.call(maxSelect);

            // Trigger updateMaxValue once after rendering the partial view
            let minSelect = $partialView.find("select.min-select");
            updateMinValueForEdit.call(minSelect);

            // Attach delete event listener for the trash icon
            $partialView
              .find(".delete-modifier-group")
              .on("click", function () {
                let groupId = $(this).attr("modifiergroup-id");

                // Remove the partial view
                $partialView.remove();

                // Remove the object from selectedModifierGroupsforedit
                selectedModifierGroupsforedit =
                  selectedModifierGroupsforedit.filter(
                    (obj) => obj.modifierGroupId != groupId
                  );

                console.log(
                  "Updated list after deletion:",
                  selectedModifierGroupsforedit
                );
              });

            // Append Partial View and show container
            $("#modifieritemspartialviewforedit")
              .append($partialView)
              .removeClass("d-none");
          },
          error: function () {
            alert("Error loading modifier items!");
          },
        });
      });
    },
    error: function () {
      alert("Error loading modifier group IDs!");
    },
  });
}

function updateMinValueForEdit() {
  let groupId = $(this).attr("data-group-id");
  let newValue = $(this).val();


  // Find and update the object in the list
  let modifierGroup = selectedModifierGroupsforedit.find(
    (obj) => obj.modifierGroupId == groupId
  );
  if (modifierGroup) {
    modifierGroup.min = newValue;
  }

  // this will ensure that if user select min option than all the option whose value is less than the selected min option will be removed from max select options
  let minVal = parseInt($(this).val());
  let modgroup = $(this).closest(".modifiergroupminmaxsection");
  let maxSelect = modgroup.find(".max-select");
  console.log("minval", minVal);
  maxSelect.find("option").each(function () {
    console.log("hii", parseInt($(this).val()));
    if (parseInt($(this).val()) < minVal) {
      $(this).css("display", "none"); // Hide the option
    } else {
      $(this).css("display", "block"); // Show the option
    }
  });

}

// Function to update max value in the list
function updateMaxValueForEdit() {
  let groupId = $(this).attr("data-group-id");
  let newValue = $(this).val();

  // Find and update the object in the list
  let modifierGroup = selectedModifierGroupsforedit.find(
    (obj) => obj.modifierGroupId == groupId
  );
  if (modifierGroup) {
    modifierGroup.max = newValue;
  }

  // this will ensure that if user select max option than all the option whose value is greater than the selected max option will be removed from min select options
  let maxVal = parseInt($(this).val());
  let modgroup = $(this).closest(".modifiergroupminmaxsection");
  let minSelect = modgroup.find(".min-select");

  minSelect.find("option").each(function () {

    if (parseInt($(this).val()) > maxVal) {
      $(this).css("display", "none"); // Hide the option
    } else {
      $(this).css("display", "block"); // Show the option
    }
  });

  console.log(selectedModifierGroupsforedit);
}

// Function To Open Add Menu Item Modal

function openAddMenuItemModal()
{
  var catid = $("#category-list").find(".category-active-option").attr('category-id');
  $("#Select-Category-option").val(catid);
  $("#Select-Category-option-hidden").val(catid);
  var modal = new bootstrap.Modal(document.getElementById("addmenuitem"));
  modal.show();
}

 // Function to update file name in the input field and File Validations
 function updateFileName(myFile, fileNameSpan, imageFileValidation) {

  var input = document.getElementById(myFile);
  var fileNameSpan = document.getElementById(fileNameSpan);
  var validationSpan = document.getElementById(imageFileValidation);
  validationSpan.textContent = "";

  if (input.files && input.files.length > 0) {
      var file = input.files[0];
      var fileSizeMB = file.size / (1024 * 1024);
      var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.jfif)$/i;

      //  Check extension
      if (!allowedExtensions.exec(file.name)) {
          validationSpan.textContent = "Only JPG, JPEG, PNG, and JFIF files are allowed.";
          fileNameSpan.textContent = 'Drag and Drop or browse file';
          input.value = "";
          return;
      }

      //  Check file size 
      if (fileSizeMB > 2) {
          validationSpan.textContent = "File size should not exceed 2 MB.";
          fileNameSpan.textContent = 'Drag and Drop or browse file';
          input.value = "";
          return;
      }

      //  If valid, show file name
      fileNameSpan.textContent = file.name;
  } else {
      fileNameSpan.textContent = 'Drag and Drop or browse file';
  }
}

$(document).ready(function () {
  // Store selected values as objects for showing partial view in add item
  let selectedModifierGroups = [];

  // modifierGroupSelect partial view render

  $("#modifierGroupSelect").change(function () {
    let selectedValue = $(this).val(); // Get selected option value
    let selectedText = $("#modifierGroupSelect option:selected").text();

    // Check if "Select Modifier Group" is chosen or if the value already exists
    if (
      selectedValue === "Select Modifier Group" ||
      selectedModifierGroups.some((obj) => obj.modifierGroupId == selectedValue)
    ) {
      return;
    }

    // Create object for the selected modifier group
    let newModifierGroup = {
      modifierGroupId: selectedValue,
      min: "0", // Default min value
      max: "10",
    };

    // Push new object to list
    selectedModifierGroups.push(newModifierGroup);

    // AJAX call to fetch the partial view
    $.ajax({
      url: "/Menu/GetModifierItems", // Replace with actual controller/action
      type: "GET",
      data: { modifierGroupId: selectedValue },
      success: function (response) {
        let $partialView = $(response);

        // Modify select dropdowns to track changes
        $partialView
          .find("select.min-select")
          .attr("data-group-id", selectedValue);
        $partialView
          .find("select.max-select")
          .attr("data-group-id", selectedValue);

        // Attach event listeners to dropdowns inside the newly added partial view
        $partialView.find("select.min-select").change(updateMinValue);
        $partialView.find("select.max-select").change(updateMaxValue);

        // Trigger updateMaxValue once after rendering the partial view
        let maxSelect = $partialView.find("select.max-select");
        updateMaxValue.call(maxSelect);

        // Attach delete event listener for the trash icon
        $partialView.find(".delete-modifier-group").on("click", function () {
          let groupId = $(this).attr("modifiergroup-id");

          // Remove the partial view
          $partialView.remove();

          // Remove the object from selectedModifierGroups
          selectedModifierGroups = selectedModifierGroups.filter(
            (obj) => obj.modifierGroupId != groupId
          );

          console.log("Updated list after deletion:", selectedModifierGroups);
        });

        // Append Partial View and show container
        $("#modifieritemspartialview")
          .append($partialView)
          .removeClass("d-none");
      },
      error: function () {
        alert("Error loading modifier items!");
      },
    });
  });

  // Function to update min value in the list
  function updateMinValue() {
    let groupId = $(this).attr("data-group-id");
    let newValue = $(this).val();

    // Find and update the object in the list
    let modifierGroup = selectedModifierGroups.find(
      (obj) => obj.modifierGroupId == groupId
    );
    if (modifierGroup) {
      modifierGroup.min = newValue;
    }

    // this will ensure that if user select min option than all the option whose value is less than the selected min option will be removed from max select options
    let minVal = parseInt($(this).val());
    let modgroup = $(this).closest(".modifiergroupminmaxsection");
    let maxSelect = modgroup.find(".max-select");
    console.log("minval", minVal);
    maxSelect.find("option").each(function () {
      console.log("hii", parseInt($(this).val()));
      if (parseInt($(this).val()) < minVal) {
        $(this).css("display", "none"); // Hide the option
      } else {
        $(this).css("display", "block"); // Show the option
      }
    });

    console.log("hi", selectedModifierGroups);
  }

  // Function to update max value in the list
  function updateMaxValue() {
    let groupId = $(this).attr("data-group-id");
    let newValue = $(this).val();

    // Find and update the object in the list
    let modifierGroup = selectedModifierGroups.find(
      (obj) => obj.modifierGroupId == groupId
    );
    if (modifierGroup) {
      modifierGroup.max = newValue;
    }

    // this will ensure that if user select max option than all the option whose value is greater than the selected max option will be removed from min select options
    let maxVal = parseInt($(this).val());
    let modgroup = $(this).closest(".modifiergroupminmaxsection");
    let minSelect = modgroup.find(".min-select");

    minSelect.find("option").each(function () {
 
      if (parseInt($(this).val()) > maxVal) {
        $(this).css("display", "none"); // Hide the option
      } else {
        $(this).css("display", "block"); // Show the option
      }
    });

    console.log(selectedModifierGroups);
  }

  // Add New Menu Item Validation
  $("#addItemForm").validate({
    rules: {
        "Menuitem.ItemName": {
            required: true,
            minlength: 2
        },
        "Menuitem.Type": {
            required: true
        },
        "Menuitem.Rate": {
            required: true,
            number: true,
            min: 0
        },
        "Menuitem.Quantity": {
            required: true,
            number: true,
            min: 1
        },
        "Menuitem.Unit": {
            required: true
        },
        "Menuitem.TaxPercentage": {
            required: true,
            number: true,
            min: 0,
            max: 100
        },
        
    },
    messages: {
        "Menuitem.ItemName": {
            required: "Please enter the item name.",
            minlength: "Name should be at least 2 characters."
        },
        "Menuitem.Type": {
            required: "Please select the item type."
        },
        "Menuitem.Rate": {
            required: "Please enter the rate.",
            number: "Please enter a valid number.",
            min: "Rate must be at least 0."
        },
        "Menuitem.Quantity": {
            required: "Please enter the quantity.",
            number: "Please enter a valid number.",
            min: "Quantity must be at least 1."
        },
        "Menuitem.Unit": {
            required: "Please select the unit."
        },
        "Menuitem.TaxPercentage": {
            required: "Please enter the tax percentage.",
            number: "Please enter a valid number.",
            min: "Minimum value is 0.",
            max: "Maximum value is 100."
        },
       
    },
    errorElement: 'span',
    errorClass: 'text-danger',
    highlight: function (element) {
        $(element).addClass('is-invalid');
    },
    unhighlight: function (element) {
        $(element).removeClass('is-invalid');
    }
});

  // Reset Add Form When Modal Close
  $("#addmenuitem").on("hidden.bs.modal", function () {
    // Reset the form inside the modal
    $(this).find("form")[0].reset();

    // Reset ASP.NET Core validation
    var validator = $(this).find("form").validate();
    validator.resetForm();
    $("#imageFileValidation").text('');

    // Remove invalid classes
    $(this).find(".is-invalid").removeClass("is-invalid");
  });

  // Add New Menu Item Form Submition

  $("#addItemForm").on('submit',function (e) {
    e.preventDefault();
    
    if(!$(this).valid())
    {
      return;
    }
    var formData = new FormData(this);

    console.log("venil", selectedModifierGroups);

    formData.append("ModifierGroups", JSON.stringify(selectedModifierGroups));

    $.ajax({
      url: "/Menu/AddNewItem",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        // close the opened delete modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("addmenuitem")
        );
        Modal.hide();

        // reset form after item added succesfully
        $("#addItemForm")[0].reset();
        $("#modifieritemspartialview").html("");
 
        // clear the list of selected modifier items for add item
        selectedModifierGroups = [];

        if (response.success) {
          //get the active category id
          let cat_id = $("#category-list .category-active-option").attr(
            "category-id"
          );
          loadMenuItem(cat_id);
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

    // Edit Menu Item Validation
  $("#editItemForm").validate({
      rules: {
          "Menuitem.ItemName": {
              required: true,
              minlength: 2
          },
          "Menuitem.Type": {
              required: true
          },
          "Menuitem.Rate": {
              required: true,
              number: true,
              min: 0
          },
          "Menuitem.Quantity": {
              required: true,
              number: true,
              min: 1
          },
          "Menuitem.Unit": {
              required: true
          },
          "Menuitem.TaxPercentage": {
              required: true,
              number: true,
              min: 0,
              max: 100
          },
          
      },
      messages: {
          "Menuitem.ItemName": {
              required: "Please enter the item name.",
              minlength: "Name should be at least 2 characters."
          },
          "Menuitem.Type": {
              required: "Please select the item type."
          },
          "Menuitem.Rate": {
              required: "Please enter the rate.",
              number: "Please enter a valid number.",
              min: "Rate must be at least 0."
          },
          "Menuitem.Quantity": {
              required: "Please enter the quantity.",
              number: "Please enter a valid number.",
              min: "Quantity must be at least 1."
          },
          "Menuitem.Unit": {
              required: "Please select the unit."
          },
          "Menuitem.TaxPercentage": {
              required: "Please enter the tax percentage.",
              number: "Please enter a valid number.",
              min: "Minimum value is 0.",
              max: "Maximum value is 100."
          },
         
      },
      errorElement: 'span',
      errorClass: 'text-danger',
      highlight: function (element) {
          $(element).addClass('is-invalid');
      },
      unhighlight: function (element) {
          $(element).removeClass('is-invalid');
      }
  });

   // Reset Edit Menu Item Form When Modal Close
   $("#editmenuitem").on("hidden.bs.modal", function () {
    // Reset the form inside the modal
    $(this).find("form")[0].reset();

    // Reset ASP.NET Core validation
    var validator = $(this).find("form").validate();
    validator.resetForm();
    $("#imageFileValidationForEdit").text('');

    // Remove invalid classes
    $(this).find(".is-invalid").removeClass("is-invalid");
  });



  // mass delete of menu item

  $("#deletemassmenuitemBtn").click(function (e) {
    $.ajax({
      url: "/Menu/Deleteitems",
      method: "POST",
      data: {
        ids: selectedMenuItems,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deletemassmenuitemmodal")
        );
        deleteModal.hide();

        if (response.success) {
          //get the active category id
          let cat_id = $("#category-list .category-active-option").attr(
            "category-id"
          );
          loadMenuItem(cat_id);
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

  // single delete of menu item

  $("#deleteitemBtn").click(function (e) {
    var catid = $(this).attr("category-id");
    var itemid = $(this).attr("item-id");

    

    $.ajax({
      url: "/Menu/DeleteSingleItem",
      method: "POST",
      data: {
        id: itemid,
        catid: catid,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deleteitemmodal")
        );
        deleteModal.hide();

        //clear the search field after delete
        $("#menuitem-search-field").val('');

        var page = $("#menuitem-pagination-section").data('page');

        console.log(page);

        if (response.success) {
          loadMenuItem(catid,page.pageSize,page.currentPage);
          toastr.success("Item Deleted Successfully!");
        } else {
          toastr.error("Error In Deleting Item!");
        }
      },
      error: function (xhr, status, error) {
        console.error("Error deleting items:", error);
      },
    });
  });

  //submit edit item form 

  $("#editItemForm").submit(function (e) {
    e.preventDefault();

    // if (!validateFormEditMenuItem()) {
    //   return;
    // }
    if(!$(this).valid())
    {
      return;
    }
    var formData = new FormData(this);

    console.log("venil", selectedModifierGroupsforedit);

    formData.append(
      "ModifierGroups",
      JSON.stringify(selectedModifierGroupsforedit)
    );

    $.ajax({
      url: "/Menu/EditItem",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        // close the opened delete modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("editmenuitem")
        );
        Modal.hide();

        if (response.success) {
          //get the active category id
          let cat_id = $("#category-list .category-active-option").attr(
            "category-id"
          );
          loadMenuItem(cat_id);
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

  // Form Validation Of Edit Menu Item
  function validateFormEditMenuItem() {
    let isValid = true;
    let errorMessage = "";

    const itemName = $("#itemnameforedit").val();
    const type = $("#itemtypeforedit").val();
    const unit = $("#ItemUnitForEdit").val();
    const rate = $("#ItemRateForEdit").val();
    const quantity = $("#ItemQuantityForEdit").val();
    const tax = $("#itemTaxPercentageForEdit").val();

    if (!itemName) {
      isValid = false;
    }

    if (!type) {
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
    if (!tax || isNaN(tax) || tax < 0 || tax > 100) {
      isValid = false;
    }

    return isValid;
  }

  // render new partial view for edit item modal

  $("#modifierGroupSelectforedit").change(function () {
    let selectedValue = $(this).val();
    let selectedText = $("#modifierGroupSelect option:selected").text();

    if (
      selectedValue === "Select Modifier Group" ||
      selectedModifierGroupsforedit.some(
        (obj) => obj.modifierGroupId == selectedValue
      )
    ) {
      return;
    }

    let newModifierGroup = {
      modifierGroupId: selectedValue,
      min: "0",
      max: "10",
    };
    selectedModifierGroupsforedit.push(newModifierGroup);

    $.ajax({
      url: "/Menu/GetModifierItems",
      type: "GET",
      data: { modifierGroupId: selectedValue },
      success: function (response) {
        let $partialView = $(response);

        $partialView
          .find("select.min-select")
          .attr("data-group-id", selectedValue);
        $partialView
          .find("select.max-select")
          .attr("data-group-id", selectedValue);

        $partialView.find("select.min-select").change(updateMinValue);
        $partialView.find("select.max-select").change(updateMaxValue);

        let maxSelect = $partialView.find("select.max-select");
        updateMaxValueForEdit.call(maxSelect);

        $partialView.find(".delete-modifier-group").on("click", function () {
          let groupId = $(this).attr("modifiergroup-id");
          $partialView.remove();
          selectedModifierGroupsforedit = selectedModifierGroupsforedit.filter(
            (obj) => obj.modifierGroupId != groupId
          );
          console.log(
            "Updated list after deletion:",
            selectedModifierGroupsforedit
          );
        });

        $("#modifieritemspartialviewforedit")
          .append($partialView)
          .removeClass("d-none");
      },
      error: function () {
        alert("Error loading modifier items!");
      },
    });
  });
});
