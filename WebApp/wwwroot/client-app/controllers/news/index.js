var page = 1;
var totalPages = parseInt($('#load-more-news').data('total-page'));
$('#load-more-news').on('click', function (e) {
    page++;
    $.ajax({
        url: '/blog/get-data-paging/' + page,
        type: 'GET',
        success: function (result) {
            $('#load-page-news').append(result);
            if (page >= totalPages) {
                $('#load-more-news').hide();
            }
        },
        error: function () {
            page--;
        }
    });
});
