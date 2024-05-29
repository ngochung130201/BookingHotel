var BookingController = function () {
  this.initialize = function () {
      
    loadData();
    registerEvents();
  };

  var registerEvents = function () {
    //Init validation
    $("#formMaintainance").validate({
      errorClass: "text-danger",
      ignore: [],
      lang: "en",
      rules: {
        txtReplyBy: {
          maxlength: 255,
        },
      },
    });

    // Event search
    $("#txtKeyword").on("focusout", function (e) {
      e.preventDefault();
      loadData(true);
    });
    $("#txtKeyword").on("keypress", function (e) {
      if (e.which === 13) {
        e.preventDefault();
        loadData(true);
      }
    });
    // Event button edit
    // Event save
    $("#btnSave").on("click", function (e) {
      e.preventDefault();
      saveData(false);
    });

    // Event save
    $("#btnSaveAndContinue").on("click", function (e) {
      e.preventDefault();
      saveData(true);
    });

    // Event button edit
    $("body").on("click", ".btn-edit", function (e) {
      e.preventDefault();
      var id = $(this).data("id");
      $("#btnSaveAndContinue").hide();
      base.setTitleModal("edit");
      $("#formMaintainance").validate().resetForm();
      loadDetail(id);
    });
    // Event select page size
    $("#ddl-show-page").on("change", function () {
      base.configs.pageSize = $(this).val();
      base.configs.pageIndex = 1;
      loadData(true);
    });

    //event click btn-exportExcel
    $("#btn-exportExcel").on("click", function (e) {
      e.preventDefault();
      var keyword = $("#txtKeyword").val();
      window.location.href = "/Admin/Booking/ExportExcel?keyword=" + keyword;
    });

    // Event button delete
    $("body").on("click", ".btn-delete", function (e) {
      e.preventDefault();
      var id = $(this).data("id");
      base.confirm("Bạn có chắc chắn muốn xóa?", function () {
        deteleItem(id);
      });
    });
  };

  // Event change status
  $("body").on("click", ".btn-status", function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    changeStatus(id);
  });
  var changeStatus = function (id) {
    $.ajax({
      type: "POST",
      url: "/admin/Booking/ChangeStatus",
      data: {
        id: id,
      },
      dataType: "json",
      beforeSend: function () {
        base.startLoading();
      },
      success: function (response) {
        if (response.succeeded) {
          base.notify(response.messages[0], "success");
          loadData(true);
        } else {
          base.notify(response.messages[0], "error");
        }
        base.stopLoading();
      },
      error: function (status) {
        console.log(status);
      },
    });
  };

  var loadData = function (isPageChanged) {
    $.ajax({
      type: "GET",
      url: "/Admin/Booking/GetAllPaging",
      data: {
        keyword: $("#txtKeyword").val(),
        pageNumber: base.configs.pageIndex,
        pageSize: base.configs.pageSize,
      },
      dataType: "json",
      beforeSend: function () {
        setTimeout(function () {
          $("#spinnerRow").show();
          base.startLoading();
        }, 2000);
        $("#spinnerRow").show();
        base.startLoading();
      },
      success: function (response) {
        console.log(response);
        var template = $("#table-template").html();
        var render = "";
        $("#lbl-total-records").text(response.totalCount);
        if (response.totalCount > 0) {
          var stt = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
          $.each(response.data, function (i, item) {
            render += Mustache.render(template, {
              Order: stt,
              Id: item.id,
              FullName: item.fullName,
              BookingCode: item.bookingCode,
              TransactionDate: base.dateFormatJson(item.transactionDate),
              CheckInDate: base.dateFormatJson(item.checkInDate),
              CheckOutDate: base.dateFormatJson(item.checkOutDate),
              BookedRoomNumber: item.bookedRoomNumber,
              //// check duplicate services arising value and display it in the table, if not duplicate, display the value
              ServicesArising: item.servicesArising.length > 1 ? item.servicesArising.map(service => service.name).join(", ") : item.servicesArising.length == 0 ? "" :  item.servicesArising[0].name ,
              TotalAmount: item.totalAmount,
              Status: getStatus(item.status, item.id),
            });
            stt++;
          });
          if (render !== undefined) {
            $("#tbl-content").html(render);
          }
          base.wrapPaging(
            response.totalCount,
            function () {
              loadData();
            },
            isPageChanged
          );
        } else {
          $("#tbl-content").html(
            '<tr><td colspan="10" style="text-align: center; vertical-align: middle;">Danh sách trống</td></tr>'
          );
        }
        $("#spinnerRow").hide();
        base.stopLoading();
      },
      error: function (status) {
        console.log(status);
      },
    });
  };

  var deteleItem = function (id) {
    $.ajax({
      type: "POST",
      url: "/Admin/Booking/Delete",
      data: {
        id: id,
      },
      dataType: "json",
      beforeSend: function () {
        base.startLoading();
      },
      success: function (response) {
        if (response.succeeded) {
          base.notify(response.messages[0], "success");
          base.stopLoading();
          loadData(true);
        } else {
          base.notify(response.messages[0], "error");
          base.stopLoading();
        }
      },
      error: function (status) {
        base.notify("Đang xảy ra lỗi!", "error");
        base.stopLoading();
      },
    });
  };
  var resetFormMaintainance = function () {
    $("#hidId").val(0);
    $("#txtBookingCode").val("");
    $("#txtTransactionDate").val("");
    $("#txtCheckInDate").val("");
    $("#txtCheckOutDate").val("");
    $("#txtStatus").val("");
    $("#txtAdult").val("");
    $("#txtKid").val("");
    $("#txtTotalAmount").val("");
    $("#txtDownPayment").val("");
  };

  var loadDetail = function (id) {
    $.ajax({
      type: "GET",
      url: "/Admin/Booking/GetBookingById",
      data: {
        id: id,
      },
      dataType: "json",
      beforeSend: function () {
        base.startLoading();
      },
      success: function (response) {
        if (response.succeeded) {
          var data = response.data;
          $("#hidId").val(data.id);
          $("#txtBookingCode").val(data.bookingCode);
          $("#txtTransactionDate").val(
            data.transactionDate ? data.transactionDate.split("T")[0] : ""
          );
          $("#txtCheckInDate").val(
            data.checkInDate ? data.checkInDate.split("T")[0] : ""
          );
          $("#txtCheckOutDate").val(
            data.checkOutDate ? data.checkOutDate.split("T")[0] : ""
          );
          $("#txtStatus").val(data.status);
          $("#txtAdult").val(data.adult);
          $("#txtKid").val(data.kid);
          $("#txtTotalAmount").val(data.totalAmount);
          $("#txtDownPayment").val(data.downPayment);
          $("#txtPayment").val(data.payment);
          $("#txtMessage").val(data.message);
          $("#txtUserId").val(data.userId);
          $("#txtRoomId").val(data.roomId);
          $("#txtFullName").val(data.fullName);



          $("#modal-add-edit").modal("show");
        } else {
          base.notify(response.messages[0], "error");
        }
        base.stopLoading();
      },
      error: function (status) {
        base.notify("Đang xảy ra lỗi!", "error");
        base.stopLoading();
      },
    });
    console.log("id", id);
  };

  var saveData = function (continueFlg) {
    if ($("#formMaintainance").valid()) {
      var id = $("#hidId").val();
      console.log("id", id);
      var bookingCode = $("#txtBookingCode").val();
      var transactionDate = $("#txtTransactionDate").val();
      var checkInDate = $("#txtCheckInDate").val();
      var checkOutDate = $("#txtCheckOutDate").val();
      var bookedRoomNumber = $("#txtBookedRoomNumber").val();
      var servicesArising = $("#txtServicesArising").val();
      var totalAmount = $("#txtTotalAmount").val();
      var status = $("#txtStatus").val();
      var txtAdult = $("#txtAdult").val();
      var txtKid = $("#txtKid").val();
      var txtDownPayment = $("#txtDownPayment").val();
      var txtPayment = $("#txtPayment").val();
      var txtMessage = $("#txtMessage").val();
      var roomId = $("#txtRoomId").val();
      var fullName = $("#txtFullName").val();
      var txtUserId = $("#txtUserId").val();


      $.ajax({
        type: "POST",
        url: "/Admin/Booking/SaveEntity",
        data: {
          Id: id,
          FullName: fullName,
          BookingCode: bookingCode,
          TransactionDate: transactionDate,
          CheckInDate: checkInDate,
          CheckOutDate: checkOutDate,
          BookedRoomNumber: bookedRoomNumber,
          ServicesArising: servicesArising,
          TotalAmount: totalAmount,
          Status: status,
          Adult: txtAdult,
          Kid: txtKid,
          DownPayment: txtDownPayment,
          Payment: txtPayment,
          Message: txtMessage,
          RoomId: roomId,
          UserId: txtUserId,

        },
        dataType: "json",
        beforeSend: function () {
          base.startLoading();
        },
        success: function (response) {
          if (response.succeeded) {
            continueFlg === true
              ? resetFormMaintainance()
              : $("#modal-add-edit").modal("hide");
            resetFormMaintainance();
            // reload page
            loadData(true);

          } else {
            base.notify(response.messages[0], "error");
            base.stopLoading();
          }
        },
        error: function (status) {
          base.notify("Đang xảy ra lỗi!", "error");
          base.stopLoading();
        },
      });
    }
  };
};
var getStatus = function (status, id) {
  console.log("status", status);
  if (status == true)
    return (
      '<button class="btn btn-sm btn-success btn-status" data-id="' +
      id +
      '">Kích hoạt</button>'
    );
  else
    return (
      '<button class="btn btn-sm btn-danger btn-status" data-id="' +
      id +
      '">Chặn</button>'
    );
};
