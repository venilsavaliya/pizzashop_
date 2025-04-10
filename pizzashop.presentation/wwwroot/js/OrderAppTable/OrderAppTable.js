let selectedTableList = [];
// Logic For Select Table to Add into Selected Table List
function toggleBorder(ele) {
  console.log(ele);
  if (ele.getAttribute("table-status") != 1) {
    toastr.warning("This table is not available");
    return;
  }
  $(ele).toggleClass("green_border");

  if ($(ele).hasClass("green_border")) {
    selectedTableList.push($(ele).attr("table-id"));
  } else {
    selectedTableList = selectedTableList.filter(
      (item) => item !== $(ele).attr("table-id")
    );
  }
}

// Load Section List 
function LoadSectionList() {
  $.ajax({
    type: "GET",
    url: "/OrderAppTable/GetSectionList",
    success: function (data) {
      $("#dining_table_container").html(data);

      // handle logic whic execute during any accordian get collapsed

      $('.accordion-collapse').on('hidden.bs.collapse', function (e) {
        console.log('Accordion closed:');

        //empty the selected table list
        selectedTableList=[];

       $(this).find('.green_border').removeClass('green_border')
        
      });
    },
    error: function (xhr, status, error) {
      console.error("Error loading section list:", error);
    },
  });
}

// open modal for waiting token

function openWaitingTokenModal(ele) {
  var modal = new bootstrap.Modal(document.getElementById("waitingtokenmodal"));
  modal.show();

  var sectionid = $(ele).attr("section-id");
  var sectionElement = $('#addWaitingTokenForm [name="SectionId"]');
  sectionElement.val(sectionid);
}

// open assign offcanvas

function openAssignOffcanvas(ele) {
  // Open the offcanvas manually
  var offcanvasElement = document.getElementById("offcanvasRight");
  var bsOffcanvas = new bootstrap.Offcanvas(offcanvasElement);
  bsOffcanvas.show();

  // Set SectionId value (example)
  var sectionid = $(ele).attr("section-id");
  var sectionElement = $('#TableAssignForm [name="SectionId"]');
  sectionElement.val(sectionid);
}

$(document).ready(function () {
  // When an accordion item is about to be collapsed
  $(".accordion-collapse").on("hidden.bs.collapse", function () {
    // SelectedTableIds = [];
    console.log("Accordion closed", SelectedTableIds);
  });
  LoadSectionList();

  // Add Waiting Token Validations
  $("#addWaitingTokenForm").validate({
    rules: {
      CustomerEmail: {
        required: true,
        email: true,
      },
      CustomerName: {
        required: true,
      },
      CustomerPhone: {
        required: true,
        digits: true,
        minlength: 10,
        maxlength: 10,
      },
      NoOfPeople: {
        required: true,
        number: true,
        min: 1,
      },
      SectionId: {
        required: true,
      },
    },
    messages: {
      CustomerEmail: {
        required: "Please enter the Email",
        email: "Please enter a valid Email (e.g. mailto:name@example.com)",
      },
      CustomerName: {
        required: "Please enter a Name",
      },
      CustomerPhone: {
        required: "Please enter a Mobile Number",
        digits: "Please enter digits only",
        minlength: "Mobile number must be 10 digits",
        maxlength: "Mobile number must be 10 digits",
      },
      NoOfPeople: {
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
  // Assign Table Token Validations
  $("#TableAssignForm").validate({
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
      "SectionId": {
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
      "SectionId": {
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

  // reset validation after offcanvas close

  $("#offcanvasRight").on("hidden.bs.offcanvas", function () {
    // Reset form fields
    $("#TableAssignForm")[0].reset();

    // Reset validation
    var validator = $("#TableAssignForm").validate();
    validator.resetForm();

    // Remove is-invalid classes
    $("#addWaitingTokenForm ").find(".is-invalid").removeClass("is-invalid");
  });


  // submit waiting token form

  $("#addWaitingTokenForm").on("submit", function (e) {
    e.preventDefault(); 

    if (!$(this).valid()) {
      return;
    }
    console.log("submitted!")
  });

  // submit assign table form

  $("#TableAssignForm").on("submit", function (e) {

    e.preventDefault(); 

    if (!$(this).valid()) {
      return;
    }

    var formdata = new FormData(this);
    selectedTableList.forEach(function (id) {
      formdata.append("TableId", id);
    });
    console.log("Selected Table List:", selectedTableList);
    $.ajax({
      type: "POST",
      url: "/OrderAppTable/AssignTable",
      data: formdata,
      contentType: false,
      processData: false,
      success: function (response) {
        console.log("Table assigned successfully");
        // Close the offcanvas
        var offcanvasElement = document.getElementById("offcanvasRight");
        var bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);
        bsOffcanvas.hide();

        selectedTableList=[];
        console.log("done",selectedTableList);
        LoadSectionList();
      },
      error: function (xhr, status, error) {
        console.error("Error assigning table:", error);
      },
    })
  });
});
 