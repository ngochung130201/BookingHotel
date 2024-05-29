$(document).ready(function () {
    // Xử lý formMaintainance submit
    $('#formMaintainance').submit(function (e) {
        e.preventDefault(); // Ngăn chặn gửi form mặc định

        // Lấy dữ liệu từ form
        var formData = $(this).serialize();

        // Gửi AJAX request
        $.ajax({
            type: 'POST',
            url: '/Room/SaveEntity',
            data: formData,
            success: function (response) {
                // Xử lý response ở đây nếu cần
                console.log(response);
                alert("Send comment success");
                location.reload();
            },
            error: function () {
                alert("Failed to send comment");
            }
        });
    });

    // Xử lý booking-form submit
    $('#booking-form').submit(function (e) {
        e.preventDefault(); // Ngăn chặn gửi form mặc định
        
        // Lấy dữ liệu từ form và gán vào một đối tượng formData
        var formData = {
            Id: $('#hidId').val(),
            TotalAmount: $('#txtTotalAmount').val(),
            RoomId: $('#txtRoomId').val(),
            Adult: $('#Adult').val(),
            Kid: $('#Kid').val(),
            CheckInDate: $('#CheckInDate').val(),
            CheckOutDate: $('#CheckOutDate').val(),
            PhoneNumber: $('[name="PhoneNumber"]').val(),
            RoomName: $('[name="RoomName"]').val(),
            Adult: $('#Adult').val(),
            Kid: $('#Kid').val(),
            CheckInDate: $('#CheckInDate').val(),
            CheckOutDate: $('#CheckOutDate').val(),
            // Lấy giá trị của các checkbox đã chọn
            SelectedCostIds: $('input[name="SelectedCostIds[]"]:checked').map(function() {
               
                return $(this).val();
            }).get()

        };

        // Gửi AJAX request
        $.ajax({
            type: 'POST',
            url: '/Room/SaveBooking',
            data: formData,
            success: function (response) {
                console.log(response);
                alert("Booking success!");
                location.reload();
            },
            error: function () {
                alert("Failed to booking!");
            }
        });
    });
});
