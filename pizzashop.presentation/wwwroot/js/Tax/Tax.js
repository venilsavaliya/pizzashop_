// pagination function

function TableListPaginationAjax(pageSize, pageNumber) {

    let id = $("#section-list .category-active-option").attr("section-id");

    let searchkeyword = $("#taxitem-search-field").val();

    console.log("inside item", searchkeyword)
    console.log("get id", id)

    $.ajax({
        url: "/Tax/GetTaxList",
        data: { 'pageSize': pageSize, 'pageNumber': pageNumber, 'searchKeyword': searchkeyword },
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#taxlistcontainer").html(data);
        },
        error: function () {
            $("#taxlistcontainer").html('An error has occurred');
        }
    });
}

//set data for edit tax 
function setEditTaxData(ele) {

    var c = JSON.parse(ele.getAttribute("item-obj"));
    console.log(c);

    var editsectionitem = document.getElementById("EditTaxmodal");
    editsectionitem.querySelector("#taxIdForEdit").value = c.taxId;
    editsectionitem.querySelector("#taxNameForEdit").value = c.taxName;
    editsectionitem.querySelector("#typeOfTaxForEdit").value = c.type;
    editsectionitem.querySelector("#taxAmountForEdit").value = c.taxAmount;
    editsectionitem.querySelector("#isEnabledForEdit").checked = c.isenable;
    editsectionitem.querySelector("#isDefaultForEdit").checked = c.isdefault;

}

//delete tax
function setDeleteTaxData(element) {

    var Id = element.getAttribute("tax-id");
    var deleteBtn = document.getElementById("deleteTaxBtn");
    deleteBtn.href = '/Tax/DeleteTax' + '?id=' + Id;

}

$(document).ready(function () {

    TableListPaginationAjax();

    //keyup search
    document.getElementById("taxitem-search-field").addEventListener('keyup', () => {
        TableListPaginationAjax();
    })

    // add tax valiadtion rule
    $("#AddTaxForm").validate({
        rules: {
            "Taxes.TaxName": {
                required: true,
            },
            "Taxes.Type": {
                required: true,
            },
            "Taxes.TaxAmount": {
                required: true,
                number:true,
                min:0
            },
        },
        messages: {
            "Taxes.TaxName": {
                required: "Please enter a Tax name",
            },
            "Taxes.Type": {
                required: "Please select Tax Type",
            },
            "Taxes.TaxAmount": {
                required: "Please select Tax Amount",
                number:"Tax Amount Must be Number",
                min:"Tax Amount can not be negative"
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
    // edit tax valiadtion rule
    $("#EditTaxForm").validate({
        rules: {
            "Taxes.TaxName": {
                required: true,
            },
            "Taxes.Type": {
                required: true,
            },
            "Taxes.TaxAmount": {
                required: true,
                number:true,
                min:0
            },
        },
        messages: {
            "Taxes.TaxName": {
                required: "Please enter a Tax name",
            },
            "Taxes.Type": {
                required: "Please select Tax Type",
            },
            "Taxes.TaxAmount": {
                required: "Please select Tax Amount",
                number:"Tax Amount Must be Number",
                min:"Tax Amount can not be negative"
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
    $('#AddTaxmodal').on('hidden.bs.modal', function () {
        console.log("Modal hidden!");

        // Reset the form inside the modal
        $(this).find('form')[0].reset();

        // Reset ASP.NET Core validation
        var validator = $(this).find('form').validate();
        validator.resetForm();

        // Remove invalid classes
        $(this).find('.is-invalid').removeClass('is-invalid');

        // Optional: Clear validation summary if you're using one
        $(this).find('.validation-summary-errors').html('');
    });
    // Reset Edit Form When Modal Close
    $('#EditTaxmodal').on('hidden.bs.modal', function () {
        console.log("Modal hidden!");

        // Reset the form inside the modal
        $(this).find('form')[0].reset();

        // Reset ASP.NET Core validation
        var validator = $(this).find('form').validate();
        validator.resetForm();

        // Remove invalid classes
        $(this).find('.is-invalid').removeClass('is-invalid');

        // Optional: Clear validation summary if you're using one
        $(this).find('.validation-summary-errors').html('');
    });
})
