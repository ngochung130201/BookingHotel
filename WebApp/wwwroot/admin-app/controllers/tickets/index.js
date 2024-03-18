var TicketsController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }

    var registerEvents = function () {
        //Init validation
        $('#formMaintainance').validate({
            errorClass: 'text-danger',
            ignore: [],
            lang: 'en',
            rules: {
                txtSerialNumber: {
                    required: true,
                    maxlength: 255,
                },
            }
        });

        $('#txtKeyword').on('focusout', function (e) {
            e.preventDefault();
            loadData();
        });

        // Event select page size
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Event click Add New button
        $("#btn-create").on('click', function () {
            base.setTitleModal('add');
            resetFormMaintainance();
            $("#formMaintainance").validate().resetForm();
            $('#modal-add-edit').modal('show');
        });

        // Event save
        $('#btnSave').on('click', function (e) {
            if ($('#formMaintainance').valid()) {
                e.preventDefault();
                var ticketCategoryId = $('#selTicketCategory').val();
                var quantity = $('#txtQuantity').val();
                var expiryDate = $('#txtExpiryDate').val();
                $.ajax({
                    type: "POST",
                    url: "/Admin/Tickets/SaveEntity",
                    data: {
                        ticketCategoryId: ticketCategoryId,
                        quantity: quantity,
                        expiryDate: expiryDate
                    },
                    dataType: "json",
                    beforeSend: function () {
                        base.startLoading();
                    },
                    success: function (response) {
                        if (response.succeeded) {
                            base.notify(response.messages[0], 'success');
                            $('#modal-add-edit').modal('hide');
                            resetFormMaintainance();
                            base.stopLoading();
                            loadData(true);
                        } else {
                            base.notify(response.messages[0], 'error');
                        }
                    },
                    error: function () {
                        base.notify('Has an error in progress', 'error');
                        base.stopLoading();
                    }
                });
                return false;
            }
        });

        // Event button print
        $('body').on('click', '.btn-print', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.setTitleModal('view');
            $("#formMaintainance").validate().resetForm();
            loadDetail(id);
        });

        // Event button excel
        $('body').on('click', '.btn-excel', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.setTitleModal('edit');
            $("#formMaintainance").validate().resetForm();
            loadExcel(id);
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/tickets/GetAllPaging",
            data: {
                keyword: $('#txtKeyword').val(),
                pageNumber: base.configs.pageIndex,
                pageSize: base.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                $("#lbl-total-records").text(response.totalCount);
                if (response.totalCount > 0) {
                    var stt = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                    $.each(response.data, function (i, item) {
                        render += Mustache.render(template, {
                            Order: stt,
                            Price: item.price,
                            Symbol: item.symbol,
                            ParentCode: item.parentCode,
                            Id: item.id,
                            SerialNumber: "Từ " + item.serialNumberFrom + " đến " + item.serialNumberTo,
                            Note: item.quantity,
                            CreatedBy: item.createdBy,
                            CreatedOn: base.dateTimeFormatJson(item.createdOn)
                        });
                        stt++;
                    });
                    if (render !== undefined) {
                        $('#tbl-content').html(render);
                    }
                    base.wrapPaging(response.totalCount, function () {
                        loadData();
                    }, isPageChanged);
                }
                else {
                    $('#tbl-content').html('');
                }
                base.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    }
    var loadPrintPage = function (tickets) {
        $('#ticketsContainer').html("");
        var ticketsContainer = $('#ticketsContainer');
        var imagesLoaded = 0; // Biến để theo dõi số lượng hình ảnh đã tải
        var totalImages = tickets.length; // Giả sử mỗi ticket có 2 hình ảnh
        let count = 0;
        tickets.forEach(function (ticket,index) {
            count++;
            if (index % 4 === 0) {
                var pageBreakDiv = $('<div class="page-break"></div>');
                ticketsContainer.append(pageBreakDiv);
            }
            var ticketHtml = `
                        <div class="wrap-inner">
                            <div class="caption-header">
                                <h1>Vé tham quan di sản văn hóa thế giới hội an</h1>
                                <div class="sub-infor">
                                    <p class="issuing-unit">Đơn vị phát hành: <span>Ban quản lý di sản văn hóa thế giới hội an</span></p>
                                    <p class="date"><span style="margin-right: 5px;"> Ngày: ${ticket.issueDate}</span> <span>Số seri:<small>${ticket.serialNumber}</small></span></p>
                                </div>
                            </div>
                            <div class="logo">
                                <img src="http://119.82.130.211:5000/assets/images/logo.png" alt="">
                            </div>
                            <div class="main-body">
                                <div class="body-price">
                                    <p class="price">Giá :${ticket.price} đồng</p>
                                    <p class="note">(Bằng chữ: một trăm ngìn đồng)</p>
                                    <p>MTS: 4000534908</p>
                                    <p>KÝ HIỆU: 5C23GMS</p>
                                    <p class="include">
                                        <span>Bao gồm </span>
                                        <span> - Phí tham quan: 40.000 VNĐ <br> - Phí dịch vụ đã bao gồm thuế VAT:<br> 40.000 VNĐ</span>

                                    </p>
                                    <!-- Các thông tin khác -->
                                </div>
                                <div class="image">
                                    <div class="image-big">
                                        <img src="http://119.82.130.211:5000/assets/images/hoian.jpg" alt="">
                                    </div>
                                    <div class="image-small">
                                        <img src="http://119.82.130.211:5000/assets/images/hoianquan-logo.jpg" alt="">
                                        <div id="qrcode${ticket.serialNumber}"></div>
                                    </div>
                                    <div class="body-price copyright">
                                        <p style="font-style: italic;">Để lấy hóa đơn, vui lòng vào link</p>
                                        <p><strong> http://119.82.130.211:5000</strong> nhập mã tra cứu <strong> 14CHLKKDJKLD37463</strong></p>
                                    </div>
                                </div>
                                <!-- Các thông tin khác -->
                            </div>
                        </div>`;
            ticketsContainer.children('.page-break').last().append(ticketHtml);

            var QRCode = ticket.qRCodeData;
            generateQRCode(QRCode, 'qrcode' + ticket.serialNumber);

            // Xử lý sự kiện onload cho mỗi hình ảnh
            $('#qrcode' + ticket.serialNumber + ' img').on('load', function () {
                imagesLoaded++;
                checkAllImagesLoaded();
            });
        });
        function checkAllImagesLoaded() {
            if (imagesLoaded >= totalImages) {
                printTickets();
            }
        }

        function printTickets() {
            var content = $('#modal-view').html();
            var printWindow = window.open('', '');
            printWindow.document.open();
            printWindow.document.write('<html><head><title>Print</title>');

            // Tạo thẻ <link> cho file CSS
            var cssLink = printWindow.document.createElement("link");
            cssLink.href = "http://119.82.130.211:5000/assets/admin/css/print.css"; // Đường dẫn đến file CSS
            cssLink.rel = "stylesheet";
            cssLink.type = "text/css";

            // Chờ cho file CSS được tải xong
            cssLink.onload = function () {
                printWindow.document.body.innerHTML = content; // Chèn nội dung sau khi CSS tải xong
                printWindow.document.close(); // Đóng tài liệu để hoàn tất việc tải

                // Thực hiện in sau một khoảng thời gian ngắn để đảm bảo nội dung đã được hiển thị đúng cách
                setTimeout(function () {
                    printWindow.print(); // Mở hộp thoại in
                    printWindow.close(); // Đóng cửa sổ in sau khi in
                }, 500);
            };

            // Thêm thẻ <link> vào <head>
            printWindow.document.head.appendChild(cssLink);
            printWindow.document.write('</head><body></body></html>'); // Tạo cấu trúc cơ bản của tài liệu

            base.stopLoading();
        }
    }
    var loadDetail = function (id) {
        $.ajax({
            type: "GET",
            url: "/admin/tickets/GetById",
            data: {
                id: id
            },
            dataType: "json",
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                var data = response.data;
                loadPrintPage(data);
                base.stopLoading();
            },
            error: function (status) {
                base.notify('Has an error in progress', 'error');
                base.stopLoading();
            }
        });
    }

    var loadExcel = function (id) {
        $.ajax({
            type: "GET",
            url: "/admin/tickets/ExportExcel",
            data: { id: id },
            xhrFields: {
                responseType: 'blob' // Chỉ định kiểu dữ liệu nhận về là blob
            },
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response, status, xhr) {
                base.stopLoading();

                // Tạo một URL tạm thời cho file blob
                var blob = new Blob([response], { type: xhr.getResponseHeader('Content-Type') });
                var downloadUrl = URL.createObjectURL(blob);

                // Tạo một liên kết tạm thời để download file
                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = getFileNameFromHttpResponse(xhr); // Lấy tên file từ header 'Content-Disposition'
                document.body.appendChild(a);
                a.click();

                // Sau khi click để download, loại bỏ liên kết tạm thời và URL
                setTimeout(function () {
                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(downloadUrl);
                }, 100);
            },
            error: function (xhr, status, error) {
                base.notify('Has an error in progress', 'error');
                base.stopLoading();
            }
        });
    }

    // Hàm để trích xuất tên file từ header 'Content-Disposition'
    function getFileNameFromHttpResponse(xhr) {
        var contentDisposition = xhr.getResponseHeader('Content-Disposition') || '';
        var match = contentDisposition.match(/filename="?([^"]+)"?/);
        return (match && match[1]) || 'default-filename.xlsx';
    }

    var resetFormMaintainance = function () {
        var symbol = generateRandomSymbol();
        $('#txtSymbol').val(symbol);
        $('#txtQuantity').val(10);
        var expiryDate = calculateExpiryDate();
        $('#txtExpiryDate').val(expiryDate);
    }

    function generateRandomSymbol() {
        let result = '';
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
        const charactersLength = characters.length;

        for (let i = 0; i < 7; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }

        return result;
    }
    function calculateExpiryDate() {
        const today = new Date();
        const expiryDate = new Date(today);
        expiryDate.setDate(today.getDate() + 7); // Cộng thêm 7 ngày

        // Định dạng ngày: YYYY-MM-DD
        return expiryDate.toISOString().split('T')[0];
    }

    function generateQRCode(text, id) {
        let qr = qrcode(0, 'H'); // Độ chính xác 'H' cho phép mức độ chịu lỗi cao
        qr.addData(text);
        qr.make();
        document.getElementById(id).innerHTML = qr.createImgTag();
    }
}