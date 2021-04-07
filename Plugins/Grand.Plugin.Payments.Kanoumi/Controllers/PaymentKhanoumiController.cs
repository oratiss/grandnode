using Grand.Domain.Orders;
using Grand.Framework.Controllers;
using Grand.Framework.Mvc.Filters;
using Grand.Plugin.Payments.Khanoumi.Helpers;
using Grand.Plugin.Payments.Khanoumi.Models;
using Grand.Services.Configuration;
using Grand.Services.Orders;
using Grpc.Net.Client;
using KhanoumiPyamentGrpc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Grand.Plugin.Payments.Khanoumi.Controllers
{
    public class PaymentKhanoumiController : BasePaymentController
    {
        private readonly KhanoumiPaymentSetting _khanoumiPaymentSetting;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentKhanoumiController(KhanoumiPaymentSetting khanoumiPaymentSetting, ISettingService settingService,
            IOrderService orderService, IOrderProcessingService orderProcessingService)
        {
            _khanoumiPaymentSetting = khanoumiPaymentSetting;
            _settingService = settingService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
        }



        [HttpGet]
        [AuthorizeAdmin]
        [Area("Admin")]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.GrpcKey = _khanoumiPaymentSetting.GrpcKey;
            model.GrpcPassword = _khanoumiPaymentSetting.GrpcPassword;
            model.GrpcAddress = _khanoumiPaymentSetting.GrpcAddress;
            model.AdditionalFee = _khanoumiPaymentSetting.AdditionalFee;
            model.AdditionalFeePercentage = _khanoumiPaymentSetting.AdditionalFeePercentage;

            return View("~/Plugins/Payments.Khanoumi/Views/PaymentKhanoumi/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area("Admin")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            _khanoumiPaymentSetting.GrpcAddress = model.GrpcAddress;
            _khanoumiPaymentSetting.GrpcKey = model.GrpcKey;
            _khanoumiPaymentSetting.GrpcPassword = model.GrpcPassword;
            _khanoumiPaymentSetting.AdditionalFee = model.AdditionalFee;
            _khanoumiPaymentSetting.AdditionalFeePercentage = model.AdditionalFeePercentage;

            await _settingService.ClearCache();
            await _settingService.SaveSetting(_khanoumiPaymentSetting);
            return View("~/Plugins/Payments.Khanoumi/Views/PaymentKhanoumi/Configure.cshtml", model);

        }

        #region SamanHandlers
        //Todo:SamanResultHandler and SmanaErrorHandler
        //public ActionResult SamanResultHandler(string Status, string Authority, string OGUID)
        //{
        //    if (!(_paymentService.LoadPluginBySystemName("NopTop.Payments.Zarinpal") is ZarinPalPaymentProcessor processor) || !_paymentPluginManager.IsPluginActive(processor))
        //        throw new NopException("ZarinPal module cannot be loaded");

        //    Guid orderNumberGuid = Guid.Empty;
        //    try
        //    {
        //        orderNumberGuid = new Guid(OGUID);
        //    }
        //    catch { }

        //    Order order = _orderService.GetOrderByGuid(orderNumberGuid);
        //    var total = Convert.ToInt32(Math.Round(order.OrderTotal, 2));
        //    if (_ZarinPalPaymentSettings.RialToToman) total = total / 10;

        //    if (string.IsNullOrEmpty(Status) == false && string.IsNullOrEmpty(Authority) == false)
        //    {
        //        string _refId = "0";
        //        System.Net.ServicePointManager.Expect100Continue = false;
        //        int _status = -1;
        //        var storeScope = _storeContext.ActiveStoreScopeConfiguration;
        //        var _ZarinPalSettings = _settingService.LoadSetting<ZarinpalPaymentSettings>(storeScope);

        //        if (_ZarinPalPaymentSettings.Method == EnumMethod.SOAP)
        //        {
        //            if (_ZarinPalPaymentSettings.UseSandbox)
        //                using (ServiceReferenceZarinpalSandBox.PaymentGatewayImplementationServicePortTypeClient ZpalSr = new ServiceReferenceZarinpalSandBox.PaymentGatewayImplementationServicePortTypeClient())
        //                {
        //                    var res = ZpalSr.PaymentVerificationAsync(
        //                        _ZarinPalSettings.MerchantID,
        //                        Authority,
        //                        total).Result; //test
        //                    _status = res.Body.Status;
        //                    _refId = res.Body.RefID.ToString();
        //                }
        //            else
        //                using (ServiceReferenceZarinpal.PaymentGatewayImplementationServicePortTypeClient ZpalSr = new ServiceReferenceZarinpal.PaymentGatewayImplementationServicePortTypeClient())
        //                {
        //                    var res = ZpalSr.PaymentVerificationAsync(
        //                        _ZarinPalSettings.MerchantID,
        //                        Authority,
        //                        total).Result;
        //                    _status = res.Body.Status;
        //                    _refId = res.Body.RefID.ToString();
        //                }
        //        }
        //        else if (_ZarinPalPaymentSettings.Method == EnumMethod.REST)
        //        {
        //            var _url = $"https://{(_ZarinPalPaymentSettings.UseSandbox ? "sandbox" : "www")}.zarinpal.com/pg/rest/WebGate/PaymentVerification.json";
        //            var _values = new Dictionary<string, string>
        //                {
        //                    { "MerchantID", _ZarinPalSettings.MerchantID },
        //                    { "Authority", Authority },
        //                    { "Amount", total.ToString() } //Toman
        //                };

        //            var _paymenResponsetJsonValue = JsonConvert.SerializeObject(_values);
        //            var content = new StringContent(_paymenResponsetJsonValue, Encoding.UTF8, "application/json");

        //            var _response = ZarinPalPaymentProcessor.clientZarinPal.PostAsync(_url, content).Result;
        //            var _responseString = _response.Content.ReadAsStringAsync().Result;

        //            RestVerifyModel _RestVerifyModel =
        //            JsonConvert.DeserializeObject<RestVerifyModel>(_responseString);
        //            _status = _RestVerifyModel.Status;
        //            _refId = _RestVerifyModel.RefID;
        //        }

        //        var result = ZarinpalHelper.StatusToMessage(_status);

        //        var orderNote = new OrderNote() {
        //            OrderId = order.Id,
        //            Note = string.Concat(
        //             "پرداخت ",
        //            (result.IsOk ? "" : "نا"), "موفق", " - ",
        //                "پیغام درگاه : ", result.Message,
        //              result.IsOk ? string.Concat(" - ", "کد پی گیری : ", _refId) : ""
        //              ),
        //            DisplayToCustomer = true,
        //            CreatedOnUtc = DateTime.UtcNow
        //        };

        //        _orderService.InsertOrderNote(orderNote);

        //        if (result.IsOk && _orderProcessingService.CanMarkOrderAsPaid(order))
        //        {
        //            order.AuthorizationTransactionId = _refId;
        //            _orderService.UpdateOrder(order);
        //            _orderProcessingService.MarkOrderAsPaid(order);
        //            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        //        }
        //    }
        //    return RedirectToRoute("orderdetails", new { orderId = order.Id });
        //}
        //public ActionResult SamanErrorHandler(string Error)
        //{
        //    int code = 0;
        //    Int32.TryParse(Error, out code);
        //    if (code != 0)
        //        Error = ZarinpalHelper.StatusToMessage(code).Message;
        //    ViewBag.Err = string.Concat("خطا : ", Error);
        //    return View("~/Plugins/NopTop.Payments.ZarinPal/Views/ErrorHandler.cshtml");
        //} 
        #endregion

        #region ZarinpalHandlers
        public async Task<ActionResult> ZarinpalResultHandler(string Status, string Authority, string OGUID)
        {

            Guid orderNumberGuid = Guid.Empty;
            try
            {
                orderNumberGuid = new Guid(OGUID);
            }
            catch { }

            Order order = await _orderService.GetOrderByGuid(orderNumberGuid);
            var total = Convert.ToInt32(Math.Round(order.OrderTotal, 2));
            if (_khanoumiPaymentSetting.RialToToman) total = total / 10;

            if (string.IsNullOrEmpty(Status) == false && string.IsNullOrEmpty(Authority) == false)
            {
                string refId = "0";
                System.Net.ServicePointManager.Expect100Continue = false;
                int status = -1;

                var khanoumiGate = 6; //zarinpal
                var grpcKey = _khanoumiPaymentSetting.GrpcKey;
                var grpcPassword = _khanoumiPaymentSetting.GrpcPassword;
                var grpcAddress = _khanoumiPaymentSetting.GrpcAddress;
                
                var channel = GrpcChannel.ForAddress(grpcAddress);
                var client = new KhanoumiPayment.KhanoumiPaymentClient(channel);
                var paymentRequest = new PaymentRequest() {
                    GrpcMerchandId = grpcKey,
                    GrpcMerchantPassword = grpcPassword,
                    Amount = total,
                    KhanoumiGateType = khanoumiGate,
                };

                var paymentResult = client.Pay(paymentRequest);

                if (paymentResult.GrpcStatus != 200 || paymentResult.Status != 100)
                {
                    ZarinpalErrorHandler(paymentResult.Status.ToString());
                }

                status = paymentResult.Status;
                refId = paymentResult.RefId;


                var result = ZarinpalHelper.StatusToMessage(status);

                var orderNote = new OrderNote() {
                    OrderId = order.Id,
                    Note = string.Concat(
                     "پرداخت ",
                    (result.IsOk ? "" : "نا"), "موفق", " - ",
                        "پیغام درگاه : ", result.Message,
                      result.IsOk ? string.Concat(" - ", "کد پی گیری : ", refId) : ""
                      ),
                    DisplayToCustomer = true,
                    CreatedOnUtc = DateTime.UtcNow
                };

                _orderService.InsertOrderNote(orderNote);

                if (result.IsOk && _orderProcessingService.CanMarkOrderAsPaid(order).Result)
                {
                    order.AuthorizationTransactionId = refId;
                    _orderService.UpdateOrder(order);
                    _orderProcessingService.MarkOrderAsPaid(order);
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
            }
            return RedirectToRoute("orderdetails", new { orderId = order.Id });
        }
        public ActionResult ZarinpalErrorHandler(string Error)
        {
            int code = 0;
            Int32.TryParse(Error, out code);
            if (code != 0)
                Error = ZarinpalHelper.StatusToMessage(code).Message;
            ViewBag.Err = string.Concat("خطا : ", Error);
            return View("~/Plugins/Payments.Khanoumi/Views/ErrorHandler.cshtml");
        } 
        #endregion
    }
}