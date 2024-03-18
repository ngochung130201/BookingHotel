var AccountController = function () {
    this.initialize = function () {
        registerEvents();
    }
    var registerEvents = function () {
        $('#btnSave').on('click', function (e) {
            if ($('#formAddUser').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var fullName = $('#fullName').val();
                var avatarUrl = $('#txtImage').val();
                var dateOfBirth = $('#dateOfBirth').val();
                var phoneNumber = $('#phoneNumber').val();
                var address = $('#address').val();
                var currentPassword = $('#currentPassword').val();
                var passwordNew = $('#passwordNew').val();
                var confirmPassword = $('#confirmPassword').val();
                $.ajax({
                    type: "POST",
                    url: "/admin/Account/ChangeProfileUser",
                    data: {
                        id: id,
                        fullName: fullName,
                        avatarUrl: avatarUrl,
                        dateOfBirth: dateOfBirth,
                        phoneNumber: phoneNumber,
                        address: address,
                        passwordCurrent : currentPassword,
                        passwordNew : passwordNew,
                        confirmPassword : confirmPassword,
                    },
                    dataType: "json",
                    beforeSend: function () {
                        base.startLoading();
                    },
                    success: function (response) {
                        if (response.succeeded) {
                            base.notify(response.messages[0], 'success');
                            base.stopLoading();
                            setTimeout(function () {
                                location.reload(true);
                            }, 2000);
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
                    var imgElement = document.getElementById('txtImageShow');
                    var appDomain = document.getElementById('appDomain');
                    imgElement.src = appDomain.value + path;
                    base.notify('Uploaded successful!', 'success');
                },
                error: function () {
                    base.notify('Has an error in progress', 'error');
                }
            });
        });

        $('.eye_icon').on('click', function () {
            var passwordField = $(this).parent().find('input')
            var fieldType = passwordField.attr('type');
            if (fieldType === 'password') {
                passwordField.attr('type', 'text');
                $(this).addClass('active');
            } else {
                passwordField.attr('type', 'password');
                $(this).removeClass('active');
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
    }   
}