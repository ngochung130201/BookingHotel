using AutoMapper;
using BusinessLogic.Contexts;
using BusinessLogic.Dtos.Invoice;
using BusinessLogic.Wrapper;
using Newtonsoft.Json;
using System.Text;

namespace BusinessLogic.Services
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceResponseDto>> Create();
    }

    public class InvoiceService(ApplicationDbContext dbContext, IMapper mapper) : IInvoiceService
    {
        public async Task<Result<InvoiceResponseDto>> Create()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://api-vinvoice.viettel.vn/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/createInvoice/0123456787-081");
                request.Headers.Add("Authorization", "Basic MDEyMzQ1Njc4Ny0wODE6UXVhbmduYW1AMjAyMg==");
                string jsonString = @"
                                    {
                                       ""generalInvoiceInfo"":{
                                          ""invoiceType"":""02GTTT"",
                                          ""templateCode"":""1/001"",
                                          ""invoiceSeries"": ""K23TDN"",
                                          ""currencyCode"":""VND"",
                                          ""adjustmentType"":""1"",
                                          ""paymentStatus"":false,
                                          ""paymentType"":""TM"",
                                          ""paymentTypeName"":""TM"",
                                          ""cusGetInvoiceRight"":true,
                                          ""userName"":""user 1""
                                       },
                                       ""buyerInfo"":{
                                          ""buyerName"":""Khách không xác định"",
                                          ""buyerLegalName"":"""",
                                          ""buyerTaxCode"":"""",
                                          ""buyerAddressLine"":""HN VN"",
                                          ""buyerPhoneNumber"":""0868485488"",
                                          ""buyerEmail"":"""",
                                          ""buyerIdNo"":""123456789"",
                                          ""buyerIdType"":""1""
                                       },
                                       ""sellerInfo"":{
                                          ""sellerLegalName"":""Khách không xác định"",
                                          ""sellerTaxCode"":""0123456787-081"",
                                          ""sellerAddressLine"":""test"",
                                          ""sellerPhoneNumber"":""0868485488"",
                                          ""sellerEmail"":""PerformanceTest1@viettel.com.vn"",
                                          ""sellerBankName"":""vtbank"",
                                          ""sellerBankAccount"":""23423424""
                                       },
                                       ""payments"":[
                                          {
                                             ""paymentMethodName"":""TM""
                                          }
                                       ],
                                       ""itemInfo"":[
                                          {
                                             ""lineNumber"":1,
                                             ""itemCode"":""TICKET"",
                                             ""itemName"":""Vé vào cổng"",
                                             ""unitName"":""Vé"",
                                             ""unitPrice"":100000.0,
                                             ""quantity"":1.0,
                                             ""itemTotalAmountWithoutTax"":100000,
                                             ""taxPercentage"":0.0,
                                             ""taxAmount"":0.0,
                                             ""discount"":0.0,
                                             ""itemDiscount"":0.0
                                          }
                                       ],
                                       ""summarizeInfo"":{
                                          ""sumOfTotalLineAmountWithoutTax"":100000,
                                          ""totalAmountWithoutTax"":100000,
                                          ""totalTaxAmount"":100000.0,
                                          ""totalAmountWithTax"":100000,
                                          ""totalAmountWithTaxInWords"":""Một trăm nghìn đồng chẵn"",
                                          ""discountAmount"":0.0,
                                          ""settlementDiscountAmount"":0.0,
                                          ""taxPercentage"":0.0
                                       },
                                       ""taxBreakdowns"":[
                                          {
                                             ""taxPercentage"":0.0,
                                             ""taxableAmount"":100000,
                                             ""taxAmount"":100000.0
                                          }
                                       ]
                                    }";
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<InvoiceResponseDto>(responseContent);
                    return await Result<InvoiceResponseDto>.SuccessAsync(responseDto);
                }
                
            }
            catch (Exception e)
            {
                return await Result<InvoiceResponseDto>.FailAsync();
            }
            return await Result<InvoiceResponseDto>.FailAsync();
        }
    }

}
