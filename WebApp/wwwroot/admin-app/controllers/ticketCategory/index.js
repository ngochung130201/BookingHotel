var TicketCategoryController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }

    var registerEvents = function () {
        //Init validation
        $('#formMaintainance').validate({
            errorClass: 'text-danger',
            ignore: [],
            rules: {
                txtName: {
                    required: true,
                    maxlength: 255,
                },
                txtType: {
                    required: true,
                },
                txtPrice: {
                    required: true,
                },
                txtParentCode: {
                    required: true,
                },
                txtSymbol: {
                    required: true,
                    maxlength: 10,
                },
                txtClass: {
                    required: true,
                },
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

        // Event save
        $('#btnSave').on('click', function (e) {
            if ($('#formMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var name = $('#txtName').val();
                var type = $('#txtType').val();
                var price = $('#txtPrice').val();
                var parentCode = $('#txtParentCode').val();
                var symbol = $('#txtSymbol').val();
                var classTicket = $('#txtClass').val();
                var discount = $('#txtDiscount').val();
                var maxQuantity = $('#txtMaxQuantity').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/TicketCategory/SaveEntity",
                    data: {
                        Id: id,
                        Name: name,
                        Type: type,
                        Price: price,
                        ParentCode: parentCode,
                        Symbol: symbol,
                        Class: classTicket,
                        Discount: discount,
                        MaxQuantity: maxQuantity,
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
    const ticketTypes = {
        1: "Vé lượt",
        2: "Vé lẻ"
    };
    const ticketParentCodes = {
        1: "1",
        2: "2",
        3: "3"
    };
    const ticketClass = {
        1: "Sử dụng 1 lần",
    };
    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/ticketCategory/GetAllPaging",
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
                        var typeShow = 
                        render += Mustache.render(template, {
                            Order: stt,
                            Type: ticketTypes[item.type] || "-",
                            Name: item.name,
                            Price: item.price,
                            Id: item.id,
                            ParentCode: ticketParentCodes[item.parentCode] || "-",
                            Symbol: item.symbol,
                            Class: ticketClass[item.class] || "-",
                            Discount: item.discount,
                            MaxQuantity: item.maxQuantity,
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
            url: "/admin/ticketCategory/GetById",
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
                $('#txtType').val(data.type);
                $('#txtName').val(data.name);
                $('#txtPrice').val(data.price);
                $('#txtParentCode').val(data.parentCode);
                $('#txtSymbol').val(data.symbol);
                $('#txtClass').val(data.class);
                $('#txtDiscount').val(data.discount);
                $('#txtMaxQuantity').val(data.maxQuantity);

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
            url: "/admin/ticketCategory/Delete",
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

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#txtName').val('');
        $('#txtType').val('');
        $('#txtPrice').val('');
        $('#txtParentCode').val('');
        $('#txtSymbol').val('');
        $('#txtClass').val('');
        $('#txtDiscount').val('');
        $('#txtMaxQuantity').val('');
    }
}