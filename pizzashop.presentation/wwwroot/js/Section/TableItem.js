//  function to open add table modal 

function openAddTableModal(ele)
{
  var sectionid = $("#section-list").find('.category-active-option').attr('section-id');

  $("#sectionoftable").val(sectionid)

  var modal = new bootstrap.Modal(document.getElementById("AddTablemodal"));
  modal.show();

}

// Mass Delete For Table 

let SelectedTableList = [];

function attachMassDeleteForTable() {
  // Event listener for main checkbox
  $(document).on("change", "#main_table_checkbox", function () {
      let isChecked = this.checked;
      $(".tablelist_inner_checkbox").prop("checked", isChecked);

      if (isChecked) {
          $(".tablelist_inner_checkbox").each(function () {
              let id = parseInt($(this).val()); // Convert to integer
              if (!SelectedTableList.includes(id)) {
                  SelectedTableList.push(id);
              }
          });
      } else {
          $(".tablelist_inner_checkbox").each(function () {
            let id = parseInt($(this).val());
            // Remove the unchecked item from the array
            SelectedTableList = SelectedTableList.filter(
              (item) => item != id
            );
          });

          console.log("ok table list",SelectedTableList);
      }
  });

  // Event listener for inner checkboxes
  $(document).on("change", ".tablelist_inner_checkbox", function () {
      let id = parseInt($(this).val()); // Convert to integer
      if (this.checked) {
          if (!SelectedTableList.includes(id)) {
              SelectedTableList.push(id);
          }
      } else {
          SelectedTableList = SelectedTableList.filter(item => item !== id);
      }

      // Update main checkbox state
      const allChecked = $(".tablelist_inner_checkbox").length === $(".tablelist_inner_checkbox:checked").length && $(".tablelist_inner_checkbox:checked").length!=0;
      $("#main_table_checkbox").prop("checked", allChecked);
  });

  // Restore previously selected checkboxes
  $(".tablelist_inner_checkbox").each(function () {
      if (SelectedTableList.includes(parseInt($(this).val()))) { // Convert to integer
          $(this).prop("checked", true);
      }
  });

  // Update main checkbox state initially
  const allChecked = $(".tablelist_inner_checkbox").length === $(".tablelist_inner_checkbox:checked").length && $(".tablelist_inner_checkbox:checked").length!=0;
  $("#main_table_checkbox").prop("checked", allChecked);
}



// Delete single Dining Table 

function setDeleteTableData(element) {

    var Id = element.getAttribute("table-id");
    var sectionid = element.getAttribute("section-id");
    var deleteBtn = document.getElementById("deleteTableBtn");
    deleteBtn.setAttribute("table-id", Id);
    deleteBtn.setAttribute("section-id", sectionid);

}

function loadTableList(id) {
  $.ajax({
    url: '/Section/GetDiningTableList',
    type: "GET",
    data: { id: id },
    success: function (data) {
      $("#diningtablelistcontainer").html(data);
      attachMassDeleteForTable();
    },
  });
}

// function for opening mass delete modal for menu item

function openMassDeleteModalForTable() {
  if (SelectedTableList.length == 0) {
    toastr.warning("Please Select Menu Item!");
    return;
  }
  var deleteModal = new bootstrap.Modal(
    document.getElementById("deletemultipletablemodal")
  );
  deleteModal.show();
}

