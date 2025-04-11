// ============
const selectedTables = new Set();

function updateSelectedTableNames() {
  let names = Array.from(selectedTables)
    .map((item) => item.name)
    .join(", ");
  if (names.length === 0) names = "Select Table";
  $("#tableSelectBox").text(names);
}


function openAssignTableModal(ele) {
  const sectionid = $(ele).attr("section-id");
  const form = $("#TableAssignForm");

  form.find("select[name='SectionId']").val(sectionid);
  
  selectedTables.clear(); // clear previous selections
  updateSelectedTableNames();

  $.ajax({
    type: "GET",
    url: "/OrderAppWaitingList/GetAvailableTableList",
    data: { SectionId: sectionid },
    success: function (data) {
      const tables = data;
      console.log(tables)
      const dropdown = $("#tableDropdown");

      dropdown.empty(); // Clear old list

      if (!tables || tables.length === 0) {
        $("#tableSelectBox").text("No Tables Available").addClass("text-muted");
        return;
      }

      $("#tableSelectBox").removeClass("text-muted").text("Select Table");

      tables.forEach((table) => {
        const checkbox = `
          <div class="form-check ">
          
            <input type="checkbox" class="form-check-input table-checkbox" 
              value="${table.tableId}" id="table_${table.tableId}" data-name="${table.name}">
              <div class="d-flex justify-content-between">
              <label class="form-check-label" for="table_${table.tableId}">${table.name}</label>
              <div class="d-flex gap-2"><i class="bi bi-people"></i>${table.capacity}</div>
            </div>
          </div>
        `;
        dropdown.append(checkbox);
      });
    },
  });

  new bootstrap.Modal(document.getElementById("assignTableModal")).show();
}


// ==========



// function to load  Waiting List

function LoadWaitingList(sectionid) {
  console.log("ok in");

  $.ajax({
    url: "/OrderAppWaitingList/GetWaitingList",
    type: "GET",
    data: { sectionid: sectionid },
    success: function (data) {
      $("#waiting_List_container").html(data);
    },
    error: function (error) {
      console.log("Error loading in-progress orders:", error);
    },
  });
}

// open modal for waiting token

function openDeleteTokenModal(tokenid) {
  var deleteBtn = document.getElementById("deleteWaitingTokenBtn");
  deleteBtn.setAttribute("token-id", tokenid);

  var modal = new bootstrap.Modal(
    document.getElementById("deleteWaitingTokenModal")
  );
  modal.show();
}

// oprn modal for assign table

// function openAssignTableModal(ele) {
//   sectionid = $(ele).attr("section-id");
//   tokenid = $(ele).attr("token-id");

//   $.ajax({
//     type: "GET",
//     url: "/OrderAppWaitingList/GetAvailableTableList",
//     data: { SectionId: sectionid },
//     success: function (data) {
     
//     },
//   });

//   var form = $("#TableAssignForm");

//   form.find("select[name='SectionId']").val(sectionid);

//   var modal = new bootstrap.Modal(document.getElementById("assignTableModal"));
//   modal.show();
// }

// function to delete waiting token

function deleteToken(ele) {
  var tokenid = $(ele).attr("token-id");

  $.ajax({
    type: "POST",
    url: "/OrderAppWaitingList/DeleteWaitingToken",
    data: { TokenId: tokenid },
    success: function (data) {
      data.success ? toastr.success(data.message) : toastr.error(data.message);
      var modal = bootstrap.Modal.getInstance(
        document.getElementById("deleteWaitingTokenModal")
      );
      modal.hide();
      var sectionid = $("#section_list").find(".active").attr("section-id");
      LoadWaitingList(sectionid);
    },
  });
}

//function to load section list
function LoadSectionList(sectionid) {
  $.ajax({
    url: "/OrderAppWaitingList/GetSectionList",
    type: "GET",
    success: function (data) {
      // Wrap the returned HTML string into a jQuery object

      var $partialView = $(data);

      console.log("sectionid", sectionid);

      $("#section_list").html($partialView);

      // Update the partial view

      if (sectionid) {
        $("#section_list").find(".active").removeClass("active");
        $("#section_list")
          .find(`.nav-link[section-id="${sectionid}"]`)
          .addClass("active");
      }

      // Load section-specific content
      LoadWaitingList(sectionid);
    },
    error: function (error) {
      console.log("Error loading section:", error);
    },
  });
}

//function to set initial edit data for edit waiting modal

function setEditDataForWaitingToken(ele) {
  const itemData = $(ele).data("item"); // jQuery

  console.log(itemData);

  const form = $("#addWaitingTokenForm");

  form.find("input[name='Customer.Email']").val(itemData.email);
  form.find("input[name='Customer.Mobile']").val(itemData.mobile);
  form.find("input[name='Customer.Name']").val(itemData.name);
  form.find("input[name='Customer.TotalPerson']").val(itemData.totalperson);
  form.find("input[name='TokenId']").val(itemData.tokenid);
  form.find("select[name='SectionId']").val(itemData.sectionid);

  console.log("heloo bhai", form.find("input[name='SectionId']").val());
  openWaitingTokenModal();
}

// open modal for waiting token

