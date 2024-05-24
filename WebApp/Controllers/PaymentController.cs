using BusinessLogic.Dtos.VNPayment;
using BusinessLogic.Services;
using BusinessLogic.Services.Common;
using BusinessLogic.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class PaymentController(IVnPayService vnPayService, 
                                   IBookingService bookingService,
                                   ICurrentUserService currentUserService) : Controller
    {
        [Route("/payment")]
        public IActionResult Index()
        {
            return View();
        }
        
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(string payment)
        {
            if (ModelState.IsValid)
            {
                //var booking = await bookingService.GetById(1);

                var vnPay = new VNPaymentRequest
                {
                    Amount = decimal.Parse("500000"),//booking.Data.TotalAmount,
                    CreatedDate = DateTime.Now,
                    Description = "Payment Success!",
                    FullName = currentUserService.FullName,
                    OrderId = new Random().Next(1000, 10000),//booking.Data.Id

                };
                return Redirect(vnPayService.CreatePaymentUrl(HttpContext, vnPay));
            }

            return new ObjectResult(await Result.FailAsync("Payment Fail!"));
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = "Payment Fail!";
                return RedirectToAction("PaymentFail");
            }

            TempData["Message"] = "Payment Success!";
            return RedirectToAction("PaymentSuccess");
        }
    }
}
