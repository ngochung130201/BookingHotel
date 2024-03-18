$('#formContact').validate({
    errorClass: 'text-danger',
    ignore: [],
    lang: 'en',
    rules: {
        name : {
            required: true
        },
        email: {
            required: true
        },
        service: {
            required: true
        },
        company: {
            required: true
        },
        description: {
            required: true
        }
    }
});
$('#btnContact').on('click', function (e) {
    if ($('#formContact').valid()) {
        e.preventDefault();
        var data = {
            name: $('#nameContact').val(),
            email: $('#emailContact').val(),
            service: $('#serviceContact').val(),
            company: $('#companyContact').val(),
            description: $('#descriptionContact').val(),
        }
        sendContact(data);
    }
});
var sendContact = function (data) {
    $.ajax({
        type: 'POST',
        data: { request: data },
        dataType: 'json',
        url: '/send-contact',
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