function openWaitingTokenModal(forAdd) {
  var modal = new bootstrap.Modal(document.getElementById("waitingtokenmodal"));
  modal.show();

  if (forAdd) {
    var sectionid = $("#section_list").find(".active").attr("section-id");
    if (sectionid == null) {
      sectionid = "";
    }

    var sectionElement = $('#addWaitingTokenForm [name="SectionId"]');
    sectionElement.val(sectionid);
    var submitBtn = $("#addWaitingTokenForm button[type='submit']").text(
      "Assign"
    );
  } else {
    var submitBtn = $("#addWaitingTokenForm button[type='submit']").text(
      "Save"
    );
  }
}

$(document).ready(function () {

  // ======

  $(document).on("change", ".table-checkbox", function () {
    const tableId = $(this).val();
    const tableName = $(this).data("name");
  
    if ($(this).is(":checked")) {
      selectedTables.add({ id: tableId, name: tableName });
    } else {
      // Remove the unchecked one
      const toRemove = [...selectedTables].find((item) => item.id == tableId);
      if (toRemove) selectedTables.delete(toRemove);
    }
    console.log(selectedTables)
    updateSelectedTableNames(); // Always update UI
  });

  $("#tableSelectBox").on("click", function () {
    if (!$(this).hasClass("text-muted")) {
      $("#tableDropdown").toggle();
    }
  });
  
  $(document).on("click", function (e) {
    if (!$(e.target).closest("#tableSelectBox, #tableDropdown").length) {
      $("#tableDropdown").hide();
    }
  });

  // ========
  // Load waitinglist for all section first

  LoadSectionList();

  // Add Waiting Token Validations
  $("#addWaitingTokenForm").validate({
    rules: {
      "Customer.Email": {
        required: true,
        email: true,
      },
      "Customer.Name": {
        required: true,
      },
      "Customer.Mobile": {
        required: true,
        digits: true,
        minlength: 10,
        maxlength: 10,
      },
      "Customer.TotalPerson": {
        required: true,
        number: true,
        min: 1,
      },
      SectionId: {
        required: true,
      },
    },
    messages: {
      "Customer.Email": {
        required: "Please enter the Email",
        email: "Please enter a valid Email (e.g. name@example.com)",
      },
      "Customer.Name": {
        required: "Please enter a Name",
      },
      "Customer.Mobile": {
        required: "Please enter a Mobile Number",
        digits: "Please enter digits only",
        minlength: "Mobile number must be 10 digits",
        maxlength: "Mobile number must be 10 digits",
      },
      "Customer.TotalPerson": {
        required: "Please enter the No of People",
        number: "Please enter a valid number",
        min: "No of People must be at least 1",
      },
      SectionId: {
        required: "Please select the Section",
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

  // reset validation after modal close

  $("#waitingtokenmodal").on("hidden.bs.modal", function () {
    // Reset form fields
    $("#addWaitingTokenForm")[0].reset();

    // Reset validation
    var validator = $("#addWaitingTokenForm").validate();
    validator.resetForm();

    // Remove is-invalid classes
    $("#addWaitingTokenForm ").find(".is-invalid").removeClass("is-invalid");
  });

  // logic for fill detail of already existed customer in waiting token form

  $("#addWaitingTokenForm input[name='Customer.Email']").on(
    "input",
    function (e) {
      const email = $(this).val();

      const regex = /^[^@]+@[^@]+\.[^@]+$/;

      if (regex.test(email)) {
        $.ajax({
          type: "GET",
          url: "/Customer/GetCustomerDetail",
          data: {
            email: email,
          },
          success: function (data) {
            console.log(data.customer);
            $("#addWaitingTokenForm input[name='Customer.Name']").val(
              data.customer.name
            );
            $("#addWaitingTokenForm input[name='Customer.Mobile']").val(
              data.customer.mobile
            );
          },
        });
      }
    }
  );

  // submit assign table form

  $("#addWaitingTokenForm").on("submit", function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
      return;
    }

    var formdata = new FormData(this);

    $.ajax({
      type: "POST",
      url: "/OrderAppWaitingList/AddWaitingToken",
      data: formdata,
      contentType: false,
      processData: false,
      success: function (response) {
        // Close the offcanvas
        var modal = document.getElementById("waitingtokenmodal");
        bootstrap.Modal.getInstance(modal).hide();

        var sectionid = $("#section_list").find(".active").attr("section-id");
        LoadSectionList(sectionid);

        response.success
          ? toastr.success(response.message)
          : toastr.error(response.message);
      },
      error: function (xhr, status, error) {
        console.error("Error assigning table:", error);
      },
    });
  });

  function updateTimers() {
    $(".live-timer").each(function () {
      const createdAt = new Date($(this).data("createdat"));
      const now = new Date();
      const diffMs = now - createdAt;

      if (isNaN(diffMs)) {
        $(this).text("Invalid date");
        return;
      }

      const diffSecs = Math.floor(diffMs / 1000);
      const hours = Math.floor(diffSecs / 3600);
      const minutes = Math.floor((diffSecs % 3600) / 60);
      const seconds = diffSecs % 60;

      const formatted = `${hours}h ${minutes}min ${seconds}s`;
      $(this).text(formatted);
    });
  }

  // Initial call
  updateTimers();

  // Update every second
  setInterval(updateTimers, 1000);
});
