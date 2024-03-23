
var NewsController = function () {
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

        // Image
        $('#btnSelectImg').on('click', function () {
            $('#fileInputImage').click();
        });

        $("#fileInputImage").on('change', function () {
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }
            $.ajax({
                type: "POST",
                url: "/Admin/Upload/UploadImage",
                contentType: false,
                processData: false,
                data: data,
                success: function (path) {
                    $('#txtImage').val(path);
                    $('#txtImageShow').val(path);
                    base.notify('Cập nhật thành công!', 'success');
                },
                error: function () {
                    base.notify('Đang xảy ra lỗi!', 'error');
                }
            });
        });

        $(document).ready(function () {
            $('#fileInputImage').change(function () {
                var file = this.files[0];
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#txtImageShow').val(file.name);
                    $('#txtImage').val(e.target.result);
                    $('#imagePreview').attr('src', e.target.result);
                };
                reader.readAsDataURL(file);
            });
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

    // Event change status 
    $('body').on('click', '.btn-status', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        changeStatus(id);
    });
    var changeStatus = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/News/ChangeStatus",
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

    // Event change hot 
    $('body').on('click', '.btn-hot', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        changeHot(id);
    });
    var changeHot = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/News/ChangeHot",
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
            url: "/Admin/News/GetAllPaging",
            data: {
                keyword: $('#txtKeyword').val(),
                pageNumber: base.configs.pageIndex,
                pageSize: base.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
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
                            Content: item.content,
                            Thumbnail: item.thumbnail === undefined || item.thumbnail === null || item.thumbnail === '' ? '<img src="/assets/images/user.png" width=50 />' : '<img src="' + base.getOrigin() + item.thumbnail + '" width=50 />',
                            Status: getStatus(item.status, item.id),
                            Hot: getHot(item.hot, item.id),
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
            url: "/Admin/News/GetById",
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
                $('#txtContent').val(data.content);   
                $('#txtImage').val(data.thumbnail);
                $('#imagePreview').attr('src', data.thumbnail != null ? base.getOrigin() + data.thumbnail : "/assets/images/user.png");
                $('#ckStatus').val(data.status);
                $('#ckHot').val(data.hot);

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
            url: "/Admin/News/Delete",
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
            return '<button class="btn btn-sm btn-success btn-status" data-id="' + id + '">Xuất bản</button>';
        else
            return '<button class="btn btn-sm btn-danger btn-status" data-id="' + id + '">Ngừng</button>';
    }

    var getHot = function (hot, id) {
        if (hot == true)
            return '<button class="btn btn-sm btn-success btn-hot" data-id="' + id + '">Nổi bật</button>';
        else
            return '<button class="btn btn-sm btn-info btn-hot" data-id="' + id + '">Thường</button>';
    }

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtTitle').val('');
        $('#txtContent').val('');
        $('#txtImage').val('');
        $('#txtImageShow').val('');
        $('#imagePreview').attr('src', '/assets/images/user.png');
        $('#ckStatus').prop('checked', true);
        $('#ckHot').prop('checked', true);
    }
    var saveData = function (continueFlg) {
        if ($('#formMaintainance').valid()) {
            var id = $('#hidId').val();
            var title = $('#txtTitle').val();
            var content = $('#txtContent').val();
            var thumbnail = $('#txtImage').val();
            var status = $('#ckStatus').prop('checked') == true ? true : false;
            var hot = $('#ckHot').prop('checked') == true ? true : false;
            $.ajax({
                type: "POST",
                url: "/Admin/News/SaveEntity",
                data: {
                    Id: id,
                    Title: title,
                    Content: content,
                    Thumbnail: thumbnail,
                    Status: status,
                    Hot: hot,
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