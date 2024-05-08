
var RoomsController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
        registerControls();
    }

    var registerEvents = function () {
        $('#downloadTemplateLink').on('click', function (e) {
            e.preventDefault();
            // call ajax to download file
            window.location.href = '/Admin/Room/DownloadTemplate';

        });
        $('#excelFileInput').on('change', function () {
            var fileName = $(this).val().split('\\').pop();
            $(this).next('.custom-file-label').addClass("selected").html(fileName);
        });

        //Init validation
        $('#formMaintainance').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                txtName: {
                    required: true,
                    maxlength: 255,
                },
                txtPrice: {
                    required: true,
                    digits: true
                },
                txtRoomTypeId: {
                    required: true,
                    maxlength: 255,
                },
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

        // List images
        $('body').off('click').on('click', '.btn-image', function (e) {
            e.preventDefault();
            $('#modal-list-image').modal('show');
            $('hidId').val($(this).data('id'));
        })

        //event click btn-exportExcel 
        $('#btn-exportExcel').on('click', function (e) {
            e.preventDefault();
            var keyword = $('#txtKeyword').val();
            window.location.href = '/Admin/Room/ExportExcel?keyword=' + keyword;
        });

        $('#importExcelButton').on('click', function (e) {
            // Reset form
            $('#importExcelForm')[0].reset();
            $('.custom-file-label').removeClass('selected').html('Chọn file...');

            // Ẩn thông báo lỗi
            $('#importErrorContainer').hide();
            $('#importErrorTable tbody').empty();
        });

        $('#submitImport').on('click', function (e) {
            e.preventDefault();
            //get file to param form data
            var file = $('#excelFileInput').get(0).files;
            if (file.length === 0) {
                base.notify('Vui lòng chọn tệp để nhập', 'error');
                return false;
            }
            // set file to form data
            var data = new FormData();
            data.append('file', file[0]);

            // call ajax to import data
            $.ajax({
                type: "POST",
                url: "/Admin/Room/ImportData",
                data: data,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    base.startLoading();
                },
                success: function (response) {
                    base.stopLoading();
                    $('#excelFileInput').val(''); // Reset input file
                    $('#excelFileInput').next('.custom-file-label').html('Chọn file...'); // Reset label

                    if (response.succeeded) {
                        base.notify(response.message, 'success');
                        $('#importExcelModal').modal('hide'); // Đóng modal sau khi gửi dữ liệu thành công
                        loadData(true); // Tải lại dữ liệu sau khi import thành công
                    } else {
                        var errorTableBody = $("#importErrorTable tbody");
                        errorTableBody.empty(); // Xóa các dòng hiện có

                        response.errors.forEach(function (error) {
                            var row = $("<tr></tr>");
                            row.append($("<td></td>").text(error));
                            errorTableBody.append(row);
                        });

                        $("#importErrorContainer").show();
                    }
                },
                error: function () {
                    base.notify('Đã xảy ra lỗi trong quá trình import.', 'error');
                    base.stopLoading();
                    $('#importExcelModal').modal('hide');
                    $('#excelFileInput').val(''); // Reset input file
                    $('#excelFileInput').next('.custom-file-label').html('Chọn file...'); // Reset label
                }
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

    $(document).ready(function () {
        // Event change status 
        $('body').on('click', '.btn-active', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            changeUserStatus(id);
        });
    });

    var changeUserStatus = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/Room/ChangeStatus",
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

    var getStatus = function (status, id) {
        if (status == true)
            return '<button class="btn btn-sm btn-success btn-active" data-id="' + id + '">Kích hoạt</button>';
        else
            return '<button class="btn btn-sm btn-danger btn-active" data-id="' + id + '">Chặn</button>';
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/Admin/Room/GetPagination",
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
                            Thumbnail: item.thumbnail === undefined || item.thumbnail === null || item.thumbnail === '' ? '<img src="/assets/images/picture.png" width=50 />' : '<img src="' + base.getOrigin() + item.thumbnail + '" width=50 />',
                            RoomCode: item.roomCode,
                            RoomTypeName: item.roomTypeName,
                            Price: item.price,
                            Status: getStatus(item.status, item.id),
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
            url: "/Admin/Room/GetById",
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
                $('#txtRoomTypeId').val(data.roomTypeId);
                $('#txtRoomCode').val(data.roomCode);
                $('#txtPrice').val(data.price);
                $('#txtLocation').val(data.location);
                $('#txtAcreage').val(data.acreage);
                $('#txtAdult').val(data.adult);
                $('#txtKid').val(data.kid);
                $('#txtViews').val(data.views);
                $('#ckStatus').val(data.status);
                $('#txtImage').val(data.thumbnail);
                $('#imagePreview').attr('src', data.thumbnail != null ? base.getOrigin() + data.thumbnail : "/assets/images/picture.png");
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
            url: "/Admin/Room/Delete",
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

    var registerControls = function () {
        CKEDITOR.replace('txtDescription', {});

        //Fix: cannot click on element ck in modal
        $.fn.modal.Constructor.prototype.enforceFocus = function () {
            $(document)
                .off('focusin.bs.modal') // guard against infinite focus loop
                .on('focusin.bs.modal', $.proxy(function (e) {
                    if (
                        this.$element[0] !== e.target && !this.$element.has(e.target).length
                        // CKEditor compatibility fix start.
                        && !$(e.target).closest('.cke_dialog, .cke').length
                        // CKEditor compatibility fix end.
                    ) {
                        this.$element.trigger('focus');
                    }
                }, this));
        };
    }

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtName').val('');
        $('#txtRoomTypeId').val('');
        $('#txtRoomCode').val('');
        $('#txtPrice').val('');
        $('#txtLocation').val('');
        $('#txtAcreage').val('');
        $('#txtAdult').val('');
        $('#txtKid').val('');
        $('#txtViews').val('');
        $('#ckStatus').prop('checked', true);
        $('#txtImage').val('');
        $('#txtImageShow').val('');
        $('#imagePreview').attr('src', '/assets/images/picture.png');
        $('#txtDescription').val('');
    }
    var saveData = function (continueFlg) {
        if ($('#formMaintainance').valid()) {
            var id = $('#hidId').val();
            var name = $('#txtName').val();
            var thumbnail = $('#txtImage').val();
            var roomTypeId = $('#txtRoomTypeId').val();
            var roomCode = $('#txtRoomCode').val();
            var price = $('#txtPrice').val();
            var location = $('#txtLocation').val();
            var acreage = $('#txtAcreage').val();
            var adult = $('#txtAdult').val();
            var kid = $('#txtKid').val();
            var views = $('#txtViews').val();
            var status = $('#ckStatus').prop('checked') == true ? true : false;
            var description = CKEDITOR.instances.txtDescription.getData();
            $.ajax({
                type: "POST",
                url: "/Admin/Room/SaveEntity",
                data: {
                    Id: id,
                    Name: name,
                    Thumbnail: thumbnail,
                    RoomTypeId: roomTypeId,
                    RoomCode: roomCode,
                    Price: price,
                    Location: location,
                    Acreage: acreage,
                    Adult: adult,
                    Kid: kid,
                    Views: views,
                    Status: status,
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