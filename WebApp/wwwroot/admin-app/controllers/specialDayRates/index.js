
var SpecialDayRatesController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }

    var registerEvents = function () {
        //Init validation
        $('#formMaintainance').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                txtTitle: {
                    required: true,
                    maxlength: 255,
                },
                txtPercentDiscount: {
                    required: true,
                    range: [0, 100]
                }
            },
            messages: {
                txtPercentDiscount: {
                    required: "Vui lòng nhập giá trị",
                    range: "Giá trị phải từ 0 đến 100"
                }
            }
        });

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

        // Event click Add New button
        $("#btn-create").on('click', function () {
            base.setTitleModal('add');
            resetFormMaintainance();
            $('#btnSaveAndContinue').show();
            $("#formMaintainance").validate().resetForm();
            $('#modal-add-edit').modal('show');
        });

        // Event save
        $('#btnSave').on('click', function (e) {
            e.preventDefault();
            saveData(false);
        });

        // Event save
        $('#btnSaveAndContinue').on('click', function (e) {
            e.preventDefault();
            saveData(true);
        });


        // Event button edit
        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $('#btnSaveAndContinue').hide();
            base.setTitleModal('edit');
            $("#formMaintainance").validate().resetForm();
            loadDetail(id);
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

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/Admin/SpecialDayRates/GetAllPaging",
            data: {
                keyword: $('#txtKeyword').val(),
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
                            Title: item.title,
                            PercentDiscount: item.percentDiscount,
                            SinceDay: base.dateFormatJson(item.sinceDay),
                            ToDay: base.dateFormatJson(item.toDay),
                            Description: item.description,
                            CreatedBy: item.createdBy,
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


    var loadDetail = function (id) {
        $.ajax({
            type: "GET",
            url: "/Admin/SpecialDayRates/GetById",
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
                $('#txtTitle').val(data.title);
                $('#txtPercentDiscount').val(data.percentDiscount);
                $('#txtSinceDay').val(base.dateFormatJson(data.sinceDay));
                $('#txtToDay').val(base.dateFormatJson(data.toDay));
                $('#txtDescription').val(data.description);

                $('#modal-add-edit').modal('show');
                base.stopLoading();
            },
            error: function (status) {
                base.notify('Đang xảy ra lỗi!', 'error');
                base.stopLoading();
            }
        });
    }

    var deteleItem = function (id) {
        $.ajax({
            type: "POST",
            url: "/Admin/SpecialDayRates/Delete",
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

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtTitle').val('');
        $('#txtPercentDiscount').val('');
        $('#txtSinceDay').val('');
        $('#txtToDay').val('');
        $('#txtDescription').val('');
    }

    var saveData = function (continueFlg) {
        if ($('#formMaintainance').valid()) {
            var id = $('#hidId').val();
            var title = $('#txtTitle').val();
            var percentDiscount = $('#txtPercentDiscount').val();
            var sinceDay = $('#txtSinceDay').val();
            var toDay = $('#txtToDay').val();
            var description = $('#txtDescription').val();
            $.ajax({
                type: "POST",
                url: "/Admin/SpecialDayRates/SaveEntity",
                data: {
                    Id: id,
                    Title: title,
                    PercentDiscount: percentDiscount,
                    SinceDay: base.convertToISODateString(sinceDay),
                    ToDay: base.convertToISODateString(toDay),
                    Description: description,
                },
                dataType: "json",
                beforeSend: function () {
                    base.startLoading();
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        (continueFlg === true) ? resetFormMaintainance() : $('#modal-add-edit').modal('hide');
                        resetFormMaintainance();
                        base.stopLoading();
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

    $("#txtSinceDay").datepicker({
        dateFormat: "dd/mm/yy"
    });

    $("#openDatePicker1").on('click', function () {
        $("#txtSinceDay").datepicker("show");
    });

    $("#txtSinceDay").on('focus', function () {
        $(this).datepicker("show");
    });

    //
    $("#txtToDay").datepicker({
        dateFormat: "dd/mm/yy"
    });

    $("#openDatePicker2").on('click', function () {
        $("#txtToDay").datepicker("show");
    });

    $("#txtToDay").on('focus', function () {
        $(this).datepicker("show");
    });
}