﻿var page = 1;
var totalPages = parseInt($('#load-more-news').data('total-page'));
$('#load-more-news').on('click', function (e) {
    page++;
    $.ajax({
        url: '/blog/get-data-paging/' + page,
        type: 'GET',
        success: function (result) {
            $('#load-page-news').append(result);
            if (page >= totalPages) {
                $('#load-more-news').hide();
            }
        },
        error: function () {
            page--;
        }
    });
});

$(document).ready(function () {
    $('#formMaintainance').submit(function (e) {
        e.preventDefault();

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