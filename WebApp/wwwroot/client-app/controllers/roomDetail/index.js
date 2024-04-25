var RoomsController = function () {
    this.initialize = function () {
        loadData();
        registerControls();
    }

    var loadData = function () {
        $.ajax({
            type: "GET",
            url: "/Room/GetListRoom",
            dataType: "json",
            beforeSend: function () {
                // Bắt đầu hiển thị loading animation
            },
            success: function (response) {
                renderData(response);
            },
            error: function (status) {
                console.log(status);
            },
            complete: function () {
                // Dừng hiển thị loading animation khi hoàn thành AJAX request
            }
        });
    }

    var renderData = function (data) {
        var template = $('#table-template').html();
        var render = "";
        $("#lbl-total-records").text(data.totalCount);
        if (data.totalCount > 0) {
            $.each(data.data, function (i, item) {
                render += Mustache.render(template, {
                    Id: item.id,
                    Name: item.name,
                    Thumbnail: item.thumbnail || "/assets/images/picture.png",
                    Price: item.price,
                });
            });
            $('#accomodation_two').html(render);
        }
    }
}
