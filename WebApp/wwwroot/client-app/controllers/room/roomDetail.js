$(document).ready(function () {
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
});