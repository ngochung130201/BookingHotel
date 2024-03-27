var base = {
    configs: {
        pageSize: 10,
        pageIndex: 1
    },
    notify: function (message, type) {
        $.notify(message, {
            // whether to hide the notification on click
            clickToHide: true,
            // whether to auto-hide the notification
            autoHide: true,
            // if autoHide, hide after milliseconds
            autoHideDelay: 5000,
            // show the arrow pointing at the element
            arrowShow: true,
            // arrow size in pixels
            arrowSize: 5,
            // position defines the notification position though uses the defaults below
            position: '...',
            // default positions
            elementPosition: 'top right',
            globalPosition: 'top right',
            // default style
            style: 'bootstrap',
            // default class (string or [string])
            className: type,
            // show animation
            showAnimation: 'slideDown',
            // show animation duration
            showDuration: 400,
            // hide animation
            hideAnimation: 'slideUp',
            // hide animation duration
            hideDuration: 200,
            // padding between element and notification
            gap: 2
        });
    },
    confirm: function (message, okCallback) {
        bootbox.confirm({
            message: message,
            buttons: {
                confirm: {
                    label: 'Có',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'Không',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result === true) {
                    okCallback();
                }
            }
        });
    },
    dateFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(datetime.substr(0, 10));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        return day + "/" + month + "/" + year;
    },
    dateTimeFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(datetime.substr(0, 19));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        var ss = newdate.getSeconds();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        if (ss < 10)
            ss = "0" + ss;
        return day + "/" + month + "/" + year + " " + hh + ":" + mm + ":" + ss;
    },

    dateFormatJson: function (datetime) {
        if (datetime == null || datetime == '' || datetime == undefined)
            return '';
        var newdate = new Date(datetime.substr(0, 19));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        return day + "/" + month + "/" + year;
    },

    dateFormatDateOfBirthJson: function (datetime) {
        if (datetime == null || datetime == '' || datetime == undefined)
            return '';
        var newdate = new Date(datetime.substr(0, 19));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        return year + "-" + month + "-" + day;
    },

    startLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading').removeClass('hide');
    },
    stopLoading: function () {
        if ($('.dv-loading').length > 0)
            $('.dv-loading')
                .addClass('hide');
    },
    getStatus: function (status) {
        if (status == 1)
            return '<span class="badge bg-green">Active</span>';
        else
            return '<span class="badge bg-red">Block</span>';
    },
    formatNumber: function (number, precision) {
        if (!isFinite(number)) {
            return number.toString();
        }

        var a = number.toFixed(precision).split('.');
        a[0] = a[0].replace(/\d(?=(\d{3})+$)/g, '$&,');
        return a.join('.');
    },
    unflattern: function (arr) {
        var map = {};
        var roots = [];
        for (var i = 0; i < arr.length; i += 1) {
            var node = arr[i];
            node.children = [];
            map[node.id] = i; // use map to look-up the parents
            if (node.parentId !== null) {
                arr[map[node.parentId]].children.push(node);
            } else {
                roots.push(node);
            }
        }
        return roots;
    },
    convertToSlug: function (Text) {
        return Text
            .toLowerCase()
            .replace(/ /g, '-')
            .replace(/[^\w-]+/g, '');
    },
    wrapPaging: function (recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / base.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').off("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: '<<',
            prev: '<',
            next: '>',
            last: '>>',
            onPageClick: function (event, p) {
                if (base.configs.pageIndex !== p) {
                    base.configs.pageIndex = p;
                    setTimeout(callBack(), 200);
                }
            }
        })
    },
    setTitleModal: function (type) {
        if (type === 'add') {
            $('#modal-add-edit .modal-title').text('Thêm mới');
        } else if (type === 'edit') {
            $('#modal-add-edit .modal-title').text('Chỉnh sửa');
        }
    },
    getOrigin: function () {
        var origin = window.location.origin;
        return origin;
    },

    formatPriceToVietnameseDong: function (price) {
        if (typeof price !== 'number') {
            return 'Invalid input';
        }
        return price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    },
    convertDecimal: function (input) {
        var stringNumber = input.toString();
        if (stringNumber.indexOf('.') !== -1) {
            var roundedNumber = parseFloat(input).toFixed(2);
            var replacedNumber = roundedNumber.replace('.', ',');
            return replacedNumber;
        } else {
            return input;
        }
    },
    getDistrict: function (id) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: "GET",
                url: "/Admin/Common/GetDistrict",
                data: {
                    id: id
                },
                dataType: "json",
                success: function (response) {
                    resolve(response);
                },
                error: function (status) {
                    reject(status);
                }
            });
        });
    },
    getWard: function (id) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: "GET",
                url: "/Admin/Common/GetWard",
                data: {
                    id: id
                },
                dataType: "json",
                success: function (response) {
                    resolve(response);
                },
                error: function (status) {
                    reject(status);
                }
            });
        });
    },
    showError: function (errorList) {
        $.each(errorList, function (index, error) {
            // Tìm phần tử input có id tương ứng
            var inputElement = $('#txt' + error.fieldName);

            // Kiểm tra xem phần tử input có tồn tại không
            if (inputElement.length > 0) {
                // Tạo một thẻ <span> chứa thông báo lỗi
                var errorMessage = $('<label>').addClass('text-danger').text(error.message);

                // Thêm thông báo lỗi dưới phần tử input
                inputElement.after(errorMessage);

                inputElement.on('change', function () {
                    errorMessage.remove();
                });
            }
        });
    },
    isValidDate: function (dateString) {
        // Phân tách ngày, tháng và năm từ chuỗi
        var parts = dateString.split('/');
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[2], 10);

        // Tạo đối tượng Date từ ngày, tháng và năm
        var date = new Date(year, month - 1, day);

        // Kiểm tra xem ngày tạo thành từ chuỗi có giống với ngày tạo từ chuỗi gốc không
        // và kiểm tra xem ngày, tháng và năm trích xuất ra có giống với ngày, tháng và năm ban đầu không
        return date && (date.getMonth() + 1) === month && date.getDate() === day && date.getFullYear() === year;
    },
    convertToISODateString: function (dateString) {
        // Chia chuỗi thành các phần riêng biệt (ngày, tháng, năm)
        var dateParts = dateString.split('/');
        var day = parseInt(dateParts[0]);
        var month = parseInt(dateParts[1]) - 1; // Trừ 1 vì JavaScript tính tháng từ 0 đến 11
        var year = parseInt(dateParts[2]);

        // Tạo đối tượng Date từ các phần đã chia
        var dateObject = new Date(year, month, day);

        // Chuyển đổi sang múi giờ hiện tại
        var currentTimeZoneOffsetInMinutes = dateObject.getTimezoneOffset();
        dateObject.setMinutes(dateObject.getMinutes() - currentTimeZoneOffsetInMinutes);

        if (isNaN(dateObject.getTime())) {
            base.notify('Vui lòng nhập đúng định dạng ngày tháng năm!', 'error');
            return false;
        }
        // Chuyển đổi sang chuỗi định dạng ISO 8601 với múi giờ hiện tại
        return dateObject.toISOString();
    }
}

$(document).ajaxSend(function (e, xhr, options) {
    if (options.type.toUpperCase() == "POST" || options.type.toUpperCase() == "PUT") {
        var token = $('form').find("input[name='__RequestVerificationToken']").val();
        xhr.setRequestHeader("RequestVerificationToken", token);
    }
});

$.validator.addMethod("customNumber", function (value, element) {
    // Check if the field is empty then return true
    if (value.trim() === "") {
        return false;
    }
    // Checks if it is a number and is greater than or equal to 0
    return !isNaN(parseFloat(value)) && parseFloat(value) >= 0;
}, "Requires entering a number greater than or equal to 0");

$.validator.addMethod("customRequired", function (value, element) {
    if (value == null || value.trim() === "") {
        return false;
    }
    return true;
}, "Vui lòng nhập trường này.");

// Thêm phương thức kiểm tra ngày tháng hợp lệ
$.validator.addMethod("validDate", function (value, element) {
    return base.isValidDate(value);
}, "Vui lòng nhập ngày hợp lệ (DD/MM/YYYY)");
