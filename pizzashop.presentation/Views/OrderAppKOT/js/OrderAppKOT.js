// function to load in-progress orders

function LoadInProgressOrders()
    {
        const categoryid = $('.active').attr('category-id');
        $.ajax({
            url: "/OrderAppKOT/GetOrderDetails",
            type: "GET",
            data : {categoryid: categoryid,isPending: true},
            success: function (data) {
                $("#cardWrapper").html(data);
            },
            error: function (error) {
                console.log("Error loading in-progress orders:", error);
            }
        });
    }

// function to load in-progress orders

function LoadReadyOrders()
    {
        const categoryid = $('.active').attr('category-id');
        $.ajax({
            url: "/OrderAppKOT/GetOrderDetails",
            type: "GET",
            data : {categoryid: categoryid,isPending: false},
            success: function (data) {
                $("#cardWrapper").html(data);
            },
            error: function (error) {
                console.log("Error loading in-progress orders:", error);
            }
        });
    }

$(document).ready(function(){

    // $("#inProgressBtn").on("click", function(){

    // });

    
})