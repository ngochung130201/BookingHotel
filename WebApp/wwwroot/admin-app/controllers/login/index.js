var loginController = function () {
    this.initialize = function () {
        registerEvents();
    }

    var registerEvents = function () {
        $('#formLogin').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                userName: {
                    required: true
                },
                password: {
                    required: true
                }
            }
        });
        $('#btnLogin').on('click', function (e) {
            if ($('#formLogin').valid()) {
                e.preventDefault();
                var user = $('#username').val();
                var password = $('#password').val();
                login(user, password);
            }
        });
    }

    var login = function (user, pass) {
        $.ajax({
            type: 'POST',
            data: {
                UserName: user,
                Password: pass
            },
            dataType: 'json',
            url: '/admin/login/authenticate',
            success: function (res) {
                if (res.succeeded) {
                    window.location.href = "/Admin/Dashboard";
                }
                else {
                    base.notify(res.messages[0], 'error');
                }
            }
        })
    }
}