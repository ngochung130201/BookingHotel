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
            var imgElement = document.getElementById('showImageMyAccount');
            var appDomain = document.getElementById('appDomain');
            imgElement.src = appDomain.value + path;
        },
        error: function () {
            console.log("hello1")

        }
    });
});

$('#btnMyAccountSave').on('click', function (e) {
    if ($('#my-account-form').valid()) {
        e.preventDefault();
        var data = {
            CompanyName: $('#company').val(),
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
    var hasSpecial = /[@$!%*?&]/.test(value);

    return hasUpperCase && hasLowerCase && hasDigit && hasSpecial;
}, "Password must contain at least 9 characters, including at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&)");

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
            }
            else {
                $('#error-change-password').text(res.messages);
            }
        }
    })
}