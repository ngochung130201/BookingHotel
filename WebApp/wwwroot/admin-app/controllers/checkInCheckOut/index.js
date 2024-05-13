var CheckInCheckOutController = function () {
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
        // Event search by role
        $("#searchRoomStatus").on('change', function () {
            loadData(true);
        });

        // Event select page size
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });  

        // Event save
        $('#btnSave').on('click', function (e) {
            e.preventDefault();
            saveData(false);
        });

        // Event button edit
        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.setTitleModal('edit');
            $("#formMaintainance").validate().resetForm();
            loadDetail(id);
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/Admin/CheckInCheckOut/GetAllPaging",
            data: {
                keyword: $('#txtKeyword').val(),
                StatusRoom: $('#searchRoomStatus').val(),
                pageNumber: base.configs.pageIndex,
                pageSize: base.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
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
                            Name: item.name,
                            RoomTypes: item.roomTypes,
                            Location: item.location,
                            StatusRoom: getRoomStatus(item.statusRoom, item.id),
                            CreatedOn: base.dateTimeFormatJson(item.createdOn)
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
                $('#spinnerRow').show();
                base.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtStatusRoom').val('');
    }

    var loadDetail = function (id) {
        $.ajax({
            type: "GET",
            url: "/Admin/CheckInCheckOut/GetById",
            data: {
                id: id
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                var data = response.data;
                $('#hidId').val(data.id);
                $('#txtStatusRoom').val(data.StatusRoom);

                $('#modal-add-edit').modal('show');
                base.stopLoading();
            },
            error: function (status) {
                base.notify('Đang xảy ra lỗi!', 'error');
                base.stopLoading();
            }
        });
    }

    var saveData = function () {
        if ($('#formMaintainance').valid()) {
            var id = $('#hidId').val();
            var statusRoom = $('#txtStatusRoom').val();
            $.ajax({
                type: "POST",
                url: "/Admin/CheckInCheckOut/SaveEntity",
                data: {
                    Id: id,
                    StatusRoom: statusRoom,
                },
                dataType: "json",
                beforeSend: function () {
                    base.startLoading();
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        resetFormMaintainance();
                        base.stopLoading();
                        $('#modal-add-edit').modal('hide');
                        loadData(true);
                    } else {
                        base.notify(response.messages[0], 'error');
                    }
                },
                error: function () {
                    base.notify('Đang xảy ra lỗi!', 'error');
                    base.stopLoading();
                }
            });
            return false;
        }
    }

    var getRoomStatus = function (status, id) {
        if (status == 0)
            return '<button class="btn btn-sm btn-success btn-memberStatus" data-id="' + id + '" data-status="1">Sẳn sàng phục vụ</button>';
        else if (status == 1)
            return '<button class="btn btn-sm btn-warning btn-memberStatus" data-id="' + id + '" data-status="2">Đang phục vụ</button>';
        else if (status == 2)
            return '<button class="btn btn-sm btn-danger btn-memberStatus" data-id="' + id + '" data-status="3">Đang kiểm tra</button>';
    }
}