let selectedTableList = [];

function toggleBorder(ele){
    console.log(ele)
    if(ele.getAttribute("table-status") != 1)
    {   
        toastr.warning("This table is not available");
        return;
    }
    $(ele).toggleClass("green_border");


    if ($(ele).hasClass("green_border")) {
        selectedTableList.push($(ele).attr("id"));
    }
    else
    {
        selectedTableList = selectedTableList.filter(item => item !== $(ele).attr("id"));
    }
}

// Load Section List 
function LoadSectionList()
    {
        $.ajax({
            type: "GET",
            url: "/OrderAppTable/GetSectionList",
            success: function (data) {
               $("#dining_table_container").html(data);
            },
            error: function (xhr, status, error) {
                console.error("Error loading section list:", error);
            }
        })
    }

$(document).ready(function () {

    LoadSectionList();

});