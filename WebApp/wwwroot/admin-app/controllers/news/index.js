var NewsController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
        registerControls();
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
                txtTitleVi: {
                    required: true,
                    maxlength: 255,
                },
                txtSubDescription: {
                    required: true,
                    maxlength: 255,
                },
                txtSubDescriptionVi: {
                    required: true,
                    maxlength: 255,
                },
                txtSeoAlias: {
                    required: true,
                    maxlength: 255,
                },
                txtTag: {
                    required: true
                },
                txtDescM: {
                    maxlength: 500,
                },
                txtSeoPageTitle: {
                    maxlength: 255,
                },
                txtMetakeyword: {
                    maxlength: 255,
                },
                txtMetaDescription: {
                    maxlength: 255,
                }
            }
        });

        $('#txtKeyword').on('focusout', function (e) {
            e.preventDefault();
            loadData();
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
            $("#formMaintainance").validate().resetForm();
            $('#modal-add-edit').modal('show');
        });

        // Event click Select Img button
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
                    base.notify('Uploaded successful!', 'success');
                },
                error: function () {
                    base.notify('Has an error in progress', 'error');
                }
            });
        });

        // Event save
        $('#btnSave').on('click', function (e) {
            if ($('#formMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var title = $('#txtTitle').val();
                var titleVi = $('#txtTitleVi').val();
                var subDescription = $('#txtSubDescription').val();
                var subDescriptionVi = $('#txtSubDescriptionVi').val();
                var imageUrl = $('#txtImage').val();
                var seoPageTitle = $('#txtSeoPageTitle').val();
                var seoAlias = $('#txtSeoAlias').val();
                var seoKeywords = $('#txtSeokeywords').val();
                var seoDescription = $('#txtSeoDescription').val();
                var tags = $("#txtTag").val();
                var status = $('#ckStatus').prop('checked') == true ? 1 : 0;
                var content = CKEDITOR.instances.txtContent.getData();
                var contentVi = CKEDITOR.instances.txtContentVi.getData();

                $.ajax({
                    type: "POST",
                    url: "/Admin/News/SaveEntity",
                    data: {
                        Id: id,
                        title: title,
                        titleVi: titleVi,
                        subDescription: subDescription,
                        subDescriptionVi: subDescriptionVi,
                        imageUrl: imageUrl,
                        Content: content,
                        ContentVi: contentVi,
                        Tags: tags,
                        Status: status,
                        SeoPageTitle: seoPageTitle,
                        SeoAlias: seoAlias,
                        SeoKeywords: seoKeywords,
                        SeoDescription: seoDescription,
                    },
                    dataType: "json",
                    beforeSend: function () {
                        base.startLoading();
                    },
                    success: function (response) {
                        if (response.succeeded) {
                            base.notify(response.messages[0], 'success');
                            $('#modal-add-edit').modal('hide');
                            resetFormMaintainance();
                            base.stopLoading();
                            loadData(true);
                        } else {
                            base.notify(response.messages[0], 'error');
                        }
                    },
                    error: function () {
                        base.notify('Has an error in progress', 'error');
                        base.stopLoading();
                    }
                });
                return false;
            }
        });

        // Event button edit
        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.setTitleModal('edit');
            $("#formMaintainance").validate().resetForm();
            loadDetail(id);
        });

        // Event button delete
        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.confirm('Are you sure want to delete?', function () {
                deteleItem(id);
            });
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/news/GetAllPaging",
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
                            Title: item.title,
                            TitleVi: item.titleVi,
                            Id: item.id,
                            Status: item.status == 1 ? "Active" : "Draft",
                            SeoAlias: item.seoAlias,
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
                }
                else {
                    $('#tbl-content').html('');
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
            url: "/admin/news/GetById",
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
                $('#txtTitleVi').val(data.titleVi);
                $('#txtSubDescription').val(data.subDescription);
                $('#txtSubDescriptionVi').val(data.subDescriptionVi);
                $('#txtImage').val(data.imageUrl);
                $('#txtImageShow').val(data.imageUrl);
                $('#txtContent').val(data.content);
                $('#txtContentVi').val(data.contentVi);
                $('#txtSeoPageTitle').val(data.seoPageTitle);
                $('#txtSeoAlias').val(data.seoAlias);
                $('#txtSeokeywords').val(data.seoKeywords);
                $('#txtSeoDescription').val(data.seoDescription);
                $('#txtTag').val(data.tags);
                $('#ckStatus').prop('checked', data.status == 1);
                CKEDITOR.instances.txtContent.setData(data.content);
                CKEDITOR.instances.txtContentVi.setData(data.contentVi);

                $('#modal-add-edit').modal('show');
                base.stopLoading();
            },
            error: function (status) {
                base.notify('Has an error in progress', 'error');
                base.stopLoading();
            }
        });
    }

    var deteleItem = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/news/Delete",
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
                base.notify('Has an error in progress', 'error');
                base.stopLoading();
            }
        });
    }

    var registerControls = function () {
        CKEDITOR.replace('txtContent', {});
        CKEDITOR.replace('txtContentVi', {});

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
        $('#txtTitle').val('');
        $('#txtTitleVi').val('');
        $('#txtSubDescription').val('');
        $('#txtSubDescriptionVi').val('');
        $('#txtImage').val('');
        $('#txtImageShow').val('');
        $('#txtContent').val('');
        $('#txtContentVi').val('');
        $('#txtSeoPageTitle').val('');
        $('#txtSeoAlias').val('');
        $('#txtMetakeyword').val('');
        $('#txtMetaDescription').val('');
        $('#txtTag').val('');
        $('#ckStatus').prop('checked', true);
        CKEDITOR.instances.txtContent.setData('');
        CKEDITOR.instances.txtContentVi.setData('');
    }
}