var UserController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }

    var registerEvents = function () {
        // Event select page size
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Event search by role
        $("#searchRoleName").on('change', function () {
            loadData(true);
        });

        // Event search by keyword
        $("#txtKeyword").on('focusout', function () {
            loadData(true);
        });

        // Event change status IsActive of User
        $('body').on('click', '.btn-active', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            changeUserStatus(id);
        });

        // Event detete user
        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.confirm('Are you sure want to delete?', function () {
                deleteUser(id);
            });
        });

        // Event change member status
        $('body').on('click', '.btn-memberStatus', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var status = $(this).data('status');
            $('#hidId').val(id);
            // Check status and enable radio button
            if (status == 1) {
                $('#rdTrial').prop('checked', true);
            }
            else if (status == 2) {
                $('#rdVip1').prop('checked', true);
            }
            else if (status == 3) {
                $('#rdVip2').prop('checked', true);
            }
            $('#modal-memberStatus').modal('show');
        });

        // Event save in modal member status
        $('#btnSaveMemberStatus').on('click', function () {
            var id = $('#hidId').val();
            var status = $('input[name=rdMemberStatus]:checked').val();
            changeMemberStatus(id, status);
            $('#modal-memberStatus').modal('hide');
        });

        $("#btn-create").on('click', function () {
            base.setTitleModal('add');
            resetFormMaintainance();
            $("#formAddUser").validate().resetForm();
            $('#modal-add-edit').modal('show');
            var password = $("#password").val();
            if (password.length === 0) {
                $("#confirmPassword").prop("disabled", true).val("");
            }
        });

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
        $("#password").keyup(function () {
            var password = $("#password").val();
            if (password.length > 0) {
                $("#confirmPassword").prop("disabled", false).val("");
            }
        });
        $("#confirmPassword").keyup(function () {
            var password = $("#password").val();
            var confirmPassword = $(this).val();
            if (password != confirmPassword) {
                $("#passwordMessage").html("Mật khẩu không trùng khớp!").css("color", "red");
            } else {
                $("#passwordMessage").html("Mật khẩu trùng khớp.").css("color", "green");
            }
        });

        // Event save
        $('#btnSave').on('click', function (e) {
            if ($('#formAddUser').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var email = $('#email').val();
                var fullName = $('#fullName').val();
                var password = $('#password').val();
                var confirmPassword = $('#confirmPassword').val();
                var avatarUrl = $('#txtImage').val();
                var dateOfBirth = $('#dateOfBirth').val();
                var phoneNumber = $('#phoneNumber').val();
                var address = $('#address').val();
                var roles = $("#ddlRoleName").val();
                var isActive = $('#ckStatus').prop('checked') == true ? true : false;
                $.ajax({
                    type: "POST",
                    url: "/Admin/User/SaveEntity",
                    data: {
                        id: id,
                        email: email,
                        fullName: fullName,
                        avatarUrl: avatarUrl,
                        DateOfBirth: dateOfBirth,
                        phoneNumber: phoneNumber,
                        roles: roles,
                        isActive: isActive,
                        password: password,
                        confirmPassword: confirmPassword,
                        address: address
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

        $('#dateOfBirth').change(function () {
            var selectedDate = new Date($(this).val());
            var currentDate = new Date();
            if (selectedDate > currentDate) {
                $(this).val('');
                alert('Ngày sinh không thể sau ngày hiện tại.');
            }
        });

        $('#formAddUser').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                ddlRoleName: {
                    required: true,
                },
                email: {
                    required: true,
                },
                password: {
                    required: true,
                    maxlength: 255,
                },
                confirmPassword: {
                    required: true,
                    maxlength: 255,
                },
                fullName: {
                    required: true,
                    maxlength: 255,
                }
            }
        });

        // Event button edit
        $('body').on('click', '.btn-edit', function (e) {
            e.preventDefault();
            $('.removePass').css('display', 'none');
            var id = $(this).data('id');
            base.setTitleModal('edit');
            $("#formAddUser").validate().resetForm();
            loadDetail(id);
        });

        var loadDetail = function (id) {
            $.ajax({
                type: "GET",
                url: "/admin/user/GetById",
                data: {
                    id: id
                },
                dataType: "json",
                beforeSend: function () {
                    base.startLoading();
                },
                success: function (response) {
                    console.log("response", response)
                    $('#hidId').val(response.data.id);
                    $('#email').val(response.data.email);
                    $('#fullName').val(response.data.fullName);
                    $('#password').val("*");
                    $('#confirmPassword').val("*");
                    $('#txtImageShow').val(response.data.avatarUrl);
                    $('#txtImage').val(response.data.avatarUrl);
                    $('#imagePreview').attr('src', response.data.avatarUrl != null ? base.getOrigin() + response.data.avatarUrl : "/assets/images/user.png");
                    $('#dateOfBirth').val(base.dateFormatDateOfBirthJson(response.data.dateOfBirth));
                    $('#phoneNumber').val(response.data.phoneNumber);
                    $('#address').val(response.data.address);
                    $("#ddlRoleName option[value='" + response.data.roleName + "']").prop("selected", true);
                    $('#ckStatus').val(response.data.ckStatus);
                    $('#modal-add-edit').modal('show');
                    base.stopLoading();
                },
                error: function (status) {
                    base.notify('Has an error in progress', 'error');
                    base.stopLoading();
                }
            });
        }
        //Change Password User 
        $('body').on('click', '.btn-change-pass', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $('#hidId').val(id);
            $('#modal-change-password-user').modal('show');
        });

        $('#formChangePassword').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                passwordNew: {
                    required: true,
                },
                confirmPasswordNew: {
                    required: true,
                }
            }
        });
        $(document).ready(function () {
            $("#passwordNew").keyup(function () {
                var password = $("#passwordNew").val();
                if (password.length > 0) {
                    $("#confirmPasswordNew").prop("disabled", false).val("");
                    $("#passwordMessageChange").html("");
                }
            });

            $("#confirmPasswordNew").keyup(function () {
                var password = $("#passwordNew").val();
                var confirmPassword = $(this).val();
                if (password !== confirmPassword) {
                    $("#passwordMessageChange").html("Mật khẩu không trùng khớp!").css("color", "red");
                } else {
                    $("#passwordMessageChange").html("Mật khẩu trùng khớp.").css("color", "green");
                }
            });

            $('#btn-save-change-password').on('click', function (e) {
                if ($('#formChangePassword').valid()) {
                    e.preventDefault();
                    var id = $('#hidId').val();
                    var passwordNew = $('#passwordNew').val();
                    var confirmPasswordNew = $('#confirmPasswordNew').val();
                    if (password !== confirmPassword) {
                        $("#passwordMessageChange").html("Mật khẩu không trùng khớp!").css("color", "red");
                        return false;
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin/User/ChangePasswordUser",
                        data: {
                            id: id,
                            passwordNew: passwordNew,
                            confirmPasswordNew: confirmPasswordNew,
                        },
                        dataType: "json",
                        beforeSend: function () {
                            base.startLoading();
                        },
                        success: function (response) {
                            if (response.succeeded) {
                                base.notify('Chỉnh sửa password thành công', 'success');
                                $('#modal-change-password-user').modal('hide');
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
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/user/GetAllUserPaging",
            data: {
                keyword: $('#txtKeyword').val(),
                roleName: $('#searchRoleName').val(),
                pageSize: base.configs.pageSize,
                pageNumber: base.configs.pageIndex,
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
                            DisplayOrder: stt,
                            Email: item.email,
                            FullName: item.fullName,
                            Id: item.id,
                            AvatarUrl: item.avatarUrl === undefined || item.avatarUrl === null || item.avatarUrl === '' ? '<img src="/assets/images/user.png" width=25 />' : '<img src="' + base.getOrigin() + item.avatarUrl + '" width=25 />',
                            CreatedOn: base.dateTimeFormatJson(item.createdOn),
                            Status: getUserStatus(item.isActive, item.id),
                            EmailConfirmed: getEmailConfirmed(item.emailConfirmed),
                            UserName: item.userName,
                            DateOfBirth: base.dateFormatJson(item.dateOfBirth),
                            PhoneNumber: item.phoneNumber,
                            RoleName: item.roleName,
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

    var changeUserStatus = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/user/ChangeUserStatus",
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

    var deleteUser = function (id) {
        $.ajax({
            type: "POST",
            url: "/admin/user/DeleteUser",
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

    var changeMemberStatus = function (id, status) {
        $.ajax({
            type: "POST",
            url: "/admin/user/ChangeMemberStatus",
            data: {
                id: id,
                memberStatus: status
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

    var getUserStatus = function (status, id) {
        if (status == true)
            return '<button class="btn btn-sm btn-success btn-active" data-id="' + id + '">Active</button>';
        else
            return '<button class="btn btn-sm btn-danger btn-active" data-id="' + id + '">Block</button>';
    }

    var getEmailConfirmed = function (emailConfirm) {
        if (emailConfirm == true)
            return '<span class="btn btn-sm btn-success">Confirmed</span>';
        else
            return '<span class="btn btn-sm btn-danger">Unconfirmed</span>';
    }

    var getMemberStatus = function (status, id) {
        if (status == 1)
            return '<button class="btn btn-sm btn-success btn-memberStatus" data-id="' + id + '" data-status="1">Trial</button>';
        else if (status == 2)
            return '<button class="btn btn-sm btn-warning btn-memberStatus" data-id="' + id + '" data-status="2">Vip1</button>';
        else if (status == 3)
            return '<button class="btn btn-sm btn-danger btn-memberStatus" data-id="' + id + '" data-status="3">Vip2</button>';
    }

    var resetFormMaintainance = function () {
        $('#hidId').val(0);
        $('#email').val('');
        $('#fullName').val('');
        $('#password').val('');
        $('#confirmPassword').val('');
        $('#txtImage').val('');
        $('#txtImageShow').val('');
        $('#imagePreview').attr('src', '/assets/images/user.png');
        $('#dateOfBirth').val('');
        $('#address').val('');
        $('#phoneNumber').val('');
        $('#ddlRoleName').val('');
        $('#ckStatus').prop('checked', true);
        $('.removePass').removeAttr('style');
    }

    var resetFormChangePassword = function () {
        $('#passwordNew').val('');
        $('#confirmPasswordNew').val('');
    }
}