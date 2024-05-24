using BusinessLogic.Dtos.VNPayment;
using BusinessLogic.Helpers;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Policy;

namespace BusinessLogic.Services.Common
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VNPaymentRequest request);
        VNPaymentResponse PaymentExecute(IQueryCollection collections);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HttpContext context, VNPaymentRequest request)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VNPayLibrary();
            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString()); 

            vnpay.AddRequestData("vnp_CreateDate", value: request.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));

        }

        public VNPaymentResponse PaymentExecute(IQueryCollection collections)
        {
            throw new NotImplementedException();
        }
    }
}
