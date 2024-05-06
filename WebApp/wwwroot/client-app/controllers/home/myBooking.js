$(document).ready(function () {
    loadData();
});

var loadData = function () {
    $.ajax({
        type: "GET",
        url: "/home/GetAllPagingBooking",
        data: {
            keyword: $('#txtKeyword').val(),
        },
        dataType: "json",
        success: function (response) {
            var template = $('#table-template').html();
            var render = "";
            $("#lbl-total-records").text(response.totalCount);
            if (response.totalCount > 0) {
                $.each(response.data, function (i, item) {
                    render += Mustache.render(template, {
                        Order: i + 1,
                        Id: item.id,
                        BookingCode: item.bookingCode,
                        TransactionDate: item.TransactionDate,
                        TotalAmount: item.TotalAmount,
                        Status: getStatus(item.status, item.id),
                    });
                });
                if (render !== undefined) {
                    $('#tbl-content').html(render);
                }
            } else {
                $('#tbl-content').html('<tr><td colspan="6" style="text-align: center; vertical-align: middle;">@Localizer["You dont have any bookings yet"]</td></tr>');
            }
        },
        error: function () {
            alert("lỗi");
        }
    });
}

var getStatus = function (status, id) {
    if (status == true)
        return '<button class="btn btn-sm btn-success btn-status" data-id="' + id + '">@Localizer["Unpaid"]</button>';
    else
        return '<button class="btn btn-sm btn-danger btn-status" data-id="' + id + '">@Localizer["Paid"]</button>';
}