var FunctionController = function () {
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
                txtFunctionId: {
                    required: true,
                    maxlength: 255,
                },
                txtName: {
                    required: true,
                    maxlength: 255,
                }
            }
        });

        // Event click Add New button
        $("#btn-create").on('click', function () {
            base.setTitleModal('add');
            initTreeDropDownFunction();
            resetFormMaintainance();
            $("#formMaintainance").validate().resetForm();
            $('#modal-add-edit').modal('show');
        });

        // Event save
        $('#btnSave').on('click', function (e) {
            if ($('#formMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var functionId = $('#txtFunctionId').val();
                var name = $('#txtName').val();
                var url = $('#txtUrl').val();
                var parentId = $('#txtParentId').val();
                var iconCss = $('#txtIconCss').val();
                var sortOrder = $('#txtSortOrder').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/Function/SaveEntity",
                    data: {
                        Id: id,
                        FunctionId: functionId,
                        Name: name,
                        Url: url,
                        ParentId: parentId,
                        IconCss: iconCss,
                        SortOrder: sortOrder,
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
        $('body').on('click', '#btn-edit', function (e) {
            e.preventDefault();
            var id = $('#txtFunctionId').val();
            base.setTitleModal('edit');
            loadDetail(id);
        });

        // Event button delete
        $('body').on('click', '#btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.confirm('Are you sure want to delete?', function () {
                deteleItem(id);
            });
        });
    }

    function initTreeDropDownFunction(selectedId) {
        $.ajax({
            url: "/Admin/Function/GetAll",
            type: 'GET',
            dataType: 'json',
            async: false,
            success: function (response) {
                var data = [];
                $.each(response.data, function (i, item) {
                    data.push({
                        id: item.functionId,
                        text: item.name,
                        parentId: item.parentId,
                        sortOrder: item.sortOrder,
                    });
                });
                var arr = base.unflattern(data);
                $('#txtParentId').combotree({
                    data: arr
                });
                if (selectedId != undefined) {
                    $('#txtParentId').combotree('setValue', selectedId);
                }
            }
        });
    }
    function loadData() {
        $.ajax({
            url: '/Admin/Function/GetAll',
            dataType: 'json',
            success: function (response) {
                var data = [];
                $.each(response.data, function (i, item) {
                    data.push({
                        id: item.functionId,
                        text: item.name,
                        parentId: item.parentId,
                        sortOrder: item.sortOrder,
                    });

                });
                var treeArr = base.unflattern(data);
                treeArr.sort(function (a, b) {
                    return a.sortOrder - b.sortOrder;
                });
                $('#treeFunction').tree({
                    data: treeArr,
                    dnd: true,
                    onContextMenu: function (e, node) {
                        e.preventDefault();
                        // select the node
                        //$('#tt').tree('select', node.target);
                        $('#txtFunctionId').val(node.id);
                        // display context menu
                        $('#contextMenu').menu('show', {
                            left: e.pageX,
                            top: e.pageY
                        });
                    },
                    onDrop: function (target, source, point) {
                        var targetNode = $(this).tree('getNode', target);
                        if (point === 'append') {
                            var children = [];
                            $.each(targetNode.children, function (i, item) {
                                children.push({
                                    key: item.id,
                                    value: i
                                });
                            });

                            //Update to database
                            $.ajax({
                                url: '/Admin/Function/UpdateParentId',
                                type: 'post',
                                dataType: 'json',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id,
                                    items: children
                                },
                                success: function (res) {
                                    loadData();
                                }
                            });
                        }
                        else if (point === 'top' || point === 'bottom') {
                            $.ajax({
                                url: '/Admin/Function/ReOrder',
                                type: 'post',
                                dataType: 'json',
                                data: {
                                    sourceId: source.id,
                                    targetId: targetNode.id
                                },
                                success: function (res) {
                                    loadData();
                                }
                            });
                        }
                    }
                });
            }
        });
    }

    var loadDetail = function (id) {
        $.ajax({
            type: "GET",
            url: "/admin/Function/GetById",
            data: {
                functionId: id
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                var data = response.data;
                $('#hidId').val(data.id);
                $('#txtFunctionId').val(data.functionId);
                $('#txtName').val(data.name);
                $('#txtUrl').val(data.uRL);
                initTreeDropDownFunction(data.parentId);
                $('#txtIconCss').val(data.iconCss);
                $('#txtSortOrder').val(data.sortOrder);

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
            url: "/admin/Function/Delete",
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
        $('#txtFunctionId').val('');
        $('#txtName').val('');
        $('#txtUrl').val('');
        initTreeDropDownFunction('');
        $('#txtIconCss').val('');
        $('#txtSortOrder').val('');
    }
}