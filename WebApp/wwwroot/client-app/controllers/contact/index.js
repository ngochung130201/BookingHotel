$('#formContact').validate({
    errorClass: 'text-danger',
    ignore: [],
    lang: 'en',
    rules: {
        nameContact : {
            required: true
        },
        emailContact: {
            required: true
        },
        titleContact: {
            required: true
        },
        contentContact: {
            required: true
        },
    }
});
$('#btnContact').on('click', function (e) {
    if ($('#formContact').valid()) {
        e.preventDefault();
        var data = {
            name: $('#nameContact').val(),
            email: $('#emailContact').val(),
            title: $('#titleContact').val(),
            content: $('#contentContact').val(),
        }
        sendContact(data);
    }
});
var sendContact = function (data) {
    $.ajax({
        type: 'POST',
        data: { request: data },
        dataType: 'json',
        url: '/PostFeedback',
        success: function (res) {
            if (res) {
                alert("Send contact success");
            }
            else {
                alert("Send contact error");
            }
        }
    })
}