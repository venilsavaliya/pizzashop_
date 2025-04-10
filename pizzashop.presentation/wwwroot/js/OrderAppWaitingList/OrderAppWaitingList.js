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

function setEditDataForWaitingToken(ele)
{
  const itemData = $(ele).data("item"); // jQuery

  console.log(itemData)

  const form = $("#addWaitingTokenForm");

  form.find("input[name='Customer.Email']").val(itemData.email)
  form.find("input[name='Customer.Mobile']").val(itemData.mobile)
  form.find("input[name='Customer.Name']").val(itemData.name)
  form.find("input[name='Customer.TotalPerson']").val(itemData.totalperson)
  form.find("input[name='TokenId']").val(itemData.tokenid)
  form.find("input[name='SectionId']").val(itemData.sectionid)
  console.log("heloo bhai")
  openWaitingTokenModal();
  
}

// open modal for waiting token

function openWaitingTokenModal() {
  var modal = new bootstrap.Modal(document.getElementById("waitingtokenmodal"));
  modal.show();

  var sectionid = $("#section_list").find(".active").attr("section-id");
  if (sectionid == null) {
    sectionid = "";
  }
  var sectionElement = $('#addWaitingTokenForm [name="SectionId"]');
  sectionElement.val(sectionid);
}

$(document).ready(function () {
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

        var sectionid = $("#section_list").find(".active").attr("section-id")
        LoadSectionList(sectionid);
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
