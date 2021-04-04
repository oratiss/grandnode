using Grand.Core;
using Grand.Core.Plugins;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Services.Common;
using Grand.Services.Customers;
using Grand.Services.Payments;
using Grand.Services.Stores;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grand.Plugin.Payments.Khanoumi.Enumerations;
using Grand.Plugin.Payments.Khanoumi.Helpers;
using Grpc.Net.Client;
using KhanoumiPyamentGrpc;

namespace Grand.Plugin.Payments.Khanoumi
{
    public class KhanoumiPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly KhanoumiPaymentSetting _khanoumiPaymentSetting;
        private readonly IAddressService _addressService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KhanoumiPaymentProcessor(IWebHelper webHelper, ICustomerService customerService,
            KhanoumiPaymentSetting khanoumiPaymentSetting, IAddressService addressService,
            CustomerSettings customerSettings, IGenericAttributeService genericAttributeService,
            IStoreService storeService, IHttpContextAccessor httpContextAccessor)
        {
            _webHelper = webHelper;
            _customerService = customerService;
            _khanoumiPaymentSetting = khanoumiPaymentSetting;
            _addressService = addressService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _storeService = storeService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Methods
        public override async Task Install()
        {
            await base.Install();
        }

        public override async Task Uninstall()
        {
            await base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentKhanoumi/Configure";
        }


        public async Task<ProcessPaymentResult> ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return await Task.FromResult(result);
        }

        public async Task PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var customer = await _customerService.GetCustomerById(postProcessPaymentRequest.Order.CustomerId);
            var order = postProcessPaymentRequest.Order;

            var total = Convert.ToInt64(Math.Round(order.OrderTotal, 2));
            var isRialtoToman = _khanoumiPaymentSetting.RialToToman;
            if (isRialtoToman) total = total / 10;

            var userPhone = string.Empty;
            var billingAddress = await _addressService.GetAddressByIdSettings(order.BillingAddress.Id);
            var shippingAddress = await _addressService.GetAddressByIdSettings(order.ShippingAddress.Id ?? string.Empty);

            if (_customerSettings.PhoneEnabled)
                userPhone = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.Phone, order.StoreId);
            if (string.IsNullOrEmpty(userPhone))
                userPhone = billingAddress.PhoneNumber;
            if (string.IsNullOrEmpty(userPhone))
                userPhone = string.IsNullOrEmpty(shippingAddress?.PhoneNumber) ? userPhone : $"{userPhone} - {shippingAddress.PhoneNumber}";

            // we have to add a  standard phone number provider and use it.

            var firstName = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.FirstName, order.StoreId);
            var lastName = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.LastName, order.StoreId);

            var fullName = $"{firstName ?? string.Empty} {lastName ?? string.Empty}".Trim();
            var khanoumiGate = 0;
            if (_khanoumiPaymentSetting.GateWayChosenByKhanoumiService)
            {
                khanoumiGate = _khanoumiPaymentSetting.GateWayChosenByKhanoumiService ? (int)_khanoumiPaymentSetting.KhanoumiGateType : 0;
            }

            var description =
                $"{(await _storeService.GetStoreById(order.StoreId)).Name}{(string.IsNullOrEmpty(fullName) ? string.Empty : $" - {firstName}")} - {customer.Email}{(string.IsNullOrEmpty(userPhone) ? string.Empty : $" - {userPhone}")}";
            var callBackUrl = string.Concat(_webHelper.GetStoreLocation(), "Plugins/Payment.Khanoumi/ResultHandler", "?OGUId=" + postProcessPaymentRequest.Order.OrderGuid);
            var grpcAddress = _khanoumiPaymentSetting.GrpcAddress;
            var grpcKey = _khanoumiPaymentSetting.GrpcKey;
            var grpcPassword = _khanoumiPaymentSetting.GrpcPassword;


            //now we do the GRPC Call to get a pre-payment token
            //and redirect user to another View which then redirects user to bank
            var channel = GrpcChannel.ForAddress(grpcAddress);
            var client = new KhanoumiPayment.KhanoumiPaymentClient(channel);
            var tokenRequest = new TokenRequest {
                OrderGuid = order.OrderGuid.ToString(),
                KhanoumiGateType = khanoumiGate,
                Amount = total,
                CallBackUrl = callBackUrl,
                MerchandId = grpcKey,
                MerchantPassword = grpcPassword,
                Mobile = userPhone,
            };
            
            //as it is a multi gate if we have a specified gateway (KhanoumiGate),
            //therefore we are going to provide required field for that gateway before calling grpc.
            switch (khanoumiGate)
            {
                case 6:
                    tokenRequest.Description = description;
                    break;



            }

            var tokenResponse = client.GetToken(tokenRequest);


            //after we take response from KhanoumiGateService, based on KhanoumiGate(Gate Type) we do redirect user to
            //given url of specified bank.

            var uri = new Uri("");
            switch (khanoumiGate)
            {
                case 3://Saman
                    uri = new Uri(tokenResponse.Bankurl+"/?token="+tokenResponse.Token);
                    break;

                case 6: //ZarinPal
                    string urlToRedirect = ZarinPalHelper.ProduceRedirectUrl(_webHelper.GetStoreLocation(),
                    tokenResponse?.Status,
                    _khanoumiPaymentSetting.UseSandBox,
                    tokenResponse.Authority,
                    khanoumiGate.ToString());
                    uri = new Uri(urlToRedirect);
                    break;

            }

            _httpContextAccessor.HttpContext.Response.Redirect(uri.AbsoluteUri);
        }

        public Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(0m);
        }

        public Task<CapturePaymentResult> Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return Task.FromResult(result);
        }

        public Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return Task.FromResult(result);
        }

        public Task<VoidPaymentResult> Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return Task.FromResult(result);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        public Task<bool> CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<IList<string>> ValidatePaymentForm(IFormCollection form)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessPaymentRequest> GetPaymentInfo(IFormCollection form)
        {
            throw new NotImplementedException();
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "KhanoumiPaymentInfo";
        }

        public Task<bool> SupportCapture()
        {
            return Task.FromResult(false);
        }

        public Task<bool> SupportPartiallyRefund()
        {
            return Task.FromResult(true);
        }

        public Task<bool> SupportRefund()
        {
            return Task.FromResult(true);
        }

        public Task<bool> SupportVoid()
        {
            return Task.FromResult(false);
        }

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
        public Task<bool> SkipPaymentInfo()
        {
            return Task.FromResult(false);
        }

        public Task<string> PaymentMethodDescription()
        {
            return Task.FromResult("پرداخت با سرویسهای پرداخت الکترونیک خانومی");
        }
        #endregion
    }
}
