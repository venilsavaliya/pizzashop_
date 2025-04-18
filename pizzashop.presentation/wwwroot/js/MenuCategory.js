// Make the function global

// Open Modal For Add Edit CAtegory Form

function openAddEditCategoryModal(id)
{
  $.ajax({
    type: "GET",
    url: "/Menu/GetAddEditCategoryForm",
    data: { id: id },
    success: function (data) {
      $("#categoryAddEditFormContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("categoryAddEditModal")
      );
      modal.show();
    },
  });
}

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

function loadMenuItem(id,pagesize,pagenumber) {
  $.ajax({
    url: "/Menu/Menu",
    type: "GET",
    data: { cat: id,pageNumber:pagenumber,pageSize:pagesize },
    success: function (data) {
      $("#menuTableContainer").html(data);
      attachMassDeleteForMenuItem(); // This Function is Inside MenuItem.js
    },
  });
}



//set delete category url to the delete category modal yes button

function setDeleteCategoryId(ele) {
  let id = ele.getAttribute("category-id");
  let deleteBtn = document.getElementById("deleteCategoryBtn");
  deleteBtn.setAttribute("category-id", id);
}

$(document).ready(function () {
  

  // Add Edit Category Form Submission

  $(document).on("submit", "#categoryAddEditForm", function (e) {
    e.preventDefault();
    var form = $(this);
    if (!form.valid()) {
      return;
    }

    var formData = new FormData(form[0]);
    $.ajax({
      url: form.attr("action"),
      type: "POST",
      data: formData,
      processData: false, 
      contentType: false, 
      success: function (response) {
        if(response.success)
        {
            toastr.success(response.message);
            let cat_id = $("#category-list .category-active-option").attr(
              "category-id"
            );
            loadcategories(cat_id);
            var modal = bootstrap.Modal.getInstance(document.getElementById('categoryAddEditModal'))
            modal.hide();
        }
        else{
            toastr.error(response.message);
        }
        
       
      },
      error: function (err) {
        alert("Something went wrong");
      },
    });
  });

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
