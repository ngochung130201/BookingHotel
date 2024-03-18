var page = 1;
var totalPages = parseInt($('#load-more-projects').data('total-page'));
$('#load-more-projects').on('click', function (e) {
    page++;
    $.ajax({
        url: '/project/get-data-paging/' + page,
        type: 'GET',
        success: function (result) {
            $('#load-page-project').append(result);
            if (page >= totalPages) {
                $('#load-more-projects').hide();
            }
        },
        error: function () {
            page--;
        }
    });
});
