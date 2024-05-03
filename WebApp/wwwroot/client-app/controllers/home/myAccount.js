var flagMultiLanguageMyAccount = $('#flag-multi-language').val();
if (flagMultiLanguageMyAccount == 'en-US') {
    $('#change-password-form').validate({
        errorClass: 'text-danger',
        ignore: [],
        lang: 'en',
        rules: {
            currentpassword: {
                required: true
            },
            password: {
                required: true,
                strongPassword: true
            },
            confirmPassword: {
                required: true,
                equalTo: "#password"
            }
        }
    });

    $('#my-account-form').validate({
        errorClass: 'text-danger',
        ignore: [],
        lang: 'en',
        rules: {
            fullname: {
                required: true
            }
        }
    });

} else {
    $('#change-password-form').validate({
        errorClass: 'text-danger',
        ignore: [],
        lang: 'en',
        rules: {
            currentpassword: {
                required: true
            },
            password: {
                required: true,
                strongPassword: true
            },
            confirmPassword: {
                required: true,
                equalTo: "#password"
            }
        },
        messages: {
            currentpassword: {
                required: "Vui lòng nhập mật khẩu hiện tại của bạn."
            },
            password: {
                required: "Vui lòng nhập mật khẩu mới.",
                strongPassword: "Mật khẩu phải chứa ít nhất 9 ký tự, bao gồm ít nhất một chữ hoa, một chữ thường, một số và một ký tự đặc biệt (@$!%*?&)"
            },
            confirmPassword: {
                required: "Vui lòng nhập lại mật khẩu mới.",
                equalTo: "Mật khẩu nhập lại không khớp với mật khẩu mới."
            }
        },
        highlight: function (element, errorClass) {
            $(element).closest('.field').find('.eye_icon').addClass('has-error');
        },
        unhighlight: function (element, errorClass) {
            $(element).closest('.field').find('.eye_icon').removeClass('has-error');
        },
    });

    $('#my-account-form').validate({
        errorClass: 'text-danger',
        ignore: [],
        lang: 'en',
        rules: {
            fullname: {
                required: true
            }
        },
        messages: {
            fullname: {
                required: "Vui lòng nhập Tên đầy đủ của bạn."
            }
        }
    });
}

// Event click Select Img button
$('#btnSelectImg').on('click', function () {
    $('#fileInputImage').click();
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

$('#btnMyAccountSave').on('click', function (e) {
    if ($('#my-account-form').valid()) {
        e.preventDefault();
        var data = {
            Address: $('#address').val(),
            AvatarUrl: $('#txtImage').val(),
            Email: $('#email').val(),
            FullName: $('#fullname').val(),
            PhoneNumber: $('#phonenumber').val(),
        }
        SaveEntity(data)
    }
});

var SaveEntity = function (data) {
    $.ajax({
        type: 'POST',
        data: { request: data },
        dataType: 'json',
        url: '/save-my-account',
        success: function (res) {
            if (res) {
                alert("Success");
                resetFormMaintainance();
                location.reload();
            }
            else {
                alert("Error");
            }
        }
    })
}
$.validator.addMethod("strongPassword", function (value, element) {
    // Kiểm tra chiều dài
    if (value.length < 9) {
        return false;
    }

    // Kiểm tra chữ hoa, chữ thường, số và ký tự đặc biệt
    var hasUpperCase = /[A-Z]/.test(value);
    var hasLowerCase = /[a-z]/.test(value);
    var hasDigit = /\d/.test(value);
    var hasSpecial = /[$!%*?&]/.test(value);

    return hasUpperCase && hasLowerCase && hasDigit && hasSpecial;
}, "Password must contain at least 9 characters, including at least one uppercase letter, one lowercase letter, one number, and one special character ($!%*?&)");

$('#btnChangePassword').on('click', function (e) {
    if ($('#change-password-form').valid()) {
        e.preventDefault();
        var data = {
            CurrentPassword: $('#currentpassword').val(),
            Password: $('#password').val(),
        }
        ChangePassword(data)
    }
});

var ChangePassword = function (data) {
    $.ajax({
        type: 'POST',
        data: { request: data },
        dataType: 'json',
        url: '/change-password',
        success: function (res) {
            if (res.succeeded) {
                $('#error-change-password').text("");
                alert("Success");
                resetFormMaintainance();
                location.reload();
            }
            else {
                $('#error-change-password').text(res.messages);
            }
        }
    })
}

var resetFormMaintainance = function () {
    $('#currentpassword').val('');
    $('#password').val('');
    $('#confirmPassword').val('');
}