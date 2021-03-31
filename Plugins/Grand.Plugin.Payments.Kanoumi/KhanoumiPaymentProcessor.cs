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

        public KhanoumiPaymentProcessor(IWebHelper webHelper, ICustomerService customerService,
            KhanoumiPaymentSetting khanoumiPaymentSetting, IAddressService addressService,
            CustomerSettings customerSettings, IGenericAttributeService genericAttributeService, IStoreService storeService)
        {
            _webHelper = webHelper;
            _customerService = customerService;
            _khanoumiPaymentSetting = khanoumiPaymentSetting;
            _addressService = addressService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _storeService = storeService;
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
            if (_khanoumiPaymentSetting.RialToToman) total = total / 10;

            var userPhone = string.Empty;
            var billingAddress = await _addressService.GetAddressByIdSettings(order.BillingAddress.Id);
            var shippingAddress = await _addressService.GetAddressByIdSettings(order.ShippingAddress.Id ?? string.Empty);

            if (_customerSettings.PhoneEnabled)
                userPhone = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.Phone, order.StoreId);
            if (string.IsNullOrEmpty(userPhone))
                userPhone = billingAddress.PhoneNumber;
            if (string.IsNullOrEmpty(userPhone))
                userPhone = string.IsNullOrEmpty(shippingAddress?.PhoneNumber) ? userPhone : $"{userPhone} - {shippingAddress.PhoneNumber}";

            var firstName = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.FirstName, order.StoreId);
            var lastName = await _genericAttributeService.GetAttributesForEntity<string>(customer, SystemCustomerAttributeNames.LastName, order.StoreId);

            var fullName = $"{firstName ?? string.Empty} {lastName ?? string.Empty}".Trim();
            var urlToRedirect = string.Empty;
            var khanoumiGate = _khanoumiPaymentSetting.UseKhanoumiGate ? _khanoumiPaymentSetting.KhanoumiGateType.ToString() : null;
            var description = 
                $"{(await _storeService.GetStoreById(order.StoreId)).Name}{(string.IsNullOrEmpty(fullName) ? string.Empty : $" - {firstName}")} - {customer.Email}{(string.IsNullOrEmpty(userPhone) ? string.Empty : $" - {userPhone}")}";
            var callBackUrl = string.Concat(_webHelper.GetStoreLocation(), "Plugins/Payment.Khanoumi/ResultHandler", "?OGUId=" + postProcessPaymentRequest.Order.OrderGuid);
            var storeAddress = _webHelper.GetStoreLocation();
            var serverAddress = _khanoumiPaymentSetting.GrpcAddress;
            var grpcKey = _khanoumiPaymentSetting.GrpcKey;
            var grpcPassword = _khanoumiPaymentSetting.GrpcPassword;

            //now we do the GRPC Call





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
            throw new NotImplementedException();
        }

        public Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<VoidPaymentResult> Void(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessPaymentResult> ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanRePostProcessPayment(Order order)
        {
            throw new NotImplementedException();
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
