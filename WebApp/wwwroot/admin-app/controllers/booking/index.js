var VoteBookingController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }

    var registerEvents = function () {
        // Event search 
        $('#txtKeyword').on('focusout', function (e) {
            e.preventDefault();
            loadData(true);
        });
        $('#txtKeyword').on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();
                loadData(true);
            }
        });

        // Event select page size
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Event button delete
        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.confirm('Bạn có chắc chắn muốn xóa?', function () {
                deteleItem(id);
            });
        });
    }

    // Event change status 
    $('body').on('click', '.btn-status', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        changeStatus(id);
    });
    var changeStatus = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/Booking/ChangeStatus",
            data: {
                id: id
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                if (response.succeeded) {
                    base.notify(response.messages[0], 'success');
                    loadData(true);
                }
                else {
                    base.notify(response.messages[0], 'error');
                }
                base.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/Admin/Booking/GetAllPaging",
            data: {
                keyword: $('#txtKeyword').val(),
                pageNumber: base.configs.pageIndex,
                pageSize: base.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                setTimeout(function () {
                    $('#spinnerRow').show();
                    base.startLoading(); 
                }, 2000);
                $('#spinnerRow').show();
                base.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                $("#lbl-total-records").text(response.totalCount);
                if (response.totalCount > 0) {
                    var stt = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                    $.each(response.data, function (i, item) {
                        render += Mustache.render(template, {
                            Order: stt,
                            Id: item.id,
                            FullName: item.fullName,
                            BookingCode: base.dateFormatJson(item.bookingCode),
                            TransactionDate: base.dateFormatJson(item.TransactionDate),
                            CheckInDate: item.CheckInDate,
                            BookedRoomNumber: item.BookedRoomNumber,
                            ServicesArising: item.ServicesArising,
                            TotalAmount: item.TotalAmount,
                            Status: getStatus(item.status, item.id),
                        });
                        stt++;
                    });
                    if (render !== undefined) {
                        $('#tbl-content').html(render);
                    }
                    base.wrapPaging(response.totalCount, function () {
                        loadData();
                    }, isPageChanged);
                } else {
                    $('#tbl-content').html('<tr><td colspan="10" style="text-align: center; vertical-align: middle;">Danh sách trống</td></tr>');
                }
                $('#spinnerRow').hide();
                base.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }

    var deteleItem = function (id) {
        $.ajax({
            type: "POST",
            url: "/Admin/Booking/Delete",
            data: {
                id: id
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                if (response.succeeded) {
                    base.notify(response.messages[0], 'success');
                    base.stopLoading();
                    loadData(true);
                } else {
                    base.notify(response.messages[0], 'error');
                    base.stopLoading();
                }
            },
            error: function (status) {
                base.notify('Đang xảy ra lỗi!', 'error');
                base.stopLoading();
            }
        });
    }

    var getStatus = function (status, id) {
        if (status == true)
            return '<button class="btn btn-sm btn-success btn-status" data-id="' + id + '">Kích hoạt</button>';
        else
            return '<button class="btn btn-sm btn-danger btn-status" data-id="' + id + '">Chặn</button>';
    }
}