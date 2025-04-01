


$(document).ready(function () {
    // Add New Menu Item Form Submition

  $("#addtableform").submit(function (e) {
    e.preventDefault();

    if (!validateFormAddTableItem()) {
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

  // Add item Form Validation

  function validateFormAddTableItem() {
    let isValid = true;
    let errorMessage = "";

    const itemName = $("#ItemNameForAdd").val();
    const type = $("#ItemTypeForAdd").val();
    const unit = $("#ItemUnitForAdd").val();
    const rate = $("#ItemRateForAdd").val();
    const quantity = $("#ItemQuantityForAdd").val();
    const tax = $("#TaxPercentageForAdd").val();

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
});