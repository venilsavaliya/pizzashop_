// open add edit table form

function openAddEditTableForm(id) {
  let sectionid = $("#section-list .category-active-option").attr("section-id");
  $.ajax({
    type: "GET",
    url: "/Section/GetAddEditTableForm",
    data: { id: id, activesectionid: sectionid },
    success: function (data) {
      $("#AddEditTableModalContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("AddEditTableModal")
      );
      modal.show();
    },
  });
}
// open add edit section form

function openAddEditSectionForm(id) {
  $.ajax({
    type: "GET",
    url: "/Section/GetAddEditSectionForm",
    data: { id: id },
    success: function (data) {
      $("#AddEditSectionModalContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("AddEditSectionModal")
      );
      modal.show();
    },
  });
}

//=================================================================

//  function to open add table modal

// function openAddTableModal(ele) {
//   var sectionid = $("#section-list")
//     .find(".category-active-option")
//     .attr("section-id");
//   $.ajax({
//     type: "GET",
//     url: "/Section/GetSectionListData",
//     success: function (data) {
//       console.log(data);
//       // this will load modifier group list
//       $("#sectionoftable").html("");
//       data.forEach((c) => {
//         $("#sectionoftable").append(`
//             <option value="${c.sectionId}" ${
//           c.sectionId == sectionid ? "selected" : ""
//         }>${c.sectionName}</option>`);
//       });
//     },
//   });

//   // $("#sectionoftable").val(sectionid);

//   var modal = new bootstrap.Modal(document.getElementById("AddTablemodal"));
//   modal.show();
// }

// Mass Delete For Table

let SelectedTableList = [];

function attachMassDeleteForTable() {
  // Main checkbox change
  $(document).on("change", "#main_table_checkbox", function () {
    let isChecked = this.checked;
    $(".tablelist_inner_checkbox").prop("checked", isChecked);

    if (isChecked) {
      $(".tablelist_inner_checkbox").each(function () {
        let id = parseInt($(this).val());
        let status = $(this).data("status");

        if (!SelectedTableList.some((item) => item.id == id)) {
          SelectedTableList.push({ id, status });
        }
      });
    } else {
      $(".tablelist_inner_checkbox").each(function () {
        let id = parseInt($(this).val());
        SelectedTableList = SelectedTableList.filter((item) => item.id != id);
      });
    }
  });

  // Inner checkbox change
  $(document).on("change", ".tablelist_inner_checkbox", function () {
    let id = parseInt($(this).val());
    let status = $(this).data("status");

    if (this.checked) {
      if (!SelectedTableList.some((item) => item.id == id)) {
        SelectedTableList.push({ id, status });
      }
    } else {
      SelectedTableList = SelectedTableList.filter((item) => item.id != id);
    }

    // Update main checkbox
    const allChecked =
      $(".tablelist_inner_checkbox").length ==
        $(".tablelist_inner_checkbox:checked").length &&
      $(".tablelist_inner_checkbox:checked").length != 0;
    $("#main_table_checkbox").prop("checked", allChecked);
  });

  // Restore selection on page load or re-render
  $(".tablelist_inner_checkbox").each(function () {
    let id = parseInt($(this).val());
    if (SelectedTableList.some((item) => item.id == id)) {
      $(this).prop("checked", true);
    }
  });

  // Set initial main checkbox
  const allChecked =
    $(".tablelist_inner_checkbox").length ==
      $(".tablelist_inner_checkbox:checked").length &&
    $(".tablelist_inner_checkbox:checked").length != 0;
  $("#main_table_checkbox").prop("checked", allChecked);
}

// Delete single Dining Table

function setDeleteTableData(element) {
  var obj = $(element).data("obj");
  console.log(obj);

  if (obj.status != "Available") {
    toastr.warning("This Table is Occupied!");
    return;
  }

  var Id = element.getAttribute("table-id");
  var sectionid = element.getAttribute("section-id");
  var deleteBtn = document.getElementById("deleteTableBtn");
  deleteBtn.setAttribute("table-id", Id);
  deleteBtn.setAttribute("section-id", sectionid);

  var modal = new bootstrap.Modal(document.getElementById("deletetablemodal"));
  modal.show();
}

