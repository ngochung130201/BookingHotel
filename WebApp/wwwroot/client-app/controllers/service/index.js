var page = 1;
var totalPages = parseInt($('#load-more-services').data('total-page'));
$('#load-more-services').on('click', function (e) {
    page++;
    $.ajax({
        url: '/services/get-data-paging/' + page,
        type: 'GET',
        success: function (result) {
            $('#load-page-service').append(result);
            if (page >= totalPages) {
                $('#load-more-services').hide();
            }
        },
        error: function () {
            page--;
        }
    });
});