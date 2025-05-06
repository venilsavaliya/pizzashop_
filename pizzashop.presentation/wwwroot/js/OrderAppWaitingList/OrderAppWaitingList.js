// open Add Edit Waiting Token Modal
function openAddEditWaitingTokenModal(id) {
  var sectionid = $("#section_list").find(".active").attr("section-id");
  $.ajax({
    type: "GET",
    url: "/OrderAppWaitingList/GetAddEditWaitingTokenForm",
    data: { tokenid: id, activeSectionId: sectionid },
    success: function (data) {
      $("#AddEditWaitingTokenModalContent").html(data);
      var modal = new bootstrap.Modal(
        document.getElementById("AddEditWaitingTokenModal")
      );
      modal.show();
    },
  });
}

// submit add edit waiting token form
$(document).on("submit", "#AddEditWaitingTokenForm", function (e) {
  e.preventDefault();

  if (!$(this).valid()) {
    return;
  }

  var formdata = new FormData(this);

  $.ajax({
    type: "POST",
    url: "/OrderAppWaitingList/AddEditWaitingToken",
    data: formdata,
    contentType: false,
    processData: false,
    success: function (response) { 
      if (response.success) {
        toastr.success(response.message);

        // Close the Modal
        var modal = document.getElementById("AddEditWaitingTokenModal");
        bootstrap.Modal.getInstance(modal).hide();

        var sectionid = $("#section_list").find(".active").attr("section-id");
        LoadSectionList(sectionid);
        
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      console.error("Error assigning table:", error);
    },
  });
});

// logic for fill detail of already existed customer in waiting token form
$(document).on(
  "input",
  "#AddEditWaitingTokenForm input[name='Customer.Email']",
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
          $("#AddEditWaitingTokenForm input[name='Customer.Name']").val(
            data.customer.name
          );
          $("#AddEditWaitingTokenForm input[name='Customer.Mobile']").val(
            data.customer.mobile
          );
        },
      });
    }
  }
);

const selectedTables = new Set();

// update selected table name in box
function updateSelectedTableNames() {
  let names = Array.from(selectedTables)
    .map((item) => item.name)
    .join(", ");
  if (names.length === 0) names = "Select Table";
  $("#tableSelectBox").text(names);
}

// open Table Assign Form
function openAssignTableModal(ele) {
  const sectionid = $(ele).attr("section-id");
  const totalperson = $(ele).attr("total-person");
  const tokenid = $(ele).attr("token-id");
  const form = $("#TableAssignForm");

  form.find("select[name='SectionId']").val(sectionid);
  form.find("input[name='TotalPerson']").val(totalperson);
  form.find("input[name='TokenId']").val(tokenid);

  selectedTables.clear(); // clear previous selections
  updateSelectedTableNames();

  $.ajax({
    type: "GET",
    url: "/OrderAppWaitingList/GetAvailableTableList",
    data: { SectionId: sectionid },
    success: function (data) {
      const tables = data;
      console.log(tables);
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
              value="${table.tableId}" id="table_${table.tableId}" data-name="${table.name}" data-capacity="${table.capacity}">
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

// function to load  Waiting List
function LoadWaitingList(sectionid) {

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

// open modal for delete waiting token
function openDeleteTokenModal(tokenid) {
  var deleteBtn = document.getElementById("deleteWaitingTokenBtn");
  deleteBtn.setAttribute("token-id", tokenid);

  var modal = new bootstrap.Modal(
    document.getElementById("deleteWaitingTokenModal")
  );
  modal.show();
}

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
      LoadSectionList(sectionid);
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

$(document).ready(function () {


  $(document).on("change", ".table-checkbox", function () {
    const tableId = $(this).val();
    const tableName = $(this).data("name");
    const tableCapacity = $(this).data("capacity");

    if ($(this).is(":checked")) {
      selectedTables.add({
        id: tableId,
        name: tableName,
        capacity: tableCapacity,
      });
    } else {
      // Remove the unchecked one
      const toRemove = [...selectedTables].find((item) => item.id == tableId);
      if (toRemove) selectedTables.delete(toRemove);
    }
    console.log(selectedTables);
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

  // Load waitinglist for all section first
  LoadSectionList();


  $("#TableAssignForm").on("submit", function (e) {
    e.preventDefault();

    var formdata = new FormData(this);
    var capacity = 0;
    console.log("selected", selectedTables);
    if (selectedTables.size == 0) {
      toastr.error("Please Select Table(s) For Assign!");
      return;
    }

    selectedTables.forEach((i) => {
      formdata.append("TableId", i.id);
      capacity += parseInt(i.capacity);
    });

    if (capacity < formdata.get("TotalPerson")) {
      toastr.error("Selected Capcity is Less Than Required!");
      return;
    }

    $.ajax({
      type: "POST",
      url: "/OrderAppTable/AssignTable",
      data: formdata,
      contentType: false,
      processData: false,
      success: function (response) {
        var sectionid = $("#section_list").find(".active").attr("section-id");
        LoadSectionList(sectionid);

        response.success
          ? toastr.success(response.message)
          : toastr.error(response.message);

        var modal = bootstrap.Modal.getInstance(
          document.getElementById("assignTableModal")
        );
        modal.hide();
      },
      error: function (xhr, status, error) {
        console.error("Error assigning table:", error);
      },
    });
  });

  // Initial call
  updateTimers();

  // Update every second
  setInterval(updateTimers, 1000);
});
