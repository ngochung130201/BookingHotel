
var ServicesController = function () {
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
                txtName: {
                    required: true,
                    maxlength: 255,
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
            url: "/Admin/Services/GetAllPaging",
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
                            Name: item.name,
                            Image: '<i class="' + item.image + '"></i>',
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
            url: "/Admin/Services/GetById",
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
                $('#txtName').val(data.name);
                $('#txtImage').val(data.image);
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
            url: "/Admin/Services/Delete",
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
        $('#txtName').val('');
        $('#txtImage').val('');
        $('#txtDescription').val('');
    }
    var saveData = function (continueFlg) {
        if ($('#formMaintainance').valid()) {
            var id = $('#hidId').val();
            var name = $('#txtName').val();
            var image = $('#txtImage').val();
            var description = $('#txtDescription').val();
            $.ajax({
                type: "POST",
                url: "/Admin/Services/SaveEntity",
                data: {
                    Id: id,
                    Name: name,
                    Image: image,
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
}