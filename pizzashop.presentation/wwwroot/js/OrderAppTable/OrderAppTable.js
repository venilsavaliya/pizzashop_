const selectedTables = new Set(); 

//open add edit waiting token modal
function openAddEditWaitingTokenModal(ele, tableformid) {
  var sectionid = $(ele).attr("section-id");

  $.ajax({
    type: "GET",
    url: "/OrderAppWaitingList/GetAddEditWaitingTokenForm",
    data: { activeSectionId: sectionid, formId: tableformid },
    success: function (data) {
      if (tableformid != null && tableformid != "") {
        $("#TableAssignOffcanvasForm").html(data);
        $("#TableAssignOffcanvasForm").find(".modal-header").addClass("d-none");
        attachEventListnerForRadio();
      } else {
        $("#AddEditWaitingTokenModalContent").html(data);
        var modal = new bootstrap.Modal(
          document.getElementById("AddEditWaitingTokenModal")
        );
        modal.show();
      }
    },
  });
}

//submit table assign form
$("#TableAssignOffcanvasForm").on("submit", "#AssignTableForm", function (e) {
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

  if (capacity < formdata.get("Customer.TotalPerson")) {
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
      // response.success
      //   ? toastr.success(response.message)
      //   : toastr.error(response.message);

      // var offcanvasElement = document.getElementById("offcanvasRight");

      // var bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement);
      // bsOffcanvas.hide();

      // LoadSectionList();

      if(response.orderid == 0)
      {
        toastr.error("Customer Has Already Assigned A Token!")
        return;
      }
      else{
        window.location.href= "/OrderAppMenu/index?OrderId="+response.orderid;
      }
     

      // selectedTables.clear();
    },
    error: function (xhr, status, error) {
      console.error("Error assigning table:", error);
    },
  });
});

//submit add waiting token form
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
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      console.error("Error assigning table:", error);
    },
  });
});

// Logic For Select Table to Add into Selected Table List
function toggleBorder(ele) {

  if (ele.getAttribute("table-status") != 1) {
    
    //ajax call to get order id of the table

    var tableid = $(ele).attr('table-id');

    $.ajax({
      type:"Get",
      url:"/OrderAppTable/GetOrderIdOfTable",
      data:{tableid :tableid },
      success : function(data)
      {
        window.location.href= "/OrderAppMenu/index?orderid="+data;
      }
    })
    return;
  }

  $(ele).toggleClass("green_border");

  if ($(ele).hasClass("green_border")) {

    var tableId = $(ele).attr("table-id");
    var tableCapacity = $(ele).attr("table-capacity");

    selectedTables.add({
      id: tableId,
      capacity: tableCapacity,
    });

  } else {

    const toRemove = [...selectedTables].find((item) => item.id == tableId);

    if (toRemove) {
      selectedTables.delete(toRemove);
    }
    
  }
}

// Load Section List
function LoadSectionList() {
  
  $.ajax({
    type: "GET",
    url: "/OrderAppTable/GetSectionList",
    success: function (data) {
      $("#dining_table_container").html(data);

      //update time 
      updateTimers();
      setInterval(updateTimers, 1000);

      // handle logic whic execute during any accordian get collapsed

      $(".accordion-collapse").on("hidden.bs.collapse", function (e) {
        console.log("Accordion closed:");

        //empty the selected table list
        selectedTables.clear();

        $(this).find(".green_border").removeClass("green_border");
      });
    },
    error: function (xhr, status, error) {
      console.error("Error loading section list:", error);
    },
  });
}

// open assign offcanvas
function openAssignOffcanvas(ele) {
  // Load form in offcanvas
  openAddEditWaitingTokenModal(ele, "AssignTableForm");

  // Open the offcanvas manually
  var offcanvasElement = document.getElementById("offcanvasRight");
  var bsOffcanvas = new bootstrap.Offcanvas(offcanvasElement);
  bsOffcanvas.show();

  // Set SectionId value (example)
  var sectionid = $(ele).attr("section-id");
  var sectionElement = $('#TableAssignForm [name="SectionId"]');
  sectionElement.val(sectionid);

  // ajax call to get waiting list of customers

  $.ajax({
    url: "/OrderAppTable/GetWaitingList",
    type: "GET",
    data: { sectionid: sectionid },
    success: function (data) {
      $("#waitinglistCustomerDeatil").html(data);
      attachEventListnerForRadio();
    },
    error: function (error) {
      console.log("Error loading in-progress orders:", error);
    },
  });
}

// function to attach event listner on radio buttons to copy waiting list customer data in form directly
function attachEventListnerForRadio() {
  $(".RadionBtn").on("change", function () {
    var obj = $(this).data("obj");

 

    $("#AssignTableForm input[name='Customer.Email']").val(obj.email).addClass('disabled-toggle');
    $("#AssignTableForm input[name='Customer.Name']").val(obj.name).addClass('disabled-toggle');
    $("#AssignTableForm input[name='Customer.Mobile']").val(obj.mobile).addClass('disabled-toggle');
    $("#AssignTableForm input[name='Customer.TotalPerson']").val(
      obj.totalperson
    ).addClass('disabled-toggle');
    $("#AssignTableForm input[name='Tokenid']").val(obj.tokenid).addClass('disabled-toggle');
  });
}

$(document).ready(function () {
  LoadSectionList();
});


// logic for fill detail of already existed customer in waiting token form
$(document).on(
  "input",
  "#AddEditWaitingTokenForm input[name='Customer.Email'], #AssignTableForm input[name='Customer.Email']",
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
         
          $("#AddEditWaitingTokenForm input[name='Customer.Name']").val(
            data.customer.name
          );
          $("#AddEditWaitingTokenForm input[name='Customer.Mobile']").val(
            data.customer.mobile
          );
          $("#AssignTableForm input[name='Customer.Name']").val(
            data.customer.name
          );
          $("#AssignTableForm input[name='Customer.Mobile']").val(
            data.customer.mobile
          );
        },
      });
    }
  }
);