$(document).ready(function () {
  // Add New table Form Submition

  $("#addtableform").submit(function (e) {
    e.preventDefault();

    if (!validateFormAddTableItem()) {
      return;
    }
    var formData = new FormData(this);

    $.ajax({
      url: "/Section/AddTable",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        // close the opened delete modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("AddTablemodal")
        );
        Modal.hide();

        // reset form after item added succesfully
        $("#addtableform")[0].reset();

        if (response.success) {
          //get the active category id
          let id = $("#section-list .category-active-option").attr(
            "section-id"
          );
          loadTableList(id);
          toastr.success(response.message);
        } else {
          console.log("venil",response);
          toastr.error(response.message);
        }
      },
      error: function (err) {
        console.error("Error adding item:", err);
      },
    });
  });

  // Add table Form Validation

  function validateFormAddTableItem() {
    let isValid = true;

    const itemName = $("#NameofTable").val();
    const status = $("#statusoftable").val();
    const capacity = $("#capacityoftable").val();
    const section = $("#sectionoftable").val();

    if (!itemName) {
      isValid = false;
    }

    if (!status) {
      isValid = false;
    }

    if (!section) {
      isValid = false;
    }

    if (!capacity || isNaN(capacity) || capacity <= 0) {
      isValid = false;
    }

    return isValid;
  }
  // edit table Form Submition

  $("#EditTableform").submit(function (e) {
    e.preventDefault();

    if (!validateFormEditTableItem()) {
      return;
    }
    var formData = new FormData(this);

    $.ajax({
      url: "/Section/EditTable",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (response) {
        // close the opened delete modal

        var Modal = bootstrap.Modal.getInstance(
          document.getElementById("EditTablemodal")
        );
        Modal.hide();

        if (response.success) {
          //get the active category id
          let id = $("#section-list .category-active-option").attr(
            "section-id"
          );
          loadTableList(id);

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

  // Add table Form Validation

  function validateFormEditTableItem() {
    let isValid = true;

    const itemName = $("#NameofTableforedit").val();
    const status = $("#statusoftableforedit").val();
    const capacity = $("#capacityoftableforedit").val();
    const section = $("#sectionidforedit").val();

    if (!itemName) {
      isValid = false;
    }

    if (!status) {
      isValid = false;
    }

    if (!section) {
      isValid = false;
    }

    if (!capacity || isNaN(capacity) || capacity <= 0) {
      isValid = false;
    }

    return isValid;
  }

  // mass delete of menu item

  $("#deletemultipleTableBtn").click(function (e) {
    e.preventDefault(); // Prevent the default action of the button
    
    $.ajax({
      url: "/Section/DeleteTables",
      method: "POST",
      data: { ids: SelectedTableList }, // Properly serialize the data
      success: function (response) {
        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deletemultipletablemodal")
        );
        deleteModal.hide();

        DeleteTables =[];
  
        if (response.success) {
          let id = $("#section-list .category-active-option").attr("section-id");
          loadTableList(id);
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

  $("#deleteTableBtn").click(function (e) {
    var sectionid = $(this).attr("section-id")
    var tableid = $(this).attr("table-id");

    $.ajax({
      url: "/Section/DeleteTable",
      method: "POST",
      data: {
        id: tableid,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deletetablemodal")
        );
        deleteModal.hide();

        if (response.success) {
          loadTableList(sectionid);
          toastr.success("table Deleted Successfully!");
        } else {
          toastr.error("Error In Deleting table!");
        }
      },
      error: function (xhr, status, error) {
        console.error("Error deleting items:", error);
      },
    });
  });




  // ==========================



  // Add Section Form Validation
$("#addsectionform").validate({
  rules: {
    "Section.SectionName": {
      required: true,
    },
    "Section.Description": {
      required: true,
    },
  },
  messages: {
    "Section.SectionName": {
      required: "Please enter a section name",
    },
    "Section.Description": {
      required: "Please enter a description",
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


$("#Addsectionmodal").on("hidden.bs.modal", function () {
  console.log("hii")
  $("#addsectionform")[0].reset();
  var validator = $("#addsectionform").validate();
  validator.resetForm();
  $("#addsectionform").find(".is-invalid").removeClass("is-invalid");
});

$('#Addsectionmodal').on('show.bs.modal',function(){
  alert("Modal Closed");
});


// $("#Editsectionmodal").on("hidden.bs.modal", function () {

//   console.log("hii")
//   $("#editsectionform")[0].reset();
//   var validator = $("#editsectionform").validate();
//   validator.resetForm();
//   $("#editsectionform").find(".is-invalid").removeClass("is-invalid");
// });

// Edit Section Form Validation
$("#editsectionform").validate({
  rules: {
    "Section.SectionName": {
      required: true,
    },
    "Section.Description": {
      required: true,
    },
  },
  messages: {
    "Section.SectionName": {
      required: "Please enter a section name",
    },
    "Section.Description": {
      required: "Please enter a description",
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


  

});
