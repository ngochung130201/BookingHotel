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

        //upload image
        $("#fileInputImage").on('change', function () {
            const file = this.files[0];
            if (file) {
                let reader = new FileReader();
                reader.onload = function (event) {
                    $('#txtImage').val(event.target.result);
                    $('#showImageMyAccount').attr('src', event.target.result);
                    $("#txtImage-error").css("display", "none");
                    var settings = {
                        "url": "/Admin/Upload/UploadBase64",
                        "method": "POST",
                        "timeout": 0,
                        "headers": {
                            "Content-Type": "application/json"
                        },
                        "data": JSON.stringify({
                            "base64": event.target.result,
                            "IsConvertToWebp": true
                        }),
                    };
                    $.ajax(settings)
                        .done(function (path) {
                            $('#txtImage').val(path);
                            var imgElement = document.getElementById('txtImageShow');
                            var appDomain = document.getElementById('appDomain');
                            imgElement.src = appDomain.value + path;
                            base.notify('Uploaded successful!', 'success');
                        })
                        .fail(function (xhr, status, error) {
                            base.notify('Error occurred while uploading!', 'error');
                        });
                };
                reader.readAsDataURL(file);
            }
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