using Grand.Core.Plugins;
using Grand.Domain.Orders;
using Grand.Services.Payments;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grand.Core;

namespace Grand.Plugin.Payments.Khanoumi
{
    public class KhanoumiPaymentProcessor: BasePlugin, IPaymentMethod
    {

        private readonly IWebHelper _webHelper;
        public KhanoumiPaymentProcessor(IWebHelper webHelper)
        {
            _webHelper = webHelper;
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


        public Task<ProcessPaymentResult> ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task<bool> SupportCapture()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SupportPartiallyRefund()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SupportRefund()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SupportVoid()
        {
            throw new NotImplementedException();
        }

        public RecurringPaymentType RecurringPaymentType { get; }
        public PaymentMethodType PaymentMethodType { get; }
        public Task<bool> SkipPaymentInfo()
        {
            throw new NotImplementedException();
        }

        public Task<string> PaymentMethodDescription()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
