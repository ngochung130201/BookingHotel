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
                base.notify('Send contact success!', 'success');
                resetFormMaintainance();
            }
            else {
                base.notify('Send contact error!', 'error');
            }
        }
    })
}

var resetFormMaintainance = function () {
    $('#hidId').val(0);
    $('#nameContact').val('');
    $('#emailContact').val('');
    $('#titleContact').val('');
    $('#contentContact').val('');
}