function loadTableList(id) {
  $.ajax({
    url: "/Section/GetDiningTableList",
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
  // check for occupied tables

  const OccupiedTables = SelectedTableList.filter(
    (item) => item.status != "Available"
  );
  if (OccupiedTables.length > 0) {
    toastr.warning("Cannot delete occupied tables");
    return;
  }

  var deleteModal = new bootstrap.Modal(
    document.getElementById("deletemultipletablemodal")
  );
  deleteModal.show();
}

// Load Sections List

function loadsection(id) {
  // clear the list of selected tables
  SelectedTableList = [];

  $.ajax({
    type: "GET",
    url: "/Section/GetSections",
    data: { id: id },
    success: function (data) {
      $("#section-list").html(data);
      $("#section-list-for-smallscreen").html(data);
    },
  });

  $.ajax({
    url: "/Section/GetDiningTableList",
    type: "GET",
    data: { id: id },
    success: function (data) {
      $("#diningtablelistcontainer").html(data);
      attachMassDeleteForTable();
    },
  });
}

// Table List Pagination
function TableListPaginationAjax(pageSize, pageNumber, sectionid) {
  console.log("inside page", pageSize, pageNumber);
  // Get the dropdown element

  let id = $("#section-list .category-active-option").attr("section-id");
  let searchkeyword = $("#tableitem-search-field").val();

  $.ajax({
    url: "/Section/GetDiningTableList",
    data: {
      pageSize: pageSize,
      pageNumber: pageNumber,
      searchKeyword: searchkeyword,
      id: id,
    },
    type: "GET",
    dataType: "html",
    success: function (data) {
      $("#diningtablelistcontainer").html(data);
      attachMassDeleteForTable();
    },
    error: function () {
      $("#diningtablelistcontainer").html("An error has occurred");
    },
  });
}

//load tables  partial view

function loadTables(id) {
  $.ajax({
    url: '@Url.Action("GetDiningTableList", "Section")',
    type: "GET",
    data: { id: id },
    success: function (data) {
      $("#diningtablelistcontainer").html(data);
      attachMassDeleteForTable();
    },
  });
}

// set edit data for table

// function setEditTabledata(ele) {
//   var c = JSON.parse(ele.getAttribute("item-obj"));
//   console.log(c);

//   var editsectionitem = document.getElementById("EditTablemodal");

//   editsectionitem.querySelector("#NameofTableforedit").value = c.name;
//   editsectionitem.querySelector("#TableidForEdit").value = c.tableId;
//   editsectionitem.querySelector("#capacityoftableforedit").value = c.capacity;
//   $.ajax({
//     url: "/Section/GetStatusIdByName", // URL from the form's action attribute
//     type: "Get", // Form method
//     data: { name: c.status },
//     success: function (data) {
//       console.log("venil", data);
//       editsectionitem.querySelector("#statusoftableforedit").value =
//         data.statusId;
//     },
//   });
//   editsectionitem.querySelector("#statusoftableforedit").value = c.status;
//   editsectionitem.querySelector("#sectionidforedit").value = c.sectionId;
// }

//set edit data for section

// function setEditSectionData(ele) {
//   var c = JSON.parse(ele.getAttribute("item-obj"));
//   console.log(c);

//   var editsectionitem = document.getElementById("Editsectionmodal");
//   editsectionitem.querySelector("#Sectionid").value = c.sectionId;
//   editsectionitem.querySelector("#SectionNameforedit").value = c.sectionName;
//   editsectionitem.querySelector("#Description").value = c.description;
// }

// set delete data for section

function setDeleteSectionId(Id) {
  var deleteBtn = document.getElementById("deleteSectionBtn");
  deleteBtn.setAttribute("section-id", Id);
  $.ajax({
    type: "Get",
    url: "/Section/GetBusyTableCountOfSection",
    data: { sectionid: Id },
    success: function (count) {
      if (count > 0) {
        toastr.warning("This Section Have Occupied Table(s)!");
        return;
      }
      var modal = new bootstrap.Modal(
        document.getElementById("deletesectionmodal")
      );
      modal.show();
    },
  });
}

$(document).ready(function () {
  loadsection();
  TableListPaginationAjax();

  // handle list showing of section in add table form
  //  $("#AddTablemodal").on("shown.bs.modal", function () {

  // });
  // handle list showing of section in edit table form
  // $("#EditTablemodal").on("shown.bs.modal", function () {
  //   $.ajax({
  //     type: "GET",
  //     url: "/Section/GetSectionListData",
  //     success: function (data) {
  //       console.log(data);
  //       // this will load modifier group list
  //       $("#sectionidforedit").html("");
  //       data.forEach((c) => {
  //         $("#sectionidforedit").append(`
  //           <option value="${c.sectionId}">${c.sectionName}</option>`);
  //       });
  //     },
  //   });
  // });

  // keyup search
  document
    .getElementById("tableitem-search-field")
    .addEventListener("keyup", () => {
      console.log("hello");
      TableListPaginationAjax();
    });

  // Add Section Form Validation
  // $("#addsectionform").validate({
  //   rules: {
  //     "Section.SectionName": {
  //       required: true,
  //     },
  //     "Section.Description": {
  //       required: true,
  //     },
  //   },
  //   messages: {
  //     "Section.SectionName": {
  //       required: "Please enter a section name",
  //     },
  //     "Section.Description": {
  //       required: "Please enter a description",
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

  // add section form reset
  // $("#Addsectionmodal").on("hidden.bs.modal", function () {
  //   $("#addsectionform")[0].reset();
  //   var validator = $("#addsectionform").validate();
  //   validator.resetForm();
  //   $("#addsectionform").find(".is-invalid").removeClass("is-invalid");
  // });

  // add section form submission
  // $("#addsectionform").on("submit", function (event) {
  //   event.preventDefault(); // Prevent the default form submission

  //   if (!$(this).valid()) {
  //     event.preventDefault();
  //     return;
  //   }

  //   // Create FormData object from the form
  //   var formData = new FormData(this);

  //   $.ajax({
  //     url: "/Section/AddSection", // URL from the form's action attribute
  //     type: "POST", // Form method
  //     data: formData, // Form data
  //     processData: false, // Important: prevent jQuery from processing the data
  //     contentType: false, // Important: prevent jQuery from setting content type
  //     success: function (response) {
  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("Addsectionmodal")
  //       );
  //       Modal.hide();

  //       if (response.success) {
  //         //get the active category id
  //         let id = $("#section-list .category-active-option").attr(
  //           "section-id"
  //         );
  //         loadsection(id);
  //         toastr.success(response.message);
  //       } else {
  //         toastr.error(response.message);
  //       }
  //     },
  //     error: function (xhr, status, error) {
  //       toastr.error(error);
  //     },
  //   });
  // });

  // Edit Section Form Validation
  // $("#editsectionform").validate({
  //   rules: {
  //     "Section.SectionName": {
  //       required: true,
  //     },
  //     "Section.Description": {
  //       required: true,
  //     },
  //   },
  //   messages: {
  //     "Section.SectionName": {
  //       required: "Please enter a section name",
  //     },
  //     "Section.Description": {
  //       required: "Please enter a description",
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

  // add section form reset
  // $("#Editsectionmodal").on("hidden.bs.modal", function () {
  //   $("#editsectionform")[0].reset();
  //   var validator = $("#editsectionform").validate();
  //   validator.resetForm();
  //   $("#editsectionform").find(".is-invalid").removeClass("is-invalid");
  // });

  // Edit section form submission
  // $("#editsectionform").on("submit", function (event) {
  //   event.preventDefault(); // Prevent the default form submission

  //   if (!$(this).valid()) {
  //     return;
  //   }

  //   // Create FormData object from the form
  //   var formData = new FormData(this);

  //   $.ajax({
  //     url: "/Section/EditSection", // URL from the form's action attribute
  //     type: "POST", // Form method
  //     data: formData, // Form data
  //     processData: false, // Important: prevent jQuery from processing the data
  //     contentType: false, // Important: prevent jQuery from setting content type
  //     success: function (response) {
  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("Editsectionmodal")
  //       );
  //       Modal.hide();

  //       if (response.success) {
  //         //get the active category id
  //         let id = $("#section-list .category-active-option").attr(
  //           "section-id"
  //         );
  //         loadsection(id);
  //         toastr.success(response.message);
  //       } else {
  //         toastr.error(response.message);
  //       }
  //     },
  //     error: function (xhr, status, error) {
  //       toastr.error(error);
  //     },
  //   });
  // });

  // delete section

  $("#deleteSectionBtn").click(function (e) {
    var id = $(this).attr("section-id");
    $.ajax({
      url: "/Section/DeleteSection",
      method: "POST",
      data: {
        id: id,
      },
      success: function (response) {
        // close the opened delete modal

        var deleteModal = bootstrap.Modal.getInstance(
          document.getElementById("deletesectionmodal")
        );
        deleteModal.hide();

        if (response.success) {
          //get the active category id
          let secid = $("#section-list .category-active-option").attr(
            "section-id"
          );
          secid == id ? loadsection() : loadsection(secid); // if current section is deleted than by default select first section

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

  // Add Table Form Validation

  // $("#addtableform").validate({
  //   rules: {
  //     "Table.Name": {
  //       required: true,
  //     },
  //     "Table.SectionId": {
  //       required: true,
  //     },
  //     "Table.Capacity": {
  //       required: true,
  //       number: true, // Ensures the input is a number
  //       min: 1, // Minimum value validation (if needed)
  //     },
  //     "Table.Status": {
  //       required: true,
  //     },
  //   },
  //   messages: {
  //     "Table.Name": {
  //       required: "Please enter a table name",
  //     },
  //     "Table.SectionId": {
  //       required: "Please select a section",
  //     },
  //     "Table.Capacity": {
  //       required: "Please enter a capacity",
  //       number: "Capacity must be a number",
  //       min: "Capacity must be greater than 0",
  //     },
  //     "Table.Status": {
  //       required: "Please select a status",
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

  // add Table form reset
  // $("#AddTablemodal").on("hidden.bs.modal", function () {
  //   $("#addtableform")[0].reset();
  //   var validator = $("#addtableform").validate();
  //   validator.resetForm();
  //   $("#addtableform").find(".is-invalid").removeClass("is-invalid");
  // });

  // Add New table Form Submition
  // $("#addtableform").on("submit", function (e) {
  //   e.preventDefault();

  //   if (!$(this).valid()) {
  //     return;
  //   }

  //   var formData = new FormData(this);

  //   $.ajax({
  //     url: "/Section/AddTable",
  //     type: "POST",
  //     processData: false,
  //     contentType: false,
  //     data: formData,
  //     success: function (response) {
  //       // close the opened delete modal

  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("AddTablemodal")
  //       );
  //       Modal.hide();

  //       // reset form after item added succesfully
  //       $("#addtableform")[0].reset();

  //       if (response.success) {
  //         //get the active category id
  //         let id = $("#section-list .category-active-option").attr(
  //           "section-id"
  //         );
  //         loadTableList(id);
  //         toastr.success(response.message);
  //       } else {
  //         console.log("venil", response);
  //         toastr.error(response.message);
  //       }
  //     },
  //     error: function (err) {
  //       console.error("Error adding item:", err);
  //     },
  //   });
  // });

  // Edit Table Form Validation
  // $("#EditTableform").validate({
  //   rules: {
  //     "Table.Name": {
  //       required: true,
  //     },
  //     "Table.SectionId": {
  //       required: true,
  //     },
  //     "Table.Capacity": {
  //       required: true,
  //       number: true, // Ensures the input is a number
  //       min: 1, // Minimum value validation (if needed)
  //     },
  //     "Table.Status": {
  //       required: true,
  //     },
  //   },
  //   messages: {
  //     "Table.Name": {
  //       required: "Please enter a table name",
  //     },
  //     "Table.SectionId": {
  //       required: "Please select a section",
  //     },
  //     "Table.Capacity": {
  //       required: "Please enter a capacity",
  //       number: "Capacity must be a number",
  //       min: "Capacity must be greater than 0",
  //     },
  //     "Table.Status": {
  //       required: "Please select a status",
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

  // Edit Table Form Reset

  // $("#EditTablemodal").on("hidden.bs.modal", function () {
  //   $("#EditTableform")[0].reset();
  //   var validator = $("#EditTableform").validate();
  //   validator.resetForm();
  //   $("#EditTableform").find(".is-invalid").removeClass("is-invalid");
  // });

  // Edit table form submission
  // $("#EditTableform").on("submit", function (e) {
  //   e.preventDefault();

  //   if (!$(this).valid()) {
  //     return;
  //   }
  //   var formData = new FormData(this);

  //   $.ajax({
  //     url: "/Section/EditTable",
  //     type: "POST",
  //     processData: false,
  //     contentType: false,
  //     data: formData,
  //     success: function (response) {
  //       // close the opened delete modal

  //       var Modal = bootstrap.Modal.getInstance(
  //         document.getElementById("EditTablemodal")
  //       );
  //       Modal.hide();

  //       if (response.success) {
  //         //get the active category id
  //         let id = $("#section-list .category-active-option").attr(
  //           "section-id"
  //         );
  //         loadTableList(id);

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

  // single Table delete

  $("#deleteTableBtn").click(function (e) {
    var sectionid = $(this).attr("section-id");
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

  // mass delete of multiple table

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

        DeleteTables = [];

        if (response.success) {
          let id = $("#section-list .category-active-option").attr(
            "section-id"
          );
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

  $(document).on("submit", "#AddEditSectionForm", function (e) {
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
        if (response.success) {
          toastr.success(response.message);

          var modal = bootstrap.Modal.getInstance(
            document.getElementById("AddEditSectionModal")
          );
          modal.hide();
          let id = $("#section-list .category-active-option").attr(
            "section-id"
          );
          loadsection(id);
        } else {
          toastr.error(response.message);
        }
      },
      error: function (err) {
        console.log("Something went wrong");
      },
    });
  });
  $(document).on("submit", "#AddEditTableForm", function (e) {
    e.preventDefault();
    console.log("in");
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
        if (response.success) {
          toastr.success(response.message);
          var page = $("#TableAndSectionPagination").data("page");

          console.log("page", page);
          TableListPaginationAjax(page?.pageSize, page?.currentPage);
          var modal = bootstrap.Modal.getInstance(
            document.getElementById("AddEditTableModal")
          );
          modal.hide();
        } else {
          toastr.error(response.message);
        }
      },
      error: function (err) {
        console.log("Something went wrong");
      },
    });
  });
});
