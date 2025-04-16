// Make the function global

// load category list partial view

function loadcategories(id) {
  // For Clearing the List of Previously Selected Category
  selectedMenuItems = [];

  //   id = ele?.getAttribute("category-id");

  $.ajax({
    type: "GET",
    url: "/Menu/GetCategories",
    data: { cat: id },
    success: function (data) {
      $("#category-list").html(data);
    },
  });

  $.ajax({
    url: "/Menu/Menu",
    type: "GET",
    data: { cat: id },
    success: function (data) {
      $("#menuTableContainer").html(data);
      attachMassDeleteForMenuItem(); // This Function is Inside MenuItem.js
    },
  });
}

// opening modals

//load menu item partial view

function loadMenuItem(id) {
  $.ajax({
    url: "/Menu/Menu",
    type: "GET",
    data: { cat: id },
    success: function (data) {
      $("#menuTableContainer").html(data);
      attachMassDeleteForMenuItem(); // This Function is Inside MenuItem.js
    },
  });
}

// set The Category Data To The Modal For Edit

function setEditCategoryData(ele) {
  var c = JSON.parse(ele.getAttribute("item-obj"));

  // Set the values in the modal form
  document.getElementById("editcategoryid").value = c.id;
  document.getElementById("editcategoryformname").value = c.name;
  document.getElementById("editcategoryformdescription").value = c.description;
}

//set delete category url to the delete category modal yes button

function setDeleteCategoryId(ele) {
  let id = ele.getAttribute("category-id");
  let deleteBtn = document.getElementById("deleteCategoryBtn");
  deleteBtn.setAttribute("category-id", id);
}

$(document).ready(function () {
  // Add Category Form Validation

  $("#categoryForm").validate({
    rules: {
      "Category.Name": {
        required: true,
        minlength: 2,
      },
    },
    messages: {
      "Category.Name": {
        required: "Please enter a category name.",
        minlength: "Name must be at least 2 characters.",
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

  // Reset Add Form When Modal Close
  $("#addcategorymodal").on("hidden.bs.modal", function () {
    // Reset the form inside the modal
    $(this).find("form")[0].reset();

    // Reset ASP.NET Core validation
    var validator = $(this).find("form").validate();
    validator.resetForm();

    // Remove invalid classes
    $(this).find(".is-invalid").removeClass("is-invalid");
  });

  // Add Category (done by asp-controller and action direct form submission)

  $("#categoryForm").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission

    if (!$(this).valid()) {
      return;
    }

    // Create FormData object from the form
    var formData = new FormData(this);

    $.ajax({
      url: "/Menu/Category", // URL from the form's action attribute
      type: "POST", // Form method
      data: formData, // Form data
      processData: false, // Important: prevent jQuery from processing the data
      contentType: false, // Important: prevent jQuery from setting content type
      success: function (response) {
        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("addcategorymodal")
        );
        Modal.hide();

        if (response.success) {
          //get the active category id
          let cat_id = $("#category-list .category-active-option").attr(
            "category-id"
          );
          loadcategories(cat_id);
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (xhr, status, error) {
        toastr.error(error);
      },
    });
  });

    // Add Category Form Validation

    $("#editcategoryform").validate({
      rules: {
        "Category.Name": {
          required: true,
          minlength: 2,
        },
      },
      messages: {
        "Category.Name": {
          required: "Please enter a category name.",
          minlength: "Name must be at least 2 characters.",
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
  
    // Reset Add Form When Modal Close
    $("#editcategory").on("hidden.bs.modal", function () {
      // Reset the form inside the modal
      $(this).find("form")[0].reset();
  
      // Reset ASP.NET Core validation
      var validator = $(this).find("form").validate();
      validator.resetForm();
  
      // Remove invalid classes
      $(this).find(".is-invalid").removeClass("is-invalid");
    });



  //Edit Category (Form Direct Submitted Using Asp Controller And Action)

  $("#editcategoryform").submit(function (event) {
    event.preventDefault(); // Prevent the default form submission

    if (!$(this).valid()) {
      return;
    }

    // Create FormData object from the form
    var formData = new FormData(this);

    var categoryId = formData.get("Category.Id");

    $.ajax({
      url: "/Menu/EditCategoryById", // URL from the form's action attribute
      type: "POST", // Form method
      data: formData, // Form data
      processData: false, // Important: prevent jQuery from processing the data
      contentType: false, // Important: prevent jQuery from setting content type
      success: function (response) {
        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("editcategory")
        );
        Modal.hide();

        if (response.success) {
          loadcategories(categoryId);
          toastr.success(response.message);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (xhr, status, error) {
        toastr.error(error);
      },
    });
  });

  function validateEditCategoryForm() {
    const name = $("#editcategoryformname").val().trim();

    if (name === "") {
      return false;
    } else {
      return true;
    }
  }

  // Delete Category

  $("#deleteCategoryBtn").click(function (e) {
    var catid = $(this).attr("category-id");
    $.ajax({
      url: "/Menu/DeleteCategory",
      method: "POST",
      data: {
        id: catid,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deletecategorymodal")
        );
        deleteModal.hide();

        if (response.success) {
          //get the active category id
          let cat_id = $("#category-list .category-active-option").attr(
            "category-id"
          );
          cat_id == catid ? loadcategories() : loadcategories(cat_id); // if current category is deleted than by default select first category

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

  // Hover Effect On Category

  // Function to handle hover effect
  function attachHoverEffect() {
    document.querySelectorAll(".category-option").forEach((opt) => {
      opt.addEventListener("mouseover", function () {
        console.log("Mouse Entered hahah");
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

  $(document).on("ajaxComplete", function () {
    attachHoverEffect();
  });
});
