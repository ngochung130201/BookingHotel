var TicketSaleController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
        scanQr();
    }

    var registerEvents = function () {
        // Event select page size
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Event search by role
        $("#ddlRoleName").on('change', function () {
            loadData(true);
        });

        // Event search by keyword
        $("#txtKeyword").on('focusout', function () {
            loadData(true);
        });

        // Event change status IsActive of User
        $('body').on('click', '.btn-active', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            changeUserStatus(id);
        });

        // Event detete user
        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            base.confirm('Are you sure want to delete?', function () {
                deleteUser(id);
            });
        });

        // Event change member status
        $('body').on('click', '.btn-memberStatus', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var status = $(this).data('status');
            $('#hidId').val(id);
            // Check status and enable radio button
            if (status == 1) {
                $('#rdTrial').prop('checked', true);
            }
            else if (status == 2) {
                $('#rdVip1').prop('checked', true);
            }
            else if (status == 3) {
                $('#rdVip2').prop('checked', true);
            }
            $('#modal-memberStatus').modal('show');
        });

        // Event save in modal member status
        $('#btnSaveMemberStatus').on('click', function () {
            var id = $('#hidId').val();
            var status = $('input[name=rdMemberStatus]:checked').val();
            changeMemberStatus(id, status);
            $('#modal-memberStatus').modal('hide');
        });
    }

    var loadData = function (isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/ticketsale/GetAllTicketSalePaging",
            data: {
                date: $('#date-filter').val(),
                employee: $('#employee-filter').val(),
                price: $('#price-filter').val(),
                pageSize: base.configs.pageSize,
                pageNumber: base.configs.pageIndex,
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
                            //Id: item.id,
                            DisplayOrder: stt,
                            UpdateBy: item.updateBy,
                            Price: item.price,
                            Symbol: item.symbol,
                            ParentCode: item.parentCode,
                            SerialNumber: item.serialNumber,
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

    var scanQr = function () {
        // Get access to the camera
        navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } })
            .then(function (stream) {
                var video = document.getElementById('qr-video');
                video.srcObject = stream;
                video.setAttribute('playsinline', true);
                video.play();

                // QR code scanning
                var canvas = document.createElement('canvas');
                var context = canvas.getContext('2d');
                setInterval(function () {
                    if (video.readyState === video.HAVE_ENOUGH_DATA) {
                        canvas.width = video.videoWidth;
                        canvas.height = video.videoHeight;
                        context.drawImage(video, 0, 0, canvas.width, canvas.height);
                        var imageData = context.getImageData(0, 0, canvas.width, canvas.height);
                        var code = jsQR(imageData.data, imageData.width, imageData.height);
                        if (code) {
                            if ("https://qrco.de/beTaVb" == code.data) {
                                var isCheck = true;
                                if (isCheck) {
                                    isCheck = false;      
                                    $.ajax({
                                        type: "POST",
                                        url: "/admin/TicketSale/CreateInvoice",
                                        data: {
                                            qr: code.data,
                                        },
                                        dataType: "json",
                                        beforeSend: function () {
                                            base.startLoading();
                                        },
                                        success: function (response) {
                                            if (response.succeeded) {
                                                base.notify(response.messages[0], 'success');
                                                loadData(true);
                                            }
                                            else {
                                                base.notify(response.messages[0], 'error');
                                            }
                                            base.stopLoading();
                                            isCheck = true;   
                                        },
                                        error: function (status) {
                                            console.log(status);
                                            base.notify('No tickets found or tickets have been used', 'error');
                                            isCheck = true;   
                                        }
                                    });
                                }
                            } else {
                                base.notify('No tickets found or tickets have been used', 'error');
                            }
                        }
                    }
                }, 1000);
            })
            .catch(function (err) {
                console.log('Error accessing the camera');
            });
    }